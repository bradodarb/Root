using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Util.Root;
using Model.Root;

namespace DAL.Root
{
    public interface IFilteredQueryRepository<T> where T : class, IPersistenceEntity
    {
        Expression<Func<T, bool>> QueryFilter { get; set; }
        

    }
}
