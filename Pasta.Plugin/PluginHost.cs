using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Pasta.Plugin
{
	/// <summary>
	/// Host for a plugin in a separate AppDomain.
	/// </summary>
	public class PluginHost : MarshalByRefObject
	{
		private Assembly pluginAssembly;
		private ProxyGenerator proxyGenerator;

		/// <summary>
		/// Loads plugin assembly into the plugin host domain.
		/// </summary>
		/// <param name="pluginAssemblyPath"></param>
		internal void LoadPluginAssembly(string pluginAssemblyPath)
		{
			var domain = AppDomain.CurrentDomain;
			pluginAssembly = domain.Load(AssemblyName.GetAssemblyName(pluginAssemblyPath));
            
            // Required for loading native dependencies
            Directory.SetCurrentDirectory(Path.GetDirectoryName(pluginAssemblyPath));
        }

		/// <summary>
		/// Get the list of plugin types in the plugin assembly.
		/// </summary>
		/// <typeparam name="TInterface">Type of plugin interface.</typeparam>
		/// <returns>The list of plugin type in the assembly.</returns>
		public List<string> GetPluginTypes<TInterface>() where TInterface : class
		{
			if (pluginAssembly == null)
			{
				throw new PluginException("Plugin assembly hasn't been loaded.");
			}

			Type interfaceType = typeof(TInterface);
			if (!interfaceType.IsInterface)
			{
				throw new PluginException("Plugin type should be an interface type.");
			}

			return pluginAssembly
				.GetExportedTypes()
				.Where(type => interfaceType.IsAssignableFrom(type) && !type.IsAbstract)
				.Select(type => type.FullName)
				.ToList();
		}

		/// <summary>
		/// Checks whether the assembly has plugin interfaces.
		/// </summary>
		/// <param name="pluginInterfaces">Array of plugin interfaces' types to verify.</param>
		/// <returns>True if there is at least one type implementing at least on interface for the given array.</returns>
		public bool HasPluginInterfaces(Type[] pluginInterfaces)
		{
			if (pluginAssembly == null)
			{
				throw new PluginException("Plugin assembly hasn't been loaded.");
			}

			var assemblyImplementedInterfaces = pluginAssembly
				.GetExportedTypes()
				.Where(type => !type.IsAbstract)
				.SelectMany(type => type.GetInterfaces())
				.Distinct();

			return assemblyImplementedInterfaces
				.Intersect(pluginInterfaces)
				.Any();
		}

		/// <summary>
		/// Creates a plugin object of the specified type.
		/// </summary>
		/// <typeparam name="TInterface">The interface of plugin.</typeparam>
		/// <param name="pluginTypeName">The name of the type of plugin.</param>
		/// <param name="optionalInterfaceTypes">The list of additional interaces that created proxy should implement if possible.</param>
		/// <returns>The proxy to created plugin object.</returns>
		public TInterface CreatePlugin<TInterface>(string pluginTypeName, params Type[] optionalInterfaceTypes) where TInterface : class
		{
			// Validate plugin assembly.
			if (pluginAssembly == null)
			{
				throw new PluginException("Plugin assembly hasn't been loaded.");
			}

			// Validate interface type.
			var interfaceType = typeof(TInterface);
			if (!interfaceType.IsInterface)
			{
				throw new PluginException("Plugin type should be an interface type.");
			}

			// Validate additional interface types.
			foreach (var optionalInterfaceType in optionalInterfaceTypes)
			{
				if (!optionalInterfaceType.IsInterface)
				{
					throw new PluginException("Optional interface type should be an interface type.");
				}
			}

			// Validate that plugin implements the main interface
			var pluginType = pluginAssembly.GetType(pluginTypeName);
			if (!interfaceType.IsAssignableFrom(pluginType))
			{
				throw new PluginException("Plugin doesn't implement requested interface.");
			}

			// Filter optional interfaces.
			var interfacesToProxy = optionalInterfaceTypes
				.Where(t => t.IsAssignableFrom(pluginType))
				.ToArray();

			// Create a plugin object
			var pluginObj = (TInterface)pluginAssembly.CreateInstance(pluginTypeName);

			// Generate a proxy.
			proxyGenerator = proxyGenerator ?? new ProxyGenerator();
			var proxyOptions = new ProxyGenerationOptions
			{
				BaseTypeForInterfaceProxy = typeof(MarshalByRefObject)
			};

			var proxy = (TInterface)proxyGenerator.CreateInterfaceProxyWithTarget(interfaceType, interfacesToProxy, pluginObj, proxyOptions);

			return proxy;
		}

		/// <summary>
		/// Extracts the resource from plugin assembly.
		/// </summary>
		/// <typeparam name="T">Type of resource.</typeparam>
		/// <param name="resourceName">Resource name.</param>
		/// <returns>The resource.</returns>
		public Stream GetPluginResourceStream(string resourceName)
		{
			// Validate plugin assembly.
			if (pluginAssembly == null)
			{
				throw new PluginException("Plugin assembly hasn't been loaded.");
			}

			var fullResourceName = pluginAssembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith(resourceName));
            if (string.IsNullOrEmpty(fullResourceName))
            {
                throw new PluginException($"Resource name is not found: {resourceName}");
            }

            return pluginAssembly.GetManifestResourceStream(fullResourceName);
		}
    }
}
