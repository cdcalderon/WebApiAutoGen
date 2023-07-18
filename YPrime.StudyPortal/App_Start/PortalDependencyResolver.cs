using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http.Dependencies;
using Microsoft.Extensions.DependencyInjection;

namespace YPrime.StudyPortal
{
    public class PortalDependencyResolver : System.Web.Mvc.IDependencyResolver, System.Web.Http.Dependencies.IDependencyResolver
    {
        protected IServiceProvider serviceProvider;
        protected IServiceScope serviceScope = null;
        private bool disposedValue;

        public PortalDependencyResolver(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public PortalDependencyResolver(IServiceScope scope)
        {
            serviceScope = scope;
            serviceProvider = scope.ServiceProvider;
        }

        public IDependencyScope BeginScope()
        {
            return new PortalDependencyResolver(serviceProvider.CreateScope());
        }

        public object GetService(Type serviceType)
        {
            var scope = GetScope();

            if (scope != null)
            {
                return scope.ServiceProvider.GetService(serviceType);
            }

            return serviceProvider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            var scope = GetScope();

            if (scope != null)
            {
                return scope.ServiceProvider.GetServices(serviceType);
            }

            return serviceProvider.GetServices(serviceType);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private IServiceScope GetScope()
        {
            if (HttpContext.Current != null)
            {
                var scope = HttpContext.Current.Items[typeof(IServiceScope)] as IServiceScope;
                return scope;
            }

            return null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    serviceScope?.Dispose();
                }

                disposedValue = true;
            }
        }
    }
}