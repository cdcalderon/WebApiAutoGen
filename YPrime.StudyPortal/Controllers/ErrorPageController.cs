using Elmah;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using YPrime.Core.BusinessLayer.Exceptions;
using YPrime.Core.BusinessLayer.Interfaces;

namespace YPrime.StudyPortal.Controllers
{
    public class ErrorPageController : Controller
    {      
        public ActionResult Error(int statusCode, Exception exception)
        {
            ViewBag.StatusCode = statusCode + " Error";
            ViewBag.CustomError = exception.InnerException?.Message ?? exception.Message;
            ErrorSignal.FromCurrentContext().Raise(exception);
            return View();
        }
    }
}