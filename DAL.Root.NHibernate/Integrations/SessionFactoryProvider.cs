using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DAL.Root.NHibernate.Integrations
{
    public static class SessionFactoryProvider
    {
        public static ISessionFactoryProvider SessionFactory { get; set; }
    }
}
