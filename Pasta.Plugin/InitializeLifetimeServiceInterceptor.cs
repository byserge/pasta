using Castle.DynamicProxy;

namespace Pasta.Plugin
{
    /// <summary>
    /// Intercepts MarshalByRefObject.InitializeLifetimeService method to make the object leave forever.
    /// Use selector to make it apply only for InitializeLifetimeService method.
    /// </summary>
    class InitializeLifetimeServiceInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.ReturnValue = null;
        }
    }
}
