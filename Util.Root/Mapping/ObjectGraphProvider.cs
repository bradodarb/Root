using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Util.Root.Mapping
{
    /// <summary>
    /// Utility to create a reduced object graph from a given object
    /// </summary>
    public static class ObjectGraphProvider
    {

        /// <summary>
        /// Used to take an object and creates a reduced object graph (e.g. for smaller payloads across the wire)
        /// </summary>
        /// <param name="target">source object to create graph from</param>
        /// <param name="properties">property names to generate new graph from</param>
        /// <returns>Dynamic ExpandoObject reperesenting the desired object graph</returns>
        public static object ObjectGraph(object target, params string[] properties)
        {

            var result = new ExpandoObject();
            var dict = result as IDictionary<string, object>;
            var targettype = target.GetType();

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Contains('.'))
                {
                    string compoundProperty = properties[i];
                    string[] bits = compoundProperty.Split('.');
                    object val = target;
                    var root = dict;
                    for (int j = 0; j < bits.Length; j++)
                    {
						if (val != null)
						{
							PropertyInfo propertyToGet = val.GetType().GetProperty(bits[j]);
							val = propertyToGet.GetValue(val, null);
							if (j == bits.Length - 1)
							{
								//set final value
								if (!root.ContainsKey(bits[j]))
								{
									root.Add(bits[j], val);
								}
								else
								{
									root[bits[j]] = val;
								}
							}
							else
							{
								//check if expando exists, or create new one
								if (!root.ContainsKey(bits[j]))
								{
									root.Add(bits[j], new ExpandoObject() as IDictionary<string, object>);
								}
								root = root[bits[j]] as IDictionary<string, object>;
							}
						}
						else
						{
							//set final value
							if (!root.ContainsKey(bits[j]))
							{
								root.Add(bits[j], val);
							}
							else
							{
								root[bits[j]] = val;
							}
						}
                    }
                    
                }
                else
                {
                    PropertyInfo propertyToGet = targettype.GetProperty(properties[i]);
                    var val = propertyToGet.GetValue(target, null);
                    if (!dict.ContainsKey(properties[i]))
                    {
                        dict.Add(properties[i], val);
                    }
                    else
                    {
                        dict[properties[i]] = val;
                    }
                }
            }


            return result;
        }
        
        /// <summary>
        /// Used to take a list of objects and creates a reduced object graph (e.g. for smaller payloads across the wire)
        /// </summary>
        /// <param name="target">source object to create graph from</param>
        /// <param name="properties">property names to generate new graph from</param>
        /// <returns>List of Dynamic ExpandoObject reperesenting the desired object graph</returns>
        public static List<object> ObjectGraph(List<object> target, params string[] properties)
        {
            List<object> result = new List<object>();

            for (int i = 0; i < target.Count; i++)
            {
                var item = ObjectGraph(target[i], properties);
                result.Add(item);
            }

            return result;
        }

        public static object Filter(object target, bool recursive, params string[] properties)
        {
            var result = target as ExpandoObject;
            if (result == null || properties == null || properties.Length < 1)
            {
                return target;
            }

            var dict = result as IDictionary<string, object>;
            var targettype = target.GetType();
            var exclusions = properties.ToList();
            var keys = dict.Keys.ToList();

            for (int i = 0; i < keys.Count; i++)
            {
                var key = keys[i];
                if (exclusions.Contains(key))
                {
                    if (dict.ContainsKey(key))
                    {
                        dict.Remove(key);
                    }
                }
                else if(recursive == true)
                {
                    if (dict.ContainsKey(key))
                    {
                        dict[key] = Filter(dict[key], recursive, properties);
                    }
                }
            }

            return result;
        }
    }
}
