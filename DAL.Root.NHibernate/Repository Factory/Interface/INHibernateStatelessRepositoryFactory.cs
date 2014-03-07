using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Root;
using NHibernate;
using Util.Root;

namespace DAL.Root.NHibernate
{
    public interface INHibernateStatelessRepositoryFactory
    {
        IRepository<T> GetRepository<T>(IStatelessSession session, params object[] args)
                   where T : class , IPersistenceEntity;
    }
}
