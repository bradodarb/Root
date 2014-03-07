using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Util.Root;
using Util.Root.Attributes;

namespace Model.Root
{
    public abstract class PersistenceEntityBase : IPersistenceEntity
    {

        #region IPersistenceEntity Members 
        public virtual int Id { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }
        public virtual string ModifiedByUserName { get; set; }
        #endregion

        public PersistenceEntityBase()
        {
            Created = DateTime.Now;
            Modified = DateTime.Now;
        }
    }
}
