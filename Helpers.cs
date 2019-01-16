using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;

namespace Ibis.Web.MVC.Classes
{
    public static class Helpers
    {

        public static MvcHtmlString BuildHeaders(this HtmlHelper html, IEnumerable<object> model)
        {
            var tr = new TagBuilder("tr");
            var genArgs = model.GetType().GetGenericArguments();
            var type = (genArgs.Length == 2) ? genArgs[1] : genArgs[0];
            MessageBox.Show(model.GetType().IsConstructedGenericType.ToString());
            foreach (MemberInfo prop in type.GetProperties())
            {
                var th = new TagBuilder("th");
                var display = prop.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
                if (display != null) th.InnerHtml = display.Name;
                else if (!prop.Name.Contains("Id")) th.InnerHtml = prop.Name;
                else continue;
                tr.InnerHtml += th.ToString();
            }
            return MvcHtmlString.Create(tr.ToString());
        }
        public static MvcHtmlString BuildRow(this HtmlHelper html, object model)
        {
            var tr = new TagBuilder("tr");
            var type = model.GetType();
            foreach (PropertyInfo prop in type.GetProperties())
            {
                var td = new TagBuilder("td");
                var val = prop.GetValue(model, null);
                string valString = "";
                if (val != null) valString = val.ToString();
                if (!prop.Name.Contains("Id")) td.InnerHtml = valString;
                else continue;
                tr.InnerHtml += td.ToString();
            }
            return MvcHtmlString.Create(tr.ToString());
        }

    }
}