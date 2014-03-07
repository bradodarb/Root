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
    public class EF_SingleContextRepositoryFactory : RepositoryFactoryBase, IRepositoryFactory, IRepositoryFactory_EF
    {
        protected static Hashtable _ContextMap = new Hashtable();
        protected static List<DbContext> _ContextCache = new List<DbContext>();

        public IRepository<T> GetRepository<T>(DbContext dataprovider, params object[] args)
            where T : class , IPersistenceEntity
        {
            if (dataprovider != null)
            {
                var cachereptype = typeof(T);
                if (RepositoryCache.ContainsKey(cachereptype))
                {
                    var item = (Type)RepositoryCache[cachereptype];

                    Type classType = Type.GetType(item.GetType().ToString() + "`1");
                    Type[] typeParams = new Type[] { typeof(T) };
                    Type constructedType = classType.MakeGenericType(typeParams);

                    var instance =
                    Activator.CreateInstance(constructedType) as IEntityRepository<T>;
                     
                    return instance;
                }
                else
                {
                    if (DefaultRepository != null)
                    {
                        Type classType = Type.GetType(DefaultRepository.GetType().ToString() +"`1");
                        Type[] typeParams = new Type[] { typeof(T) };
                        Type constructedType = classType.MakeGenericType(typeParams);

                        var instance =
                            Activator.CreateInstance(constructedType) as IEntityRepository<T>;
                         
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
                return GetRepository<T>(context);
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
                var cachereptype = type as IPersistenceEntity;
                if (cachereptype != null)
                {
                    if (RepositoryCache.ContainsKey(cachereptype))
                    {
                        var item = (Type)RepositoryCache[cachereptype];
                        var instance = Activator.CreateInstance(item, provider) as IEntityRepository<IPersistenceEntity>;
                        return instance;
                    }
                    else
                    {
                        if (DefaultRepository != null)
                        {
                            var item = (Type)DefaultRepository;
                            var instance = Activator.CreateInstance(item, provider) as IEntityRepository<IPersistenceEntity>;
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
            return GetRepository(type, contex);
        }

        public static void RegisterContext(DbContext t)
        {
            if (!_ContextCache.Contains(t))
            {
                _ContextCache.Add(t);
            }
        }
        private static DbContext GetContext(Type type)
        {
            if (_ContextMap.ContainsKey(type))
            {
                var contexttype = _ContextMap[type] as DbContext;
                if (contexttype != null)
                {
                    return contexttype;
                }
                else
                {
                    throw new Exception("Invalid Context Requested");
                }
            }
            foreach (var context in _ContextCache)
            {
                if (context != null)
                {

                    ObjectContext objContext = ((IObjectContextAdapter)context).ObjectContext;
                    var test = objContext.MetadataWorkspace;
                    var types = test.GetItems<EdmType>(System.Data.Metadata.Edm.DataSpace.CSpace);
                    foreach (EdmType item in types)
                    {
                        if (item.Name.Equals(type.Name))
                        {
                            _ContextMap.Add(type, context);
                            return context;
                        }
                    } 
                }
            }
            return null;
        }
    }
}
