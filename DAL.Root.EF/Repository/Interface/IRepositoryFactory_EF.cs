using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using DAL.Root;
using Util.Root;
using Model.Root;

namespace DAL.Root
{
    public interface IRepositoryFactory_EF
    {
        IRepository<T> GetRepository<T>(DbContext dataprovider, params object[] args)
            where T : class , IPersistenceEntity;
    }
}
