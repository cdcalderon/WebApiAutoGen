using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using YPrime.BusinessLayer.Interfaces;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.StudyPortal.Attributes;

namespace YPrime.StudyPortal.Controllers
{
    public class FunctionManagementController : BaseController
    {
        private readonly ISystemActionRepository _SysActionRepo;

        public FunctionManagementController(
            ISystemActionRepository SysActionRepo,
            ISessionService sessionService)
            : base(sessionService)
        {
            _SysActionRepo = SysActionRepo;
        }

        public ActionResult Index()
        {
            GetAllFunctionsWithAuthorizationAttribute();
            return View();
        }

        private void GetAllFunctionsWithAuthorizationAttribute()
        {
            // Step 1: Get all controller classes
            var type = typeof(BaseController);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p));

            // Step 2: Get all methods in a given controller that have the FunctionAuthorizationAttribute attached.

            Dictionary<string, dynamic> ActionList = new Dictionary<string, dynamic>();
            foreach (var t in types)
            {
                var members = t.GetMembers().Where(m => Attribute.IsDefined(m, typeof(FunctionAuthorizationAttribute)));
                Debug.WriteLine(members.Count());
                foreach (var member in members)
                {
                    var AuthAttr =
                        (FunctionAuthorizationAttribute) member.GetCustomAttributes(
                            typeof(FunctionAuthorizationAttribute), true)[0];
                    dynamic SysAction = new ExpandoObject();
                    SysAction.Name = AuthAttr.Name;
                    SysAction.Description = AuthAttr.Description;
                    SysAction.IsBlinded = AuthAttr.IsBlinded;
                    SysAction.ActionLocation = $"{t.Name}:{member.Name}";
                    if (!ActionList.ContainsKey(AuthAttr.Name)) // Peeps were reusing function names ...
                    {
                        ActionList.Add(AuthAttr.Name, SysAction);
                    }
                }
            }
            // Step 3: Update/Write methods to the database.

            _SysActionRepo.SaveActionList(ActionList);
        }
    }
}