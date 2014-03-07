using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FluentNHibernate;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using Util.Root.Attributes;

namespace DAL.Root.NHibernate.Conventions
{
    public class DbStringLengthConvention : IPropertyConvention, IPropertyConventionAcceptance
    {
        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria) { criteria.Expect(x => x.Type == typeof(string)).Expect(x => x.Length == 0); }
     
        public void Apply(IPropertyInstance instance)
        {
            int leng = 255;

            MemberInfo[] myMemberInfos = ((PropertyInstance)(instance)).EntityType.GetMember(instance.Name);
            if (myMemberInfos.Length > 0)
            {
                object[] customAttrs = myMemberInfos[0].GetCustomAttributes(false);
                if (customAttrs.Length > 0)
                {
                    for (int i = 0; i < customAttrs.Length; i++)
                    {
                        if (customAttrs[i] is DbStringLength)
                        {
                            leng = ((DbStringLength)(customAttrs[i])).Length;
                            break;
                        }
                    }
               
                }
            }

            instance.Length(leng);

        }
    } 
}

