using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;

namespace Pasta.Plugin
{
	public class Sponsor : MarshalByRefObject, ISponsor, IDisposable
	{
		private readonly Dictionary<ILease, WeakReference> sponsoredObjects = new Dictionary<ILease, WeakReference>();
		private readonly object lockObj = new object();

		public void Register(object sponsoredObject)
		{
			// Not a marshalled object
			if (!(sponsoredObject is MarshalByRefObject refObject))
				return;

			// Not a proxy
			if (!RemotingServices.IsTransparentProxy(refObject))
				return;

			// No lease-sponsor implementation
			if (!(RemotingServices.GetLifetimeService(refObject) is ILease lease))
				return;

			lock (lockObj)
			{
				Debug.WriteLine($"Lease: " +
				                $"State = {lease.CurrentState}; " +
				                $"CurrentTime = {lease.CurrentLeaseTime}; " +
				                $"InitialTime = {lease.InitialLeaseTime}; " +
				                $"RenewOnCallTime = {lease.RenewOnCallTime} " +
				                $"SponsorshipTimeout = {lease.SponsorshipTimeout}");
				lease.Register(this, TimeSpan.FromMinutes(0.2));
				sponsoredObjects.Add(lease, new WeakReference(sponsoredObject));
			}
		}

		public void Unregister(object sponsoredObject)
		{
			// Not a marshalled object
			if (!(sponsoredObject is MarshalByRefObject refObject))
				return;

			// Not a proxy
			if (!RemotingServices.IsTransparentProxy(refObject))
				return;

			// No lease-sponsor implementation
			if (!(RemotingServices.GetLifetimeService(refObject) is ILease lease))
				return;

			lock (lockObj)
			{
				Debug.WriteLine($"Lease: " +
				                $"State = {lease.CurrentState}; " +
				                $"CurrentTime = {lease.CurrentLeaseTime}; " +
				                $"InitialTime = {lease.InitialLeaseTime}; " +
				                $"RenewOnCallTime = {lease.RenewOnCallTime} " +
				                $"SponsorshipTimeout = {lease.SponsorshipTimeout}");
				lease.Unregister(this);
				sponsoredObjects.Remove(lease);
			}
		}

		public void UnregisterAll()
		{
			lock (lockObj)
			{
				foreach (var lease in sponsoredObjects.Keys)
				{
					Debug.WriteLine($"Lease: " +
					                $"State = {lease.CurrentState}; " +
					                $"CurrentTime = {lease.CurrentLeaseTime}; " +
					                $"InitialTime = {lease.InitialLeaseTime}; " +
					                $"RenewOnCallTime = {lease.RenewOnCallTime} " +
					                $"SponsorshipTimeout = {lease.SponsorshipTimeout}");
					lease.Unregister(this);
				}

				sponsoredObjects.Clear();
			}
		}

		public TimeSpan Renewal(ILease lease)
		{
			lock (lockObj)
			{
				Debug.WriteLine($"Lease: " +
				                $"State = {lease.CurrentState}; " +
				                $"CurrentTime = {lease.CurrentLeaseTime}; " +
				                $"InitialTime = {lease.InitialLeaseTime}; " +
				                $"RenewOnCallTime = {lease.RenewOnCallTime} " +
				                $"SponsorshipTimeout = {lease.SponsorshipTimeout}");
				// No tracking reference - no renewal
				if (!sponsoredObjects.TryGetValue(lease, out var weakRef))
				{
					lease.Unregister(this);
					return TimeSpan.Zero;
				}

				// Object has been GCed without Unregister called - no renewal
				if (!weakRef.IsAlive)
				{
					sponsoredObjects.Remove(lease);
					lease.Unregister(this);
					return TimeSpan.Zero;
				}
			}

			return TimeSpan.FromMinutes(0.2);
		}

		public void Dispose()
		{
			UnregisterAll();
		}
	}
}
