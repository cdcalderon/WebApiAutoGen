using OpenQA.Selenium;

namespace YPrime.Web.E2E.Extensions
{
    public static class WebElementSetExtension
    {
        public static void EnterText(this IWebElement element, string value)
        {
            element.Click();
            element.Clear();
            element.SendKeys(value);
        }

        public static IWebElement GetParent(this IWebElement element)
        {
            var parent = element.FindElement(By.XPath("./.."));

            return parent;
        }
    }
}
