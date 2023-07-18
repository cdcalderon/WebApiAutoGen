using Microsoft.Extensions.DependencyInjection;
using System;
using System.Web;

namespace YPrime.StudyPortal
{
    public class ServiceScopeModule : IHttpModule
    {
        private static IServiceProvider _serviceProvider;

        public void Dispose()
        {
            // Method intentionally left empty.
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += Context_BeginRequest;
            context.EndRequest += Context_EndRequest;
        }

        public void Context_BeginRequest(
            object sender,
            EventArgs e)
        {
            var context = GetContext(sender);

            if (context != null)
            {
                context.Items[typeof(IServiceScope)] = _serviceProvider.CreateScope();
            }
        }

        public void Context_EndRequest(
            object sender,
            EventArgs e)
        {
            var context = GetContext(sender);

            if (context != null)
            {
                var scope = context.Items[typeof(IServiceScope)] as IServiceScope;
                scope?.Dispose();
            }
        }

        public static void SetServiceProvider(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private HttpContext GetContext(object eventSender)
        {
            var context = ((HttpApplication)eventSender)?.Context;

            return context;
        }
    }
}