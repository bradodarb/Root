                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using System.Text;
using PagedList;
using DAL.Root;
using BLL.Root.Validation;
using Util.Root.Common;
using BLL.Root.Admin;
using Util.Root;
using BLL.Root.Dto;
using Util.Root.Mapping;
using System.Diagnostics;
using Model.Root;


namespace BLL.Root.Admin
{

    public abstract class AccessorServiceBase<DTO, M> : IAccessorService<DTO>, IAutoCompleteProvider where DTO : DtoBase where M : class, IPersistenceEntity
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Fields 
        protected  IRepository<M> _Repository;
        #endregion

        #region Properties
        public IRepository<M> Repository
        {
            get
            {
                return _Repository;
            }
            set
            {
                _Repository = value;
            }
        } 
        #endregion

        public AccessorServiceBase()
        {
            Repository = AdminServiceProvider.GetRepository<M>(); 
        }

        #region IAdmin Members
        public virtual DTO Get(int id)
        {
            var model = Repository.GetById(id);

            if (model != null)
            {
                return Convert(model);
            }
            else
            {
                throw new Exception(String.Format("Center not found with Id = {0}", id));
            }
        }

        public virtual DTO Get(string expression, params object[] parameters)
        {
            var model = Repository.Entities.Where(expression, parameters).FirstOrDefault();
            if (model != null)
            {
                return Convert(model);
            }
            else
            {
                throw new Exception("Item not found using the provided expression");
            }
        }

        public virtual List<DTO> GetMany(string expression, params object[] parameters)
        {
            List<DTO> result = new List<DTO>();
            var models = Repository.Entities.Where(expression, parameters);
            if (models != null)
            {
                foreach (var model in models)
                {
                    result.Add(Convert(model));
                }
            }
            else
            {
                throw new Exception("No items were found using the provided expression");
            }
            return result;
        }

        public virtual List<DTO> GetAll()
        {
            List<DTO> result = new List<DTO>();
            var models = Repository.ToList();
            if (models != null)
            {
                foreach (var model in models)
                {
                    result.Add(Convert(model));
                }
            }
            return result;
        }

        public virtual List<DTO> GetPage(int pagesize, int page, string sortfield, bool descending, string filter, out int totalcount, out bool hasprev, out bool hasnext, out string positiontext, string positiontextformat)
        {
            List<DTO> result = new List<DTO>();

            var query = PageSource(sortfield, descending, filter);

            if (query != null && query.Count() > 0)
            {
                var items = query.ToPagedList(page, pagesize);

                foreach (var item in items)
                {
                    result.Add(Convert(item));
                }
                totalcount = items.PageCount;
                hasprev = items.HasPreviousPage;
                hasnext = items.HasNextPage;
                var pagenum = items.PageNumber;
                var pagecount = items.PageCount;


                if (items.Count < 1)
                {
                    pagenum = pagecount = 0;
                }
                if (positiontextformat == null || positiontextformat == "")
                {
                    positiontextformat = "{0} of {1}";
                }
                positiontext = string.Format(positiontextformat, pagenum, pagecount);
            }
            else
            {
                totalcount = 0;
                hasprev = false;
                hasnext = false;
                positiontext = "no results";
            }
            return result;
        }

        protected virtual IQueryable<M> PageSource(string sortfield, bool descending, string filter)
        {
            IQueryable<M> query = null;
            var filterbody = GetFilterBody();
            if (!String.IsNullOrEmpty(filterbody) && !String.IsNullOrEmpty(filter) && filter != "null" && filter != "undefined")
            {
                filter = filter.Replace("'", "\"");
                var compiledfilter = GetCompiledFilter(filter);
                query = Repository.Entities.Where(filterbody, compiledfilter);
            }
            else if (!String.IsNullOrEmpty(filter) && filter != "null" && filter != "undefined")
			{
				filter = filter.Replace("'", "\"");
				query = Repository.Entities.Where(filter);
			}
			else
			{
				query = Repository.Entities;
			}


            if (descending)
            {
                query = query.OrderBy(sortfield + " descending");
            }else
            {
                query = query.OrderBy(sortfield);
            }

            return query;
        }

        protected virtual string GetFilterBody()
        {
            return String.Empty;
        }
        protected virtual object GetCompiledFilter(string filter)
        {
            return filter;
        }
        #endregion

        #region IAutoCompleteProvider Members
        public abstract object[] GetMatches(string target);
        #endregion

        #region Conversion Methods
        protected abstract DTO Convert(M model);
        protected abstract M Revert(DTO dto);
        #endregion

    }
}
