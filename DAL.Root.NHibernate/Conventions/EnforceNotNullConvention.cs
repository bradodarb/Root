using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace DAL.Root.NHibernate.Conventions
{
    public class EnforceNotNullConvention : AllowNullConvention, IPropertyConvention
    {
    
        public new void Apply(IPropertyInstance instance)
        { 
            instance.Not.Nullable();
            base.Apply(instance);
        }
    }
}

