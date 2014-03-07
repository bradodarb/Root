using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Root;
using Util.Root;

namespace DAL.Root
{
    public interface IRepositoryFactory
    {
        IRepository<T> GetRepository<T>(params object[] args)
            where T : class, IPersistenceEntity;


        IRepository<IPersistenceEntity> GetRepository(Type type, object dataprovider, params object[] args);
        IRepository<IPersistenceEntity> GetRepository(Type type, params object[] args);
    }
}