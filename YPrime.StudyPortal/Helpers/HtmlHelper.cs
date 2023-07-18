using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using YPrime.BusinessLayer.Session;
using YPrime.Core.BusinessLayer.Interfaces;
using YPrime.Data.Study.Models;
using YPrime.StudyPortal.Extensions;
using YPrime.StudyPortal.Helpers;

namespace System.Web.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static string DateDisplayFormat = YPrimeSession.Instance.GlobalDateFormat;
        public static string DateTimeDisplayFormat = $"{YPrimeSession.Instance.GlobalDateFormat} HH:mm";
        private static readonly string _missingMessage = "Missing BootstrapMessage";

        public static string DateTimeLabel(this HtmlHelper helper, DateTimeOffset? dateTime, bool utcTime,
            bool includeHoursAndMinutes)
        {
            var result = (dateTime != null
                ? (utcTime ? ((DateTimeOffset) dateTime).UtcDateTime : ((DateTimeOffset) dateTime).DateTime).ToString(
                    includeHoursAndMinutes ? DateTimeDisplayFormat : DateDisplayFormat)
                : string.Empty);
            return result;
        }

        public static string DateTimeLabel(this HtmlHelper helper, DateTime? dateTime, bool includeHoursAndMinutes)
        {
            return dateTime != null
                ? DateTime.Parse(dateTime.ToString())
                    .ToString(includeHoursAndMinutes ? DateTimeDisplayFormat : DateDisplayFormat)
                : "-";
        }

        public static string DateTimeLabel(this HtmlHelper helper, DateTime dateTime, bool includeHoursAndMinutes)
        {
            return dateTime != null
                ? DateTime.Parse(dateTime.ToString())
                    .ToString(includeHoursAndMinutes ? DateTimeDisplayFormat : DateDisplayFormat)
                : "-";
        }

        public static string BooleanLabel(this HtmlHelper helper, bool value, int size = 1)
        {
            return "<span class=\"fa fa-" + (value ? "check-circle grid-true-icon" : "times-circle grid-false-icon") +
                   " fa-" + size + "x\"></span>";
        }

        public static string TranslationLabel(
            string translationKey,
            string currentCultureCode,
            bool wrapInLabelTag = false)
        {
            return GetTranslationLabel(translationKey, currentCultureCode, wrapInLabelTag);
        }

        private static string GetTranslationLabel(
            string translationKey,
            string currentCultureCode,
            bool wrapInLabelTag = false,
            Guid? languageId = null
            ) {
            string result;
            var translationService = DependencyResolver.Current.GetService<ITranslationService>();
            var translation = Task.Run(async () =>
            {
                if ((languageId != null) && (languageId != Guid.Empty))
                {
                    return await translationService.GetByKey(translationKey, null, languageId);
                }
                else
                {
                    return await translationService.GetByKey(translationKey);
                }
            }).Result;
            
            if (wrapInLabelTag)
            {
                result = "<label>" + translation + "</label>";
            }
            else if (translation == null)
            {
                result = translationKey;
            }
            else
            {
                result = translation;
            }

            return result;
        }

        public static string TranslationLabel(
            this HtmlHelper helper, 
            string translationKey, 
            string currentCultureCode,
            bool wrapInLabelTag = false)
        {
            return GetTranslationLabel(translationKey, currentCultureCode, wrapInLabelTag);            
        }

        public static string TranslationLabel(
            this HtmlHelper helper,
            string translationKey,
            Guid? languageId)
        {
            return GetTranslationLabel(translationKey, "", false, languageId);
        }

        public static string TranslationLabel(
            this HtmlHelper helper, 
            string translationKey,
            bool wrapInLabelTag = false)
        {
            return GetTranslationLabel(translationKey, (string)helper.ViewBag.SiteUserCultureCode, wrapInLabelTag);
        }

        public static MvcHtmlString BootstrapMessage(this HtmlHelper helper, string title, string message,
            Guid messageId)
        {
            var stringBuilder = new StringBuilder();
            MvcHtmlString htmlResult;
            message = message ?? _missingMessage;

            stringBuilder.AppendLine("<script type=\"text/javascript\"> ");
            stringBuilder.AppendLine("if(getLastMessageGuid() != '" + messageId + "')");
            stringBuilder.AppendLine("{");

            stringBuilder.AppendLine("BootstrapDialog.show({");
            stringBuilder.AppendLine("message:'" + message.Replace("'", "\\'") + "',");
            stringBuilder.AppendLine("title: '" + title.Replace("'", "\\'") + "',");
            stringBuilder.AppendLine("buttons: [{");
            stringBuilder.AppendLine("label: 'Ok',");
            stringBuilder.AppendLine("cssClass: 'btn-primary',");
            stringBuilder.AppendLine("action: function(dialog){");
            stringBuilder.AppendLine("dialog.close();");
            stringBuilder.AppendLine("}");
            stringBuilder.AppendLine("}]");
            stringBuilder.AppendLine("});");

            stringBuilder.AppendLine("setLastMessageGuid('" + messageId + "');");
            stringBuilder.AppendLine("}");

            stringBuilder.AppendLine("</script>");

            htmlResult = new MvcHtmlString(stringBuilder.ToString());

            return htmlResult;
        }

        public static MvcHtmlString RawActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName,
            string controllerName, object routeValues, AjaxOptions ajaxOptions, object htmlAttributes)
        {
            var replacementId = Guid.NewGuid().ToString();

            if (!UserHasPermission(controllerName, actionName))
            {
                htmlAttributes = new {style = "display:none;"};
            }

            var lnk = ajaxHelper.ActionLink(replacementId, actionName, controllerName, routeValues, ajaxOptions,
                htmlAttributes);

            return MvcHtmlString.Create(lnk.ToString().Replace(replacementId, linkText));
        }

        public static MvcHtmlString PrimeActionLink(this HtmlHelper htmlHelper, string linkText, string actionName,
            string controllerName, object routeValues, object htmlAttributes, bool Authenticate = true)
        {
            return PrimeActionLink(htmlHelper, linkText, actionName, controllerName, routeValues, htmlAttributes,
                Authenticate, false);
        }

        public static MvcHtmlString PrimeActionLink(this HtmlHelper htmlHelper, string linkText, string actionName,
            string controllerName, object routeValues, bool Authenticate = true, bool readOnlyOnUnauthenticated = false)
        {
            return PrimeActionLink(htmlHelper, linkText, actionName, controllerName, routeValues, new { }, Authenticate,
                readOnlyOnUnauthenticated);
        }

        public static MvcHtmlString PrimeActionLink(this HtmlHelper htmlHelper, string linkText, string actionName,
            string controllerName, bool Authenticate = true, bool readOnlyOnUnauthenticated = false)
        {
            return PrimeActionLink(htmlHelper, linkText, actionName, controllerName, new { }, new { }, Authenticate,
                readOnlyOnUnauthenticated);
        }

        public static MvcHtmlString PrimeActionLink(this HtmlHelper htmlHelper, string linkText, string actionName,
            string controllerName, object routeValues, object htmlAttributes, bool Authenticate,
            bool readOnlyOnUnauthenticated = false)
        {
            MvcHtmlString rtn = null;
            if (Authenticate && !UserHasPermission(controllerName, actionName))
            {
                if (readOnlyOnUnauthenticated)
                {
                    rtn = new MvcHtmlString(linkText);
                }
                else
                {
                    htmlAttributes = new {style = "display:none;"};
                    rtn = htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes);
                }
            }
            else
            {
                rtn = htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes);
            }

            return rtn;
        }

        public static MvcHtmlString PrimeActionLink(this HtmlHelper htmlHelper, string LinkText, string actionName,
            string controllerName, string FATag, bool Authenticate = true, bool readOnlyOnUnauthenticated = true)
        {
            return PrimeActionFontAwesomeLink(htmlHelper, LinkText, actionName, controllerName, FATag, new { },
                Authenticate, readOnlyOnUnauthenticated);
        }

        public static MvcHtmlString PrimeJavascriptLink(this HtmlHelper htmlHelper, string LinkText, string actionName,
            string controllerName, string javaScriptFunctionName, bool Authenticate = true,
            bool readOnlyOnUnauthenticated = true, params string[] jscriptParameters)
        {
            return PrimeJavascriptLink(htmlHelper, LinkText, actionName, controllerName, javaScriptFunctionName,
                new { }, Authenticate, readOnlyOnUnauthenticated, jscriptParameters);
        }

        public static MvcHtmlString PrimeJavascriptLink(this HtmlHelper htmlHelper, string LinkText, string actionName,
            string controllerName, string javaScriptFunctionName, object htmlAttributes, bool Authenticate,
            bool readOnlyOnUnauthenticated, params string[] jscriptParameters)
        {
            MvcHtmlString rtn = null;
            if (Authenticate && !UserHasPermission(controllerName, actionName))
            {
                if (readOnlyOnUnauthenticated)
                {
                    rtn = new MvcHtmlString(BuildAnchorTag(LinkText, "", "", javaScriptFunctionName, htmlAttributes,
                        null, jscriptParameters).ToString());
                }
                else
                {
                    htmlAttributes = new {style = "display:none;"};
                }
            }

            var a = BuildAnchorTag(LinkText, actionName, controllerName, javaScriptFunctionName, htmlAttributes, null,
                jscriptParameters);
            rtn = new MvcHtmlString(a.ToString());

            return rtn;
        }

        public static MvcHtmlString PrimeActionFontAwesomeLink(this HtmlHelper htmlHelper, string LinkText,
            string actionName, string controllerName, string FATag, object htmlAttributes, bool Authenticate,
            bool readOnlyOnUnauthenticated)
        {
            MvcHtmlString rtn = null;
            if (Authenticate && !UserHasPermission(controllerName, actionName))
            {
                if (readOnlyOnUnauthenticated)
                {
                    rtn = new MvcHtmlString(BuildAnchorTag(LinkText, "", "", null, htmlAttributes, FATag, null)
                        .ToString());
                }
                else
                {
                    htmlAttributes = new {style = "display:none;"};
                }
            }

            var a = BuildAnchorTag(LinkText, actionName, controllerName, null, htmlAttributes, FATag, null);
            rtn = new MvcHtmlString(a.ToString());

            return rtn;
        }

        private static TagBuilder BuildAnchorTag(string LinkText, string actionName, string controllerName,
            string javaScriptFunctionName, object htmlAttributes, string FATag, params string[] jscriptParameters)
        {
            var URL = new UrlHelper(HttpContext.Current.Request.RequestContext).Action(actionName, controllerName);

            TagBuilder a = new TagBuilder("a");
            a.SetInnerText(LinkText);

            var parameters = string.Empty;
            if (jscriptParameters != null)
            {
                foreach (var param in jscriptParameters)
                {
                    parameters += $"\"{param}\",";
                }

                // Add the action URL
                parameters += $"\"{URL}\"";
            }

            if (javaScriptFunctionName != null)
            {
                // read only on unauthenticated
                if (!string.IsNullOrEmpty(actionName) && !string.IsNullOrEmpty(controllerName))
                {
                    a.Attributes.Add("href", "#");
                    a.Attributes.Add("onclick", $"{javaScriptFunctionName}({parameters})");
                }
            }

            if (htmlAttributes != null)
            {
                a.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            }

            if (FATag != null)
            {
                a.Attributes.Add("href", URL);
                // Build a new tag for the Font awesome stuff and add it as a child to the a Tag.
                TagBuilder FA = new TagBuilder("i");
                FA.AddCssClass(FATag);
                a.InnerHtml = FA + "&nbsp;" + a.InnerHtml;
            }

            return a;
        }

        /**********************************
         * Action Overload
         * ********************************/
        public static MvcHtmlString PrimeAction(this HtmlHelper htmlHelper, string actionName, string controllerName,
            object routeValues, bool authenticate = true)
        {
            var dictionary = new RouteValueDictionary(routeValues);
            return htmlHelper.PrimeAction(actionName, controllerName, dictionary, null, null, authenticate);
        }

        public static MvcHtmlString PrimeAction(this HtmlHelper htmlHelper, string actionName, string controllerName,
            RouteValueDictionary routeValues, bool authenticate = true)
        {
            var dictionary = new RouteValueDictionary(routeValues);
            return htmlHelper.PrimeAction(actionName, controllerName, dictionary, null, null, authenticate);
        }

        public static MvcHtmlString PrimeAction(this HtmlHelper htmlHelper, string actionName, string controllerName,
            object routeValues, string failActionName, string failControllerName, bool authenticate = true)
        {
            var dictionary = new RouteValueDictionary(routeValues);
            return htmlHelper.PrimeAction(actionName, controllerName, dictionary, failActionName, failControllerName,
                authenticate);
        }

        public static MvcHtmlString PrimeAction(this HtmlHelper htmlHelper, string actionName, string controllerName,
            RouteValueDictionary routeValues, string failActionName, string failControllerName,
            bool authenticate = true)
        {
            MvcHtmlString result = null;
            if (authenticate && !UserHasPermission(controllerName, actionName))
            {
                if (!string.IsNullOrEmpty(failActionName) && !string.IsNullOrEmpty(failControllerName))
                {
                    result = htmlHelper.Action(failActionName, failControllerName, routeValues);
                }
            }
            else
            {
                result = htmlHelper.Action(actionName, controllerName, routeValues);
            }

            return result;
        }


        public static bool UserHasPermission(string ControllerName, string ActionName)
        {
            bool Rtn = false;
            var currentUser = YPrimeSession.Instance.CurrentUser;
            //var currentUser = HttpContext.Current.User as CustomPrincipal;
            foreach (var Role in currentUser.Roles)
            {
                Rtn = Role.SystemActions.SingleOrDefault(s =>
                    s.ActionLocation == $"{ControllerName}Controller:{ActionName}") != null;
                if (Rtn)
                {
                    break;
                }
            }

            return (Rtn);
        }

        public static bool UserHasPermission(this HtmlHelper helper, string systemActionName)
        {
            var currentUser = YPrimeSession.Instance.CurrentUser;
            return currentUser.HasPermission(systemActionName);
        }

        public static SelectList ToSelectList<TEnum>(this HtmlHelper helper, TEnum enumObj)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var values = from TEnum e in Enum.GetValues(typeof(TEnum))
                select new {Id = e, Name = e.ToString()};
            return new SelectList(values, "Id", "Name", enumObj);
        }

        public static MvcHtmlString PrimePartial(this HtmlHelper htmlHelper, string actionName, string controllerName,
            string partialViewPath, bool authenticate = true)
        {
            return PrimePartial(htmlHelper, actionName, controllerName, partialViewPath, null, null, authenticate);
        }

        public static MvcHtmlString PrimePartial(this HtmlHelper htmlHelper, string actionName, string controllerName,
            string partialViewPath, object model, bool authenticate = true)
        {
            return PrimePartial(htmlHelper, actionName, controllerName, partialViewPath, model, null, authenticate);
        }

        public static MvcHtmlString PrimePartial(this HtmlHelper htmlHelper, string actionName, string controllerName,
            string partialViewPath, ViewDataDictionary viewDataDictionary, bool authenticate = true)
        {
            return PrimePartial(htmlHelper, actionName, controllerName, partialViewPath, null, viewDataDictionary,
                authenticate);
        }

        public static MvcHtmlString PrimePartial(this HtmlHelper htmlHelper, string actionName, string controllerName,
            string partialViewPath, object model, ViewDataDictionary viewDataDictionary, bool authenticate = true)
        {
            if (authenticate && !UserHasPermission(controllerName, actionName))
            {
                return new MvcHtmlString(string.Empty);
            }

            return Microsoft.Web.Mvc.Html.HtmlHelperExtensions.Partial(htmlHelper, partialViewPath, model,
                viewDataDictionary);
        }

        public static MvcHtmlString PrimePartial(this HtmlHelper htmlHelper, string[] actionNames,
            string[] controllerNames,
            string partialViewPath, bool authenticate = true)
        {
            bool OKToShow = false;
            for (int x = 0; x < actionNames.Length; x++)
            {
                string controllerName = controllerNames[x];
                string actionName = actionNames[x];
                if (UserHasPermission(controllerName, actionName))
                {
                    OKToShow = true;
                }
            }

            if (authenticate && !OKToShow)
            {
                return new MvcHtmlString(string.Empty);
            }

            return Microsoft.Web.Mvc.Html.HtmlHelperExtensions.Partial(htmlHelper, partialViewPath);
        }
    }
}