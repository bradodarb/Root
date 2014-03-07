using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//public static class EmbeddedResourceBootstrapper
//{
//    static string _Prefix = "~/CAMS/Embedded";
//    public static void ConfigureResources()
//    {
//        System.Diagnostics.Debug.WriteLine("****************************Resource Bootstrapper");
//        ConfigureRoutes();
//        ConfigureBundles();
//    }

//    private static void ConfigureBundles()
//    {


//        BundleTable.VirtualPathProvider = new EmbeddedVirtualPathProvider(typeof(EmbeddedResourceBootstrapper), _Prefix, HostingEnvironment.VirtualPathProvider);
//        BundleTable.Bundles.Add(new ScriptBundle(_Prefix + "/Js")
//            .Include(_Prefix + "/content/script/embedded1.js")
//            .Include(_Prefix + "/content/script/embedded2.js")
//            );
//        BundleTable.Bundles.Add(new StyleBundle(_Prefix + "/Css")
//            .Include(_Prefix + "/content/css/sample.css")
//            );
//    }

//    private static void ConfigureRoutes()
//    {
//        RouteTable.Routes.Insert(0,
//            new Route(_Prefix + "/{file}.{extension}",
//                new RouteValueDictionary(new { }),
//                new RouteValueDictionary(new { extension = "css|js" }),
//                new EmbeddedResourceRouteHandler(typeof(EmbeddedResourceBootstrapper))
//            ));
//    }
//}