using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Util.Root;
using Model.Root;

namespace DAL.Root
{
    public abstract class RepositoryFactoryBase
    {
        protected static Hashtable RepositoryCache = new Hashtable();
        protected static object DefaultRepository;


        public static void RegisterRepository<T, R>()
            where T : class, IPersistenceEntity
            where R : IRepository<T>
        {
            if (!RepositoryCache.ContainsKey(typeof(T)))
            {
                RepositoryCache.Add(typeof(T), typeof(R));
            }
        }

        public static void RegisterDefaultRepository<T>()
        {
            DefaultRepository = typeof(T);
        }
    }
}
