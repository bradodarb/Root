using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL.Root;
using NHibernate;
using NHibernate.Linq;
using System.Reflection;
using Util.Root;
using Model.Root;
using System.Linq.Expressions;




namespace DAL.Root.NHibernate
{
    public class NHibernateRepositoryBase<T> : IFilteredQueryRepository<T>, INHibernateRepository<T> where T : class, IPersistenceEntity
	{
        public Expression<Func<T, bool>> QueryFilter { get; set; }

        public bool UseCache { get; set; }
		protected ISession _Session;
		protected ITransaction _CurrentTransaction;

		public ISession Session
		{
			get { return _Session; }
		}

		public ITransaction CurrentTransaction
		{
			get { return _CurrentTransaction; }
		}


		public int Count
		{
			get { return GetQueryBase().Count(); }
		}

		public IQueryable<T> Entities
		{
			get { return GetQueryBase(); }
		}
		
        public NHibernateRepositoryBase(bool cache = true)
		{
            UseCache = cache;

		}
       
        public NHibernateRepositoryBase(ISession session, bool cache = true)
		{
			_Session = session;
			_Session.Flush();
			_CurrentTransaction = _Session.BeginTransaction();
            UseCache = cache;
		}

		public T GetById(int id)
		{
			return _Session.Load<T>(id);
			//return this.FirstOrDefault(x => x.Id == id);
		}

		public List<T> GetManyById(params int[] ids)
		{
			List<int> search = new List<int>();

			if (ids != null)
			{
				search.AddRange(ids.ToList());
			}
			return this.Where(x => search.Contains(x.Id)).ToList();
		}

		public T Single()
		{
			return GetQueryBase().Single();
		}

		public T Single(Func<T, bool> expression)
		{
			return GetQueryBase().Single(expression);
		}

		public T SingleOrDefault()
		{
			return GetQueryBase().SingleOrDefault();
		}

		public T SingleOrDefault(Func<T, bool> expression)
		{
			return GetQueryBase().SingleOrDefault(expression);
		}

		public T First()
		{
			return GetQueryBase().First();
		}

		public T First(Func<T, bool> expression)
		{
			return GetQueryBase().First(expression);
		}

		public T FirstOrDefault()
		{
			return GetQueryBase().FirstOrDefault();
		}

		public T FirstOrDefault(Func<T, bool> expression)
		{
            return GetQueryBase().Where(expression).FirstOrDefault();
		}

		public T Last()
		{
			return GetQueryBase().Last();
		}

		public T Last(Func<T, bool> expression)
		{
			return GetQueryBase().Last(expression);
		}

		public T LastOrDefault()
		{
			return GetQueryBase().LastOrDefault();
		}

		public T LastOrDefault(Func<T, bool> expression)
		{
			return GetQueryBase().LastOrDefault(expression);
		}

		public IQueryable<T> Where(System.Linq.Expressions.Expression<Func<T, bool>> expression)
		{
			return GetQueryBase().Where(expression);
		}

		public List<T> ToList()
		{
			return GetQueryBase().ToList();
		}

		public void MarkDirty(T entity)
		{
			throw new NotImplementedException();
		}

		public void InsertOrUpdateOnSubmit(T entity)
		{
			if (IsPersistent(entity))
            {
                entity.Modified = DateTime.Now;
                _Session.Update(entity);
			}
			else
            {
                entity.Created = DateTime.Now;
                entity.Modified = DateTime.Now;
				_Session.Save(entity);
			}
		}

		public void RemoveOnSubmit(T entity)
        {
            _Session.Delete(entity);
		}

		public virtual bool SubmitChanges()
		{
			bool committed = false;

            
			_CurrentTransaction.Commit();

			committed = _CurrentTransaction.WasCommitted;

			//_Session.Flush();
			_CurrentTransaction = _Session.BeginTransaction();
			return committed;
		}

		public virtual bool RollBack()
		{
			bool rolled = false;

			_CurrentTransaction.Rollback();

			rolled = _CurrentTransaction.WasRolledBack;

			_Session.Flush();
			_CurrentTransaction = _Session.BeginTransaction();
			return rolled;
		}

		public virtual void Dispose()
		{
			if (_Session != null)
			{
				if (_Session.IsOpen)
				{
					_Session.Flush();
				}
				_Session.Dispose();
			}
		}

		protected IQueryable<T> GetQueryBase()
		{
            IQueryable<T> result = null;
            if (UseCache)
            {
                result = _Session.Query<T>().Cacheable();
            }
            else
            {
                result = _Session.Query<T>();
            }
            if (QueryFilter != null)
            {
                result = result.Where(QueryFilter);
            }

            return result;
		}

		protected bool IsPersistent(T entity)
		{
			return entity.Id > 0;
		}

		public IEnumerable<T> NativeQuery(string queryString, IDictionary<string, object> parameters)
		{
			ISQLQuery query = Session.CreateSQLQuery(queryString);

			if (parameters != null)
			{
				foreach (KeyValuePair<string, object> item in parameters)
				{
					query.SetParameter(item.Key, item.Value);
				}
			}

			return query.List<T>();
		}

		public IEnumerable<T> NativeQuery(string queryString, object parameters = null)
		{
			return NativeQuery(queryString, DictionaryFromObject(parameters));
		}

		public IEnumerable<T> NamedQuery(string queryString, IDictionary<string, object> parameters)
		{
			IQuery query = Session.GetNamedQuery(queryString);

			if (parameters != null)
			{
				foreach (KeyValuePair<string, object> item in parameters)
				{
					query.SetParameter(item.Key, item.Value);
				}
			}

			return query.List<T>();
		}

		public IEnumerable<T> NamedQuery(string queryString, object parameters = null)
		{
			return NamedQuery(queryString, DictionaryFromObject(parameters));
		}

		private IDictionary<string, object> DictionaryFromObject(object obj, params string[] names)
		{
			Dictionary<string, object> dict = new Dictionary<string, object>();

			if (obj != null)
			{
				var properties = obj.GetType().GetProperties();

				foreach (PropertyInfo prop in properties)
				{
					if (names.Length == 0 || names.Contains(prop.Name))
					{
						dict.Add(prop.Name, prop.GetValue(obj, null));
					}
				}

			}

			return dict;
		}

   
    }
}
