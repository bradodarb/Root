using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Root;
using NHibernate;
using Util.Root;

namespace DAL.Root.NHibernate
{
    public class NHibernateRepositoryFactory : RepositoryFactoryBase, IRepositoryFactory, INHibernateRepositoryFactory
    {

        protected static Hashtable _ContextMap = new Hashtable();
        protected static Hashtable _ContextCache = new Hashtable();//String/ISessionFactory

        public IRepository<T> GetRepository<T>(ISession session, params object[] args) where T : class, IPersistenceEntity
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
                        var result = new NHibernateRepositoryBase<T>(session);
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
            var context = GetContext(typeof(T));
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
            var provider = dataprovider as ISession;

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
            var contex = GetContext(type);
            return GetRepository(type, contex, args);
        }

        public static void RegisterContext(ISessionFactoryProvider factory)
        {
            var name = factory.GetType().Name;
            if (!_ContextCache.ContainsKey(name))
            {
                _ContextCache.Add(name, factory.GetFactory());
            }

        }

        private static ISession GetContext(Type type)
        {
            if (_ContextMap.ContainsKey(type))
            {
                var factory = _ContextMap[type] as ISessionFactory;
                if (factory != null)
                {
                    return factory.OpenSession();
                }
                else
                {
                    throw new Exception("Invalid Context Requested");
                }
            }
            else
            {
                foreach (var context in _ContextCache.Values)
                {
                    var factory = context as ISessionFactory;
                    if (factory != null)
                    {
                        var classes = factory.GetAllClassMetadata();
                        foreach (var item in classes)
                        {
                            var poco = item.Value.GetMappedClass(EntityMode.Poco);
                            if (poco.Name.Equals(type.Name))
                            {
                                if (!_ContextMap.ContainsKey(type))
                                {
                                    _ContextMap.Add(type, factory);
                                }
                                return factory.OpenSession();
                            }
                        }
                    }
                }
            }
            return null;
        }

    }
}
