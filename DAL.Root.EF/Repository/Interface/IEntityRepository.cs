using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using DAL.Root;
using Util.Root;
using Model.Root; 

namespace DAL.Root
{
    public interface IEntityRepository<T> : IRepository<T>
        where T : class, IPersistenceEntity
    {
        DbContext Context { get; set; }
        DbEntityEntry Entry(object entity);
        DbEntityEntry<T> Entry(T entity);
    }
}
