using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using DAL.Root;
using Model.Root;
using Util.Root;

namespace BLL.Root.Dto
{
    /// <summary>
    /// Common base for all DTO's
    /// </summary>
    [Serializable]
    [DataContract(Name = "dtobase")]
    public abstract class DtoBase : IPersistenceEntity
    {

        #region Fields
        protected int _Id = 0;
        protected bool _IsActive = true;
        protected DateTime _Created = DateTime.Now;
        protected DateTime _Modified = DateTime.Now;
        protected string _ModifiedByUserName = ""; 
        #endregion
        
        #region Properties
        /// <summary>
        /// Db Primary key reference
        /// </summary>
        [DataMember(Name = "id")]
        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }

        /// <summary>
        /// Flag to determine if the model is 'active' i.e. relevant in the current context
        /// </summary>
        [DataMember(Name = "isActive")]
        public bool IsActive
        {
            get
            {
                return _IsActive;
            }
            set
            {
                _IsActive = value;
            }
        }

        /// <summary>
        /// Date when this model was created
        /// </summary>
        [DataMember(Name = "created")]
        public DateTime Created
        {
            get
            {
                return _Created;
            }
            set
            {
                _Created = value;
            }
        }

        /// <summary>
        /// Date when this model was last modified and saved to the database
        /// </summary>
        [DataMember(Name = "modified")]
        public DateTime Modified
        {
            get
            {
                return _Modified;
            }
            set
            {
                _Modified = value;
            }
        }

        /// <summary>
        /// Db Primary key reference
        /// </summary>
        [DataMember(Name = "modfiedUser")]
        public string ModifiedByUserName
        {
            get
            {
                return _ModifiedByUserName;
            }
            set
            {
                _ModifiedByUserName = value;
            }
        }
        #endregion
        
        #region Conversion Methods
        /// <summary>
        /// Central place to populate common Dto properties, such as those belonging to IPersistenceEntity
        /// </summary>
        /// <param name="target">Dto to transfer the standard values to</param>
        /// <param name="source">IPersistenceEntity to copy values from</param>
        public static void ConvertBase(DtoBase target, IPersistenceEntity source)
        {
			if (source != null)
			{
				target.Created = source.Created;
				target.Id = source.Id;
				target.IsActive = source.IsActive;
				target.Modified = source.Modified;
				target.ModifiedByUserName = source.ModifiedByUserName;
				target.Created = source.Created;
			}
        }


        /// <summary>
        /// Central place to populate common domain model properties, such as those belonging to IPersistenceEntity
        /// </summary>
        /// <param name="target">IPersistenceEntity to transfer the standard values to</param>
        /// <param name="source">Dto to copy values from</param>
        public static void RevertBase(IPersistenceEntity target, DtoBase source)
        {
			if (source != null)
			{
				target.Created = source.Created;
				target.Id = source.Id;
				target.IsActive = source.IsActive;
				target.Modified = source.Modified;
				if (!string.IsNullOrEmpty(source.ModifiedByUserName))
				{
					target.ModifiedByUserName = source.ModifiedByUserName;
				}
				else
				{
					target.ModifiedByUserName = AdminServiceProvider.GetCurrentUserName();
				}
				target.Created = source.Created;
			}
        } 
        #endregion

 
    }
}
