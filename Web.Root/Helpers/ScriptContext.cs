using System;
using System.Collections.Generic;
using System.Web;

namespace Web.Root.Helpers
{
    /*
    GIFTCARD
 ------------------------------------ */
    /// <summary>
    /// A context in which to add references to script files and blocks of script
    /// to be rendered to the view at a later point.
    /// </summary>
    public class ScriptContext : IDisposable
    {
        internal const string ScriptContextItem = "ScriptContext";

        private readonly HttpContextBase _httpContext;
        private readonly IList<string> _scriptBlocks = new List<string>();
        private readonly HashSet<string> _scriptFiles = new HashSet<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptContext"/> class.
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <exception cref="System.ArgumentNullException">httpContext</exception>
        public ScriptContext(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            _httpContext = httpContext;
        }

        /// <summary>
        /// Gets the script blocks
        /// </summary>
        public IList<string> ScriptBlocks
        {
            get { return _scriptBlocks; }
        }

        /// <summary>
        /// Gets the script files
        /// </summary>
        public HashSet<string> ScriptFiles
        {
            get { return _scriptFiles; }
        }

        public void Dispose()
        {
            var items = _httpContext.Items;
            var scriptContext = items[ScriptContextItem] as ScriptContext;
            scriptContext.ScriptFiles.ExceptWith(ScriptFiles);
            items[ScriptContextItem] = scriptContext;
        }
    }
}
