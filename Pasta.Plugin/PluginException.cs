using System;

namespace Pasta.Plugin
{
    /// <summary>
    /// Base exception for all plugin errors.
    /// </summary>
    public class PluginException : Exception
    {
        public PluginException(string message) : base(message) { }
    }
}
