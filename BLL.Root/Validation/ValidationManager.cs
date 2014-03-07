using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Util.Root;

namespace BLL.Root.Validation
{
    public static class ValidationManager
    {

        /// <summary>
        /// Utilty method for validating the properties of a class based on attributes that are derived from ValidationAttribute
        /// </summary>
        /// <param name="target">Object to validate</param>
        /// <param name="validationinfo">List of Validation Information to append to</param>
        /// <param name="ignoredprops">Optional list of Property Names to ignore</param>
        /// <returns>true if all properties passed validation, otherwise false</returns>
        public static bool ValidateProperties(object target, List<ValidationInfo> validationinfo, params string[] ignoredprops)
        {
            bool result = true;

            if (target != null)
            {
                List<string> ignored = new List<string>();
                if (ignoredprops != null)
                {
                    ignored = ignoredprops.ToList();
                }

                Type type = target.GetType();
                var properties = type.GetProperties().Where(x => !ignored.Contains(x.Name)).ToList();

                foreach (PropertyInfo property in properties)
                {
                    
                    if (!ValidateProperty(target, property, validationinfo))
                    {
                        result = false;
                    }

                }


            }
            else
            {
                Validate(false, "Item", "Cannot validate missing property", "Root Object", ValidationInfoType.Class, validationinfo);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Utility method for validating a single property
        /// </summary>
        /// <param name="target">Object to validate</param>
        /// <param name="property">Property info object</param>
        /// <param name="validationinfo">List of Validation Information to append to</param>
        /// <returns>true if property passed validation, otherwise false</returns>
        public static bool ValidateProperty(object target, PropertyInfo property, List<ValidationInfo> validationinfo, string root = "")
        {
            bool result = false;

            var propval = property.GetValue(target, null);
             
            object[] attributes = property.GetCustomAttributes(typeof(ValidationAttribute), true);

            List<ValidationInfo> validationresult = new List<ValidationInfo>();


            foreach (object attribute in attributes)
            {
                var attrib = attribute as ValidationAttribute;
                if (attrib != null)
                { 
                    if (!(attribute as ValidationAttribute).IsValid(propval))
                    {
                        object[] datamemberattributes = property.GetCustomAttributes(typeof(DescriptionAttribute), true);
                        var name = property.Name;
                        if (datamemberattributes != null && datamemberattributes.Length > 0)
                        {
                            var datamember = datamemberattributes.LastOrDefault() as DescriptionAttribute;
                            if (datamember != null)
                            {
                                if (datamember.Description != null && datamember.Description != String.Empty)
                                {
                                    name = datamember.Description;
                                }
                            }
                        }
                        name = (root == "") ? name : String.Format("{0}:{1}", root, name);
                        validationresult.Add(new ValidationInfo(ValidationInfoType.Property,
                        name, attrib.ErrorMessage, attrib.ToString(), false)); 
                    }
                }
            }


            if (validationresult.Count > 0)
            {
                validationinfo.AddRange(validationresult);
                result = false;
            }
            else
            {
                result = true;
            }


            return result;
        }

        /// <summary>
        /// Utility method for validating a single property
        /// </summary>
        /// <param name="target">Object to validate</param>
        /// <param name="property">Property name to discover and validate</param>
        /// <param name="validationinfo">List of Validation Information to append to</param>
        /// <returns>true if property passed validation, otherwise false</returns>
        public static bool ValidateProperty(object target, string property, 
            List<ValidationInfo> validationinfo, string root = "")
        {
            if (target != null)
            {
                Type type = target.GetType();
                var propinfo = type.GetProperty(property);

                if (propinfo != null)
                {
                    return ValidateProperty(target, propinfo, validationinfo, root);
                }
                else
                {
                    throw new InvalidOperationException(String.Format("Could not find Property {0} on type of {1}", property, type.Name));
                }
            }
            else
            {
                throw new NullReferenceException("parameter target cannot be null");
            }
        }

        /// <summary>
        /// Utility method for validating a single property
        /// </summary>
        /// <param name="target">Object to validate</param>
        /// <param name="property">Property info object</param>
        /// <param name="validationinfo">List of Validation Information to append to</param>
        /// <returns>true if property passed validation, otherwise false</returns>
        public static bool Validate(object target, PropertyInfo property, List<ValidationInfo> validationinfo, string root = "")
        {
            bool result = false;

            var propval = property.GetValue(target, null);

            object[] attributes = property.GetCustomAttributes(typeof(ValidationAttribute), true);

            List<ValidationInfo> validationresult = new List<ValidationInfo>();


            foreach (object attribute in attributes)
            {
                var attrib = attribute as ValidationAttribute;
                if (attrib != null)
                {
                    if (!(attribute as ValidationAttribute).IsValid(propval))
                    {
                        object[] datamemberattributes = property.GetCustomAttributes(typeof(DescriptionAttribute), true);
                        var name = property.Name;
                        if (datamemberattributes != null && datamemberattributes.Length > 0)
                        {
                            var datamember = datamemberattributes.LastOrDefault() as DescriptionAttribute;
                            if (datamember != null)
                            {
                                if (datamember.Description != null && datamember.Description != String.Empty)
                                {
                                    name = datamember.Description;
                                }
                            }
                        }
                        name = (root == "") ? name : String.Format("{0}:{1}", root, name);
                        validationresult.Add(new ValidationInfo(ValidationInfoType.Property,
                        name, attrib.ErrorMessage, attrib.ToString(), false));
                    }
                }
            }


            if (validationresult.Count > 0)
            {
                validationinfo.AddRange(validationresult);
                result = false;
            }
            else
            {
                result = true;
            }


            return result;
        }


        public static ValidationInfo Validate(bool expression, string name, 
            string errormessage, string source, ValidationInfoType valtype, string root = "",  bool faulted = false)
        {
            if (!expression)
            {
                name = (root == "") ? name : String.Format("{0}:{1}", root, name);
                return new ValidationInfo(valtype,
                             name, errormessage, source, faulted);
            }
            return null;
        }


        public static void Validate(bool expression, string name,
            string errormessage, string source, ValidationInfoType valtype, List<ValidationInfo> validationinfo,
            string root = "", bool faulted = false)
        {
            if (!expression)
            {
                name = (root == "") ? name : String.Format("{0}:{1}", root, name);
                validationinfo.Add(new ValidationInfo(valtype,
                             name, errormessage, source, faulted));
            }
        }



        public struct ValidationSource
        {
            public const string BusinessRule = "Business Rule";
        }
    }
}
