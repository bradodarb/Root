using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Mapping;
using Model.Root;
using Util.Root;

namespace DAL.Root.NHibernate
{
    public class PersistentClassMap<T> : ClassMap<T> where T : IPersistenceEntity
    {

        public PersistentClassMap()
        {
            Id(x => x.Id);
            Map(x => x.IsActive);
            Map(x => x.Created);
            Map(x => x.Modified);
            Map(x => x.ModifiedByUserName); 
        }
    }
}
