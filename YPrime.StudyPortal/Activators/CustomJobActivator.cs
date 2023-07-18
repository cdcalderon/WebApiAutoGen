using Hangfire;
using System;
using System.Web.Mvc;

namespace YPrime.StudyPortal.Activators
{
    public class CustomJobActivator : JobActivator
    {
        public override object ActivateJob(Type jobType)
        {
            return DependencyResolver.Current.GetService(jobType);
        }
    }
}