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
using BLL.Root;
using Util.Root;
using BLL.Root.Dto;
using Model.Root;


namespace BLL.Root.Admin
{

    public abstract class AdminServiceBase<DTO, M> : AccessorServiceBase<DTO, M>, ICompositeService<DTO>, IAutoCompleteProvider
        where DTO : DtoBase
        where M : class, IPersistenceEntity
    { 
        #region Fields
        protected string _User = "";
        #endregion

        #region Properties
        public string User
        {
            get
            {
                return _User;
            }
            set
            {
                _User = value;
            }
        }
        #endregion

        public AdminServiceBase(string user = "Anonymous")
        {
            User = user;
            Repository = AdminServiceProvider.GetRepository<M>();
        }

        #region IAdmin Members
        public abstract DTO New(DTO modelstart);

        public virtual bool Update(DTO source, out int id)
        {
            bool result = false;
            bool newitem = source.Id == 0;

            var model = Revert(source);
            model.ModifiedByUserName = _User;

            Repository.InsertOrUpdateOnSubmit(model);

            result = Repository.SubmitChanges();
            id = model.Id;

            if (newitem)
            {
                OnItemCreated(Convert(model));
            }
            else
            {
                OnItemUpdated(Convert(model));
            }

            return result;
        }

        public virtual bool Patch(dynamic source, int id)
        {
            bool result = false;
            var model = Repository.GetById(id);

            if (model != null)
            {

                Util.Root.DynamicMapper.MapDynamic<DTO>(source);

                Repository.InsertOrUpdateOnSubmit(model);

                OnItemUpdated(Convert(model));

                result = Repository.SubmitChanges();

                return result;
            }
            else
            {
                throw new Exception(String.Format("Item not found with Id = {0}", id));
            }
        }

        public virtual bool Remove(int id)
        {

            var model = Repository.GetById(id);

            if (model != null)
            {
                Repository.RemoveOnSubmit(model);

                OnItemDeleted(Convert(model));

                return Repository.SubmitChanges();

            }
            else
            {
                throw new Exception(String.Format("Item not found with Id = {0}", id));
            }

        }

        public virtual bool Validate(DTO dto, List<ValidationInfo> info)
        {
            //create a new List
            //if error add to the list and return false
            try 
            {
                ValidationManager.ValidateProperties(dto, info);

                if (info.Count > 0)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                throw new InvalidOperationException("Unable to validate");
            }
            
            
        } 
        #endregion

        #region Dispatch Methods
        /*
         * 
         *    The following methods should be used to fire the static events
         *                 of each concrete implementation
         * 
         */
        protected abstract void OnItemCreated(DTO item);
        protected abstract void OnItemDeleted(DTO item);
        protected abstract void OnItemUpdated(DTO item);
        #endregion

    }
}
 
 