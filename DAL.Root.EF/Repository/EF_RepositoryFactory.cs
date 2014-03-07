using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Configuration;
using System.Data.Entity;
using System.Collections;
using System.Data.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Metadata.Edm;
using DAL.Root;
using Util.Root;
using Model.Root;

namespace DAL.Root
{
    public class EF_RepositoryFactory : RepositoryFactoryBase, IRepositoryFactory, IRepositoryFactory_EF
    {
        protected static Hashtable _ContextMap = new Hashtable();
        protected static List<Type> _ContextCache = new List<Type>();

        public IRepository<T> GetRepository<T>(DbContext dataprovider, params object[] args)
            where T : class , IPersistenceEntity
        {
            if (dataprovider != null)
            {
                List<object> arguments = new List<object>();
                arguments.Add(dataprovider);
                if (args != null && args.Length > 0)
                {
                    arguments.AddRange(args);
                }
                var cachereptype = typeof(T);
                if (RepositoryCache.ContainsKey(cachereptype))
                {
                    var item = (Type)RepositoryCache[cachereptype];
                    var instance = Activator.CreateInstance(item, arguments.ToArray()) as IEntityRepository<T>;
                    return instance;
                }
                else
                {
                    if (DefaultRepository != null)
                    {
                        var item = (Type)DefaultRepository;
                        var instance = Activator.CreateInstance(item, arguments.ToArray()) as IEntityRepository<T>;
                        return instance;
                    }
                    else
                    {
                        var result = new EntityRepositoryNoTrackingBase<T>(dataprovider);
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
            var provider = dataprovider as DbContext;
     
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
                        var instance = Activator.CreateInstance(item, arguments.ToArray()) as IEntityRepository<IPersistenceEntity>;
                        return instance;
                    }
                    else
                    {
                        if (DefaultRepository != null)
                        {
                            var item = (Type)DefaultRepository;
                            var instance = Activator.CreateInstance(item, arguments.ToArray()) as IEntityRepository<IPersistenceEntity>;
                            return instance;
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            else
            {
                throw new Exception();
            }
        }
        public IRepository<IPersistenceEntity> GetRepository(Type type, params object[] args)
        {
            var contex = GetContext(type);
            return GetRepository(type, contex, args);
        }
    
        public static void RegisterContext(Type t)
        {
            if (t.BaseType.Equals(typeof(DbContext)))
            {
                if (!_ContextCache.Contains(t))
                {
                    _ContextCache.Add(t);
                }
            }
        }
        private static DbContext GetContext(Type type)
        {
            if (_ContextMap.ContainsKey(type))
            {
                var contexttype = _ContextMap[type] as Type;
                if (contexttype != null)
                {
                    var context = Activator.CreateInstance(contexttype) as DbContext;
                    if (context != null)
                    {
                        return context;
                    }
                }
                else
                {
                    throw new Exception("Invalid Context Requested");
                }
            }
            else
            {
                foreach (var context in _ContextCache)
                {
                    var dbcontext = Activator.CreateInstance(context) as DbContext;
                    if (dbcontext != null)
                    {

                        ObjectContext objContext = ((IObjectContextAdapter)dbcontext).ObjectContext;
                        var test = objContext.MetadataWorkspace;
                        var types = test.GetItems<EdmType>(System.Data.Metadata.Edm.DataSpace.CSpace);
                        foreach (EdmType item in types)
                        {
                            if (item.Name.Equals(type.Name))
                            {
                                if (!_ContextMap.ContainsKey(type))
                                {
                                    _ContextMap.Add(type, context);
                                }
                                return dbcontext;
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}
