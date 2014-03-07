using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Util.Root;
using Model.Root;

namespace DAL.Root
{
    public interface IRepository<T> : IDisposable where T : class, IPersistenceEntity
    {
        int Count { get; }
        IQueryable<T> Entities { get; }

        T GetById(int id);
        List<T> GetManyById(params int[] ids);
         
        T Single();
        T Single(Func<T, bool> expression);
        T SingleOrDefault();
        T SingleOrDefault(Func<T, bool> expression);

        T First();
        T First(Func<T, bool> expression);
        T FirstOrDefault();
        T FirstOrDefault(Func<T, bool> expression);

        T Last();
        T Last(Func<T, bool> expression);
        T LastOrDefault();
        T LastOrDefault(Func<T, bool> expression);

		IEnumerable<T> NativeQuery(string queryString, IDictionary<string, object> parameters);
		IEnumerable<T> NativeQuery(string queryString, object parameters = null);
		IEnumerable<T> NamedQuery(string queryString, IDictionary<string, object> parameters);
		IEnumerable<T> NamedQuery(string queryString, object parameters = null);

        IQueryable<T> Where(Expression<Func<T, bool>> expression);
        List<T> ToList();
         
        void InsertOrUpdateOnSubmit(T entity);
        void RemoveOnSubmit(T entity);
        bool SubmitChanges();
        bool RollBack();

    }
}
