using StarterProject.Shared.Helpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StarterProject.App.Helpers;

public static class WebpackHtmlHelper
{
    public static readonly string JsBuildPath = "/resources/responsive/js-build/app/";

    public static IHtmlContent ImportPageJs(this IHtmlHelper helper, string jsFile, object options = null, string className = null)
    {
        return helper.Raw(BuildImportScript(helper, jsFile, options, className));
    }

    private static string BuildImportScript(IHtmlHelper helper, string jsFile, object options = null, string className = null)
    {
        var html = "";

        html += $"<script>import('/dist/js/{jsFile}')";

        var jsonOptions = "";
        if (options != null)
        {
            jsonOptions = options.ToJsonNet();

            // escape anything that may corrupt our script
            jsonOptions = jsonOptions.Replace("</", "<\\/");
        }

        html += string.IsNullOrWhiteSpace(className)
            ? ".then(function(m) { if (m.init) { m.init(" + jsonOptions + "); } })"
            : string.Concat(".then(function(m) { window.controller = new m.", className, "(", jsonOptions, "); })");

        html += @"</script>";

        return html;
    }
}
