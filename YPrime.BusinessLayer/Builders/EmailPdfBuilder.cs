using System.IO;
using PdfSharp;
using PdfSharp.Drawing;
using TheArtOfDev.HtmlRenderer.Core;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using System.Text.RegularExpressions;

namespace YPrime.BusinessLayer.Builders
{
    public class EmailPdfBuilder
    {
        
        private readonly string _name;
        private string _css;
        private string _body;

        public EmailPdfBuilder(string subject, string body, string cssData = null)
        {
            _name = subject;
            _body = body;
            _css = cssData;
        }

        public EmailPdfBuilder(string subject)
        {
            _name = subject;                        
        }

        public byte[] GetBytes()
        {
            var config = GetPDFConfigSettings();

            CssData css = null;

            if (_css != null)
            {
                css = PdfGenerator.ParseStyleSheet(_css);
            }

            var pdf = PdfGenerator.GeneratePdf(_body, config, css);

            var ms = new MemoryStream();

            pdf.Save(ms, true);

            return ms.ToArray();
        }

        public string GetFileName()
        {
            return $"{_name}.pdf";
        }
        private PdfGenerateConfig GetPDFConfigSettings()
        {
            var config = new PdfGenerateConfig
            {
                PageOrientation = PageOrientation.Portrait,
                MarginBottom = 1,
                MarginLeft = 5,
                MarginRight = 5,
                MarginTop = 1,
                ManualPageSize = new XSize(730, 1000)
            };

            return config;
        }

        public void SetCSS(string css) {
            _css = css;
        }

        private string HideTags(string html, string tagToReplace)
        {
            return html.Replace(tagToReplace, tagToReplace + " style='visibility:hidden;display:none' ");
        }

        public void CreateAttachmentForConfirmationEmail(string Body, string id) {

            //Body is a base64 string containing the html
            byte[] htmlBody = System.Convert.FromBase64String(Body);

            string tmpBody = System.Text.ASCIIEncoding.ASCII.GetString(htmlBody);

            //changes to resize the layout to fit into the pdf

            tmpBody = Regex.Replace(tmpBody, @"<style>([\S\s]*?)</style>", "<style>" + _css + "</style>");
            tmpBody = tmpBody.Replace("<div class=\"row align-bottom\" style=\"position: absolute; bottom: 0px; width: 100%\">", "<br><br><br><div class=\"row align-bottom\" style=\"position: absolute; bottom: 0px; width: 100%\">");
            tmpBody = tmpBody.Replace("<div class=\"ceShadowBox\" style=\"height: 180px;\">", "<div class=\"ceShadowBox\" style=\"height: 160px;\">");
            tmpBody = tmpBody.Replace("class=\"modal-dialog ConfirmationEmailDialog\"", "class=\"ConfirmationEmailDialog\"");
            tmpBody = tmpBody.Replace("modal-header modal-header-ce", "");
            tmpBody = Regex.Replace(tmpBody, @"class=\""([\S\s]*?)\""", "");

            _body = "<html>" + tmpBody + "</html>";
        }

    }
}
