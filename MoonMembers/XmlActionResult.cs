using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace MoonMembers
{
    public sealed class XmlActionResult : ActionResult
    {
        private readonly XDocument _document;

        public Formatting Formatting { get; set; }
        public string MimeType { get; set; }

        public XmlActionResult(XDocument document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            _document = document;

            // Default values
            MimeType = "text/xml";
            Formatting = Formatting.None;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.ContentType = MimeType;

            using (XmlWriter writer = new XmlWriter(context.HttpContext.Response.OutputStream, Encoding.UTF8) { Formatting = Formatting })
            {
                XmlText ww = new XmlText(context.HttpContext.Response.OutputStream);
                _document.WriteTo(writer);
            }
        }
    }
}