using System;
using Castle.Core.Interceptor;

namespace Pasta.Plugin
{
	/// <summary>
	/// Default exception interceptor to avoid custom exceptions' serialization issues when passing them accross domains.
	/// </summary>
	class ExceptionInterceptor : IInterceptor
	{
		public void Intercept(IInvocation invocation)
		{
			try
			{
				invocation.Proceed();
			}
			catch (Exception ex)
			{
				if (ex.GetType() == typeof(Exception))
				{
					throw;
				}

				// Move everything to message to allow passing cross-domain boundaries.
				throw new Exception(ex.ToString());
			}
		}
	}
}
