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
    public partial class EntityRepositoryBase<T> : IDisposable,
        IEntityRepository<T>,
        IRepository<T>
        where T : class, IPersistenceEntity
    {
        protected DbContext _Context;

        public int Count
        {
            get { return _Context.Set<T>().Count(); }
        }

        public IQueryable<T> Entities
        {
            get { return _Context.Set<T>(); }
        }

        public DbContext Context
        {
            get
            {
                return _Context;
            }
            set
            {
                OnContextChanging(value);
                var old = _Context;
                _Context = value;
                OnContextChanged(old);
            }
        }



        public EntityRepositoryBase(DbContext context)
        {
            Context = context;
        }
        public T GetById(int id)
        {
            return SingleOrDefault(x => x.Id == id);
        }
        public List<T> GetManyById(params int[] ids)
        {
            var list = ids.ToList();
            return Where(x => list.Contains(x.Id)).ToList();
        }

        public virtual T Single()
        {
            var result = _Context.Set<T>().Single();
            OnValidateSingle(result);
            return result;

        }

        public virtual T Single(Func<T, bool> expression)
        {
            var result = _Context.Set<T>().Single(expression);
            OnValidateSingle(result);
            return result;
        }

        public virtual T SingleOrDefault()
        {
            var result = _Context.Set<T>().SingleOrDefault();
            OnValidateSingleOrDefault(result);
            return result;
        }

        public virtual T SingleOrDefault(Func<T, bool> expression)
        {
            var result = _Context.Set<T>().SingleOrDefault(expression);
            OnValidateSingleOrDefault(result);
            return result;
        }


        public virtual T First()
        {
            var result = _Context.Set<T>().First();
            OnValidateFirst(result);
            return result;
        }

        public virtual T First(Func<T, bool> expression)
        {
            var result = _Context.Set<T>().First(expression);
            OnValidateFirst(result);
            return result;
        }

        public virtual T FirstOrDefault()
        {
            var result = _Context.Set<T>().FirstOrDefault();
            OnValidateFirstOrDefault(result);
            return result;
        }

        public virtual T FirstOrDefault(Func<T, bool> expression)
        {
            var result = _Context.Set<T>().FirstOrDefault(expression);
            OnValidateFirstOrDefault(result);
            return result;
        }


        public virtual T Last()
        {
            var result = _Context.Set<T>().FirstOrDefault();
            OnValidateLast(result);
            return result;
        }

        public virtual T Last(Func<T, bool> expression)
        {
            var result = _Context.Set<T>().FirstOrDefault(expression);
            OnValidateLast(result);
            return result;
        }

        public virtual T LastOrDefault()
        {
            var result = _Context.Set<T>().LastOrDefault();
            OnValidateLastOrDefault(result);
            return result;
        }

        public virtual T LastOrDefault(Func<T, bool> expression)
        {
            var result = _Context.Set<T>().LastOrDefault(expression);
            OnValidateLastOrDefault(result);
            return result;
        }


        public virtual IQueryable<T> Where(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            var result = _Context.Set<T>().Where(expression);
            OnValidateWhere(result);
            return result;
        }

        public virtual List<T> ToList()
        {
            var result = _Context.Set<T>().ToList();
            OnValidateToList(result);
            return result;
        }



        public virtual void MarkDirty(T entity)
        {
            OnMarkingDirty(entity);
            _Context.Entry(entity).State = System.Data.EntityState.Modified;
            OnMarkedDirty(entity);
        }
        protected virtual bool IsPersistent(T entity)
        {
            return entity.Id != 0;
        }


        public virtual void Update(T entity)
        {
            OnUpdating(entity);
            entity.Created = DateTime.Now;
            this._Context.Set<T>().Attach(entity);
            this._Context.Entry<T>(entity).State = System.Data.EntityState.Modified;
            SubmitChanges();
            OnUpdated(entity);
        }
        public virtual void Insert(T entity)
        {
            OnInserting(entity);
            entity.Created = DateTime.Now;
            entity.Modified = DateTime.Now;
            entity.Id = 0;
            this._Context.Set<T>().Add(entity);
            OnInserted(entity);
        }
        public virtual void InsertOrUpdateOnSubmit(T entity)
        {
            if (IsPersistent(entity))
            {
                OnUpdating(entity);

                entity.Created = DateTime.Now;
                entity.Modified = DateTime.Now;
                var existing = this._Context.Set<T>().FirstOrDefault(x => x.Id == entity.Id);
                if (existing != null)
                {
                    this.Entry(existing).CurrentValues.SetValues(entity);
                    OnUpdated(entity);
                }
            }
            else
            {
                OnInserting(entity);
                entity.Created = DateTime.Now;
                entity.Id = 0;
                this._Context.Set<T>().Add(entity);
                OnInserted(entity);
            }
        }

        public virtual void RemoveOnSubmit(T entity)
        {
            OnRemoving(entity);
            this._Context.Set<T>().Attach(entity);
            _Context.Set<T>().Remove(entity);
            OnRemoved(entity);
        }

        public virtual bool SubmitChanges()
        {
            OnSubmitingChanges();

            foreach (var item in _Context.ChangeTracker.Entries<T>())
            {
                item.Entity.Modified = DateTime.Now;
            }

            var result = _Context.SaveChanges() > 0;
            OnSubmitedChanges();
            return result;
        }



        public virtual System.Data.Entity.Infrastructure.DbEntityEntry Entry(object entity)
        {
            var result = _Context.Entry(entity);
            OnValidateEntry(result);
            return result;
        }

        public virtual System.Data.Entity.Infrastructure.DbEntityEntry<T> Entry(T entity)
        {
            var result = _Context.Entry(entity);
            OnValidateTypedEntry(result);
            return result;
        }



        public virtual void Dispose()
        {
            _Context.Dispose();
        }




        #region Extensibility Methods
        protected virtual void OnContextChanging(DbContext newcontext) { }
        protected virtual void OnContextChanged(DbContext oldcontext) { }

        protected virtual void OnValidateSingle(T entity) { }
        protected virtual void OnValidateSingleOrDefault(T entity) { }

        protected virtual void OnValidateFirst(T entity) { }
        protected virtual void OnValidateFirstOrDefault(T entity) { }

        protected virtual void OnValidateLast(T entity) { }
        protected virtual void OnValidateLastOrDefault(T entity) { }

        protected virtual void OnValidateWhere(IQueryable<T> entities) { }
        protected virtual void OnValidateToList(List<T> entities) { }

        protected virtual void OnValidateEntry(object entity) { }
        protected virtual void OnValidateTypedEntry(System.Data.Entity.Infrastructure.DbEntityEntry<T> entity) { }

        protected virtual void OnMarkingDirty(T entity) { }
        protected virtual void OnMarkedDirty(T entity) { }

        protected virtual void OnInserting(T entity) { }
        protected virtual void OnInserted(T entity) { }

        protected virtual void OnUpdating(T entity) { }
        protected virtual void OnUpdated(T entity) { }

        protected virtual void OnRemoving(T entity) { }
        protected virtual void OnRemoved(T entity) { }

        protected virtual void OnSubmitingChanges() { }
        protected virtual void OnSubmitedChanges() { }
        #endregion




        public bool RollBack()
        {
            throw new NotImplementedException();
        }


        public IEnumerable<T> NativeQuery(string queryString, IDictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> NativeQuery(string queryString, object parameters = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> NamedQuery(string queryString, IDictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> NamedQuery(string queryString, object parameters = null)
        {
            throw new NotImplementedException();
        }
    }

}
