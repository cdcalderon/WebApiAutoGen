using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using Microsoft.Extensions.DependencyInjection;
public class DefaultDependencyResolver : System.Web.Mvc.IDependencyResolver, System.Web.Http.Dependencies.IDependencyResolver
{
    protected IServiceProvider serviceProvider;

    public DefaultDependencyResolver(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public IDependencyScope BeginScope()
    {
        return new DefaultDependencyResolver(this.serviceProvider.CreateScope().ServiceProvider);
    }

    public void Dispose()
    {
    }

    public object GetService(Type serviceType)
    {
        return this.serviceProvider.GetService(serviceType);
    }

    public IEnumerable<object> GetServices(Type serviceType)
    {
        return this.serviceProvider.GetServices(serviceType);
    }
}