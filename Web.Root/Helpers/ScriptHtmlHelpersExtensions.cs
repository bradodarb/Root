using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Web.Root.Helpers
{
    /// <summary>
    /// Methods for helping to manage scripts in partials and templates.
    /// </summary>
    public static class ScriptHtmlHelperExtensions
    {
        #region AddScriptBlock
        /// <summary>
        ///     Adds a block of script to be rendered out at a later point in the page rendering.
        /// </summary>
        /// <remarks>
        ///     A call to <see cref="RenderScripts(HtmlHelper)" /> will render all scripts.
        /// </remarks>
        /// <param name="htmlHelper">
        ///     the <see cref="HtmlHelper" />
        /// </param>
        /// <param name="script">the block of script to render. The block should not include the &lt;script&gt; tags</param>
        public static void AddScriptBlock(this HtmlHelper htmlHelper, string script)
        {
            AddToScriptContext(htmlHelper, context => context.ScriptBlocks.Add("<script type='text/javascript'>" + script + "</script>"));
        } 
        #endregion

        #region AddScriptBlock
        /// <summary>
        ///     Adds a block of script to be rendered out at a later point in the page rendering.
        /// </summary>
        /// <remarks>
        ///     A call to <see cref="RenderScripts(HtmlHelper)" /> will render all scripts.
        /// </remarks>
        /// <param name="htmlHelper">
        ///     the <see cref="HtmlHelper" />
        /// </param>
        /// <param name="scriptTemplate">
        /// the template for the block of script to render. The template should include the &lt;script&gt; tags</param>
        public static void AddScriptBlock(this HtmlHelper htmlHelper, Func<dynamic, HelperResult> scriptTemplate)
        {
            AddToScriptContext(htmlHelper, context => context.ScriptBlocks.Add(scriptTemplate(null).ToString()));
        } 
        #endregion

        #region AddScriptFile
        /// <summary>
        ///     Adds a script file to be rendered out at a later point in the page rendering.
        /// </summary>
        /// <remarks>
        ///     A call to <see cref="RenderScripts(HtmlHelper)" /> will render all scripts.
        /// </remarks>
        /// <param name="htmlHelper">
        ///     the <see cref="HtmlHelper" />
        /// </param>
        /// <param name="path">the path to the script file to render later</param>
        public static void AddScriptFile(this HtmlHelper htmlHelper, string path)
        {
            AddToScriptContext(htmlHelper, context => context.ScriptFiles.Add(path));
        } 
        #endregion
     
        #region RenderScripts
        /// <summary>
        ///     Renders the scripts out into the view using <see cref="UrlHelper.Content"/> 
        ///     to generate the paths in the &lt;script&gt; elements for the script files
        /// </summary>
        /// <param name="htmlHelper">
        ///     the <see cref="HtmlHelper" />
        /// </param>
        /// <returns>
        ///     an <see cref="IHtmlString" /> of all of the scripts.
        /// </returns>
        public static IHtmlString RenderScripts(this HtmlHelper htmlHelper)
        {
            var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection);

            Func<string[], IHtmlString> scriptPathResolver = paths =>
            {
                var builder = new StringBuilder(paths.Length);
                foreach (var path in paths)
                {
                    builder.AppendLine("<script type='text/javascript' src='" +
                                       urlHelper.Content(path) +
                                       "'></script>");
                }

                return new HtmlString(builder.ToString());
            };

            return RenderScripts(htmlHelper, scriptPathResolver);
        }        
        #endregion

        #region RenderScripts
        /// <summary>
        ///     Renders the scripts out into the view using the passed <paramref name="scriptPathResolver"/> function
        ///     to generate the &lt;script&gt; elements for the script files.
        /// </summary>
        /// <param name="htmlHelper">
        ///     the <see cref="HtmlHelper" />
        /// </param>
        /// <param name="scriptPathResolver">
        ///     a function that is passed the script paths and is used to generate the markup for
        ///     the script elements
        /// </param>
        /// <returns>
        ///     an <see cref="IHtmlString" /> of all of the scripts.
        /// </returns>
        public static IHtmlString RenderScripts(this HtmlHelper htmlHelper, Func<string[], IHtmlString> scriptPathResolver)
        {
            var scriptContext =
                htmlHelper.ViewContext.HttpContext.Items[ScriptContext.ScriptContextItem] as ScriptContext;

            if (scriptContext != null)
            {
                var builder = new StringBuilder();
                var script = new List<string>();

                script.AddRange(scriptContext.ScriptBlocks);

                
                foreach (var s in script)
                {
                    builder.AppendLine(s);
                }

                builder.Append(scriptPathResolver(scriptContext.ScriptFiles.ToArray()).ToString());


                return new HtmlString(builder.ToString());
            }

            return MvcHtmlString.Empty;
        } 
        #endregion

        #region AddToScriptContext
        /// <summary>
        ///     Performs an action on the current <see cref="ScriptContext" />
        /// </summary>
        /// <param name="htmlHelper">
        ///     the <see cref="HtmlHelper" />
        /// </param>
        /// <param name="action">the action to perform</param>
        private static void AddToScriptContext(HtmlHelper htmlHelper, Action<ScriptContext> action)
        {
            var items = htmlHelper.ViewContext.HttpContext.Items;
            var scriptContext = items[ScriptContext.ScriptContextItem] as ScriptContext;

            if (scriptContext == null)
            {
                scriptContext = new ScriptContext(htmlHelper.ViewContext.HttpContext);
                items[ScriptContext.ScriptContextItem] = scriptContext;
            }

            action(scriptContext);
        } 
        #endregion
    }
}
