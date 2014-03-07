using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Web.Root.Crawler_IO
{
    public static class CrawlerUtils
    {
        public const string EscapedFragment = "_escaped_fragment_";
        public static bool CheckHashBot(HttpRequestBase req)
        {
            return CheckHashBot(req.Url.ToString());
        }
        public static bool CheckHashBot(string url)
        {
            return url.ToLower().Contains(EscapedFragment);
        }
    }
}
