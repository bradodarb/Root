using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate;
using FluentNHibernate.Conventions;

namespace DAL.Root.NHibernate.Conventions
{
    public class ForeignKeyNameConvention : ForeignKeyConvention
    {
        protected override string GetKeyName(Member property, Type type)
        {
            if (property == null)
                return type.Name + "Id";

            return property.Name + "Id";
        }
    }
}

