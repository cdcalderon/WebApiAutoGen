﻿using System;
using System.Web;
using System.Web.Mvc;

public class NoCacheAttribute : ActionFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
        var response = filterContext.HttpContext.Response;
        response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
        response.Cache.SetValidUntilExpires(false);
        response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
        response.Cache.SetCacheability(HttpCacheability.NoCache);
        response.Cache.SetNoStore();
    }
}