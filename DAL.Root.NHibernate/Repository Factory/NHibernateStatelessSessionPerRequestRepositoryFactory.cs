using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Root.NHibernate.Integrations;
using Model.Root;
using NHibernate;
using Util.Root;

namespace DAL.Root.NHibernate
{
    public class NHibernateStatelessSessionPerRequestRepositoryFactory : RepositoryFactoryBase, IRepositoryFactory, INHibernateStatelessRepositoryFactory
    {

        protected static Hashtable _ContextMap = new Hashtable();

        public IRepository<T> GetRepository<T>(IStatelessSession session, params object[] args) where T : class, IPersistenceEntity
        {
            if (session != null)
            {
                List<object> arguments = new List<object>();
                arguments.Add(session);
                if (args != null && args.Length > 0)
                {
                    arguments.AddRange(args);
                }
                var cachereptype = typeof(T);
                if (RepositoryCache.ContainsKey(cachereptype))
                {
                    var item = (Type)RepositoryCache[cachereptype];
                    var instance = Activator.CreateInstance(item, arguments.ToArray()) as INHibernateRepository<T>;
                    return instance;
                }
                else
                {
                    if (DefaultRepository != null)
                    {
                        var item = (Type)DefaultRepository;
                        var instance = Activator.CreateInstance(item, arguments.ToArray()) as INHibernateRepository<T>;
                        return instance;
                    }
                    else
                    {
                        var result = new NHibernateStatelessRepositoryBase<T>(session);
                        return result;
                    }
                }
            }
            else
            {
                throw new Exception();
            }
        }

        public IRepository<T> GetRepository<T>(params object[] args)
            where T : class , IPersistenceEntity
        {
            var context = GetContext();
            if (context != null)
            {
                return GetRepository<T>(context, args);
            }
            else
            {
                throw new Exception("Context for type not registered");
            }
        }

        public IRepository<IPersistenceEntity> GetRepository(Type type, object dataprovider, params object[] args)
        {
            var provider = dataprovider as IStatelessSession;

            if (provider != null)
            {
                List<object> arguments = new List<object>();
                arguments.Add(provider);
                if (args != null && args.Length > 0)
                {
                    arguments.AddRange(args);
                }
                var cachereptype = type as IPersistenceEntity;
                if (cachereptype != null)
                {
                    if (RepositoryCache.ContainsKey(cachereptype))
                    {
                        var item = (Type)RepositoryCache[cachereptype];
                        var instance = Activator.CreateInstance(item, arguments.ToArray()) as INHibernateRepository<IPersistenceEntity>;
                        return instance;
                    }
                    else
                    {
                        if (DefaultRepository != null)
                        {
                            var item = (Type)DefaultRepository;
                            var instance = Activator.CreateInstance(item, arguments.ToArray()) as INHibernateRepository<IPersistenceEntity>;
                            return instance;
                        }
                        else
                        {
                            throw new Exception("No Default Repository found!");
                        }
                    }
                }
                else
                {
                    throw new Exception("Requested Type does not implement the IPersistenceEntity interface");
                }
            }
            else
            {
                throw new Exception("Provider not capable of creating a session");
            }
        }

        public IRepository<IPersistenceEntity> GetRepository(Type type, params object[] args)
        {
            var contex = GetContext();
            return GetRepository(type, contex, args);
        }

        private static IStatelessSession GetContext()
        {
            return NHibernateStatelessSessionManager.Instance.GetSession();
        }

    }
}
