
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;

namespace DAL.Root.NHibernate
{
    public interface ISessionFactoryProvider
    {

        ISessionFactory GetFactory();

    }
}
