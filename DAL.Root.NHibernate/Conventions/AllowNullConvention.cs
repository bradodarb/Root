using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using Util.Root.Attributes;

namespace DAL.Root.NHibernate.Conventions
{
    public class AllowNullConvention : IPropertyConvention
    {



        public void Apply(IPropertyInstance instance)
        {
            MemberInfo[] myMemberInfos = ((PropertyInstance)(instance)).EntityType.GetMember(instance.Name);
            if (myMemberInfos.Length > 0)
            {
                object[] customAttrs = myMemberInfos[0].GetCustomAttributes(false);
                if (customAttrs.Length > 0)
                {
                    for (int i = 0; i < customAttrs.Length; i++)
                    {
                        if (customAttrs[i] is AllowNull)
                        {

                            instance.Nullable();

                            break;
                        }
                    }

                }
            }


        }

    }
}
