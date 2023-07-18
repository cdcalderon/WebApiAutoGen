using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using YPrime.Core.BusinessLayer.Exceptions;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.StudyPortal.Controllers;

namespace YPrime.StudyPortal.Attributes
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        private List<Type> CustomExceptionTypes = new List<Type>() 
        { 
            typeof(NoProductionConfigurationException),
            typeof(StudyAccessDeactivatedException) 
        };     

        public override void OnException(ExceptionContext filterContext)
        {
            var exception = filterContext.Exception;
            var exceptionType = filterContext.Exception.GetType();

            if (filterContext.ExceptionHandled)
            {
                return;
            }

            // If it is one of these types show the custom errors page.
            if (CustomExceptionTypes.Contains(exceptionType) ||
                  exceptionType == typeof(AggregateException) &&
                  CustomExceptionTypes.Contains(exception.InnerException.GetType()))
            {

                int statusCode = (int)HttpStatusCode.InternalServerError;               

                var routeData = new RouteData();
                routeData.Values.Add("controller", "ErrorPage");
                routeData.Values.Add("action", "Error");
                routeData.Values.Add("exception", exception);
                routeData.Values.Add("statusCode", statusCode);            

                IController controller = new ErrorPageController();
                filterContext.RequestContext.RouteData = routeData;
                filterContext.RequestContext.HttpContext = new HttpContextWrapper(HttpContext.Current);
                controller.Execute(filterContext.RequestContext);
                filterContext.ExceptionHandled = true;      
            }

            base.OnException(filterContext);            
        }
    }
}