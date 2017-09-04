using System;
using System.Runtime.Serialization;

namespace Pasta.Plugin
{
    /// <summary>
    /// Base exception for all plugin errors.
    /// </summary>
    [Serializable]
    public class PluginException : Exception
    {
        public PluginException(string message) : base(message) { }

        public PluginException() : base() { }

        protected PluginException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
