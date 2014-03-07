using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL.Root;
using Model.Root;
using NHibernate;
using Util.Root;

namespace DAL.Root.NHibernate
{
    public interface INHibernateStatelessRepository<T> : IRepository<T> where T : class, IPersistenceEntity
    {
        IStatelessSession Session { get; }
        ITransaction CurrentTransaction { get; }
    }
}
