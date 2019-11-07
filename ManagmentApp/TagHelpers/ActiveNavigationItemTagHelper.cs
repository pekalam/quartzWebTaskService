using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace ManagmentApp.TagHelpers
{
    [HtmlTargetElement("li")]
    public class ActiveNavigationItemTagHelper : TagHelper
    {
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeName("active-on")]
        public string ItemName { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (ViewContext.RouteData.Values["page"].ToString().Split('/').Contains(ItemName))
            {
                output.AddClass("active", HtmlEncoder.Default);
            }
        }
    }
}
