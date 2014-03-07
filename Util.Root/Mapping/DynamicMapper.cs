﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;


namespace Util.Root
{

    public static class DynamicMapper
    {
        #region Constructor

        static DynamicMapper()
        {
            Configuration.ApplyDefaultIdentifierConventions();
        }

        #endregion Constructor

        #region Attributes

        /// <summary>
        /// Attribute for specifying that a field or property is an identifier.
        /// </summary>
        [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
        public class Id : Attribute { }

        #endregion Attributes

        #region Mapping

        /// <summary>
        /// Converts a list of dynamic property names and values to a list of type of <typeparamref name="T"/>.
        /// 
        /// Population of complex nested child properties is supported by underscoring "_" into the
        /// nested child properties in the property name.
        /// </summary>
        /// <typeparam name="T">Type to instantiate and automap to</typeparam>
        /// <param name="dynamicObject">Dynamic list of property names and values</param>
        /// <returns>List of type <typeparamref name="T"/></returns>
        public static T MapDynamic<T>(object dynamicObject)
        {
            var dictionary = dynamicObject as IDictionary<string, object>;

            if (dictionary == null)
                throw new ArgumentException("Object type cannot be converted to an IDictionary<string,object>", "dynamicObject");

            var propertiesList = new List<IDictionary<string, object>> { dictionary };

            return Map<T>(propertiesList).FirstOrDefault();
        }

        /// <summary>
        /// Converts a list of dynamic property names and values to a list of type of <typeparamref name="T"/>.
        /// 
        /// Population of complex nested child properties is supported by underscoring "_" into the
        /// nested child properties in the property name.
        /// </summary>
        /// <typeparam name="T">Type to instantiate and automap to</typeparam>
        /// <param name="dynamicListOfProperties">Dynamic list of property names and values</param>
        /// <returns>List of type <typeparamref name="T"/></returns>
        public static IEnumerable<T> MapDynamic<T>(IEnumerable<object> dynamicListOfProperties)
        {
            var dictionary = dynamicListOfProperties.Select(dynamicItem => dynamicItem as IDictionary<string, object>).ToList();

            if (dictionary == null || dictionary.Count == 0 || dictionary[0] == null)
                throw new ArgumentException("Object types cannot be converted to an IDictionary<string,object>", "dynamicListOfProperties");

            return Map<T>(dictionary);
        }

        /// <summary>
        /// Converts a list of property names and values to a list of type of <typeparamref name="T"/>.
        /// 
        /// Population of complex nested child properties is supported by underscoring "_" into the
        /// nested child properties in the property name.
        /// </summary>
        /// <typeparam name="T">Type to instantiate and automap to</typeparam>
        /// <param name="listOfProperties">List of property names and values</param>
        /// <returns>List of type <typeparamref name="T"/></returns>
        public static T Map<T>(IDictionary<string, object> listOfProperties)
        {
            var propertiesList = new List<IDictionary<string, object>> { listOfProperties };

            return Map<T>(propertiesList).FirstOrDefault();
        }

        /// <summary>
        /// Converts a list of property names and values to a list of type of <typeparamref name="T"/>.
        /// 
        /// Population of complex nested child properties is supported by underscoring "_" into the
        /// nested child properties in the property name.
        /// </summary>
        /// <typeparam name="T">Type to instantiate and automap to</typeparam>
        /// <param name="listOfProperties">List of property names and values</param>
        /// <returns>List of type <typeparamref name="T"/></returns>
        public static IEnumerable<T> Map<T>(IEnumerable<IDictionary<string, object>> listOfProperties)
        {
            var instanceCache = new Dictionary<object, object>();

            foreach (var properties in listOfProperties)
            {
                var getInstanceResult = InternalHelpers.GetInstance(typeof(T), properties);

                object instance = getInstanceResult.Item2;

                int instanceIdentifierHash = getInstanceResult.Item3;

                if (instanceCache.ContainsKey(instanceIdentifierHash) == false)
                {
                    instanceCache.Add(instanceIdentifierHash, instance);
                }

                var caseInsensitiveDictionary = new Dictionary<string, object>(properties, StringComparer.OrdinalIgnoreCase);

                InternalHelpers.Map(caseInsensitiveDictionary, instance);
            }

            foreach (var pair in instanceCache)
            {
                yield return (T)pair.Value;
            }
        }

        #endregion Mapping

        #region Cache

        public static class Cache
        {
            /// <summary>
            /// The name of the instance cache stored in the logical call context.
            /// </summary>
            public const string InstanceCacheContextStorageKey = "SlapperAutoMapper.InstanceCache";

            /// <summary>
            /// Cache of TypeMaps containing the types identifiers and PropertyInfo/FieldInfo objects.
            /// </summary>
            public static readonly ConcurrentDictionary<Type, TypeMap> TypeMapCache = new ConcurrentDictionary<Type, TypeMap>();

            /// <summary>
            /// A TypeMap holds data relevant for a particular Type.
            /// </summary>
            public class TypeMap
            {
                public TypeMap(Type type, IEnumerable<string> identifiers, Dictionary<string, object> propertiesAndFields)
                {
                    Type = type;
                    Identifiers = identifiers;
                    PropertiesAndFieldsInfo = propertiesAndFields;
                }

                /// <summary>
                /// Type for this TypeMap
                /// </summary>
                public readonly Type Type;

                /// <summary>
                /// List of identifiers
                /// </summary>
                public IEnumerable<string> Identifiers;

                /// <summary>
                /// Property/field names and their corresponding PropertyInfo/FieldInfo objects
                /// </summary>
                public Dictionary<string, object> PropertiesAndFieldsInfo;
            }

            /// <summary>
            /// Clears all internal caches.
            /// </summary>
            public static void ClearAllCaches()
            {
                TypeMapCache.Clear();
                ClearInstanceCache();
            }

            /// <summary>
            /// Clears the instance cache. This cache contains all objects created by Slapper.AutoMapper.
            /// </summary>
            public static void ClearInstanceCache()
            {
                var instanceCache = CallContext.LogicalGetData(InstanceCacheContextStorageKey) as Dictionary<object, object>;

                instanceCache = null;

                CallContext.FreeNamedDataSlot(InstanceCacheContextStorageKey);
            }

            /// <summary>
            /// Gets the instance cache containing all objects created by Slapper.AutoMapper.
            /// This cache exists for the lifetime of the current thread until manually cleared/purged.
            /// </summary>
            /// <remarks>
            /// Due to the nature of how the cache is persisted, each new thread will recieve it's own
            /// unique cache.
            /// </remarks>
            /// <returns>Instance Cache</returns>
            public static Dictionary<object, object> GetInstanceCache()
            {
                var instanceCache = CallContext.LogicalGetData(InstanceCacheContextStorageKey) as Dictionary<object, object>;

                if (instanceCache == null)
                {
                    instanceCache = new Dictionary<object, object>();

                    CallContext.LogicalSetData(Cache.InstanceCacheContextStorageKey, instanceCache);
                }

                return instanceCache;
            }
        }

        #endregion Cache

        #region Configuration

        public static class Configuration
        {
            static Configuration()
            {
                Configuration.IdentifierAttributeType = typeof(DynamicMapper.Id);
            }

            /// <summary>
            /// The attribute Type specifying that a field or property is an identifier.
            /// </summary>
            public static Type IdentifierAttributeType;

            /// <summary>
            /// Convention for finding an identifier.
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public delegate string ApplyIdentifierConvention(Type type);

            /// <summary>
            /// Conventions for finding an identifier.
            /// </summary>
            public static readonly List<ApplyIdentifierConvention> IdentifierConventions = new List<ApplyIdentifierConvention>();

            /// <summary>
            /// Applies default conventions for finding identifiers
            /// </summary>
            public static void ApplyDefaultIdentifierConventions()
            {
                IdentifierConventions.Add(type => "Id");
                IdentifierConventions.Add(type => type.Name + "Id");
                IdentifierConventions.Add(type => type.Name + "Nbr");
            }

            /// <summary>
            /// Adds an identifier for the specified type.
            /// Replaces any identifiers previously specified.
            /// </summary>
            /// <param name="type">Type</param>
            /// <param name="identifier">Identifier</param>
            public static void AddIdentifier(Type type, string identifier)
            {
                AddIdentifiers(type, new List<string> { identifier });
            }

            /// <summary>
            /// Adds identifiers for the specified type.
            /// Replaces any identifiers previously specified.
            /// </summary>
            /// <param name="type">Type</param>
            /// <param name="identifiers">Identifiers</param>
            public static void AddIdentifiers(Type type, IEnumerable<string> identifiers)
            {
                var typeMap = Cache.TypeMapCache.GetOrAdd(type, InternalHelpers.CreateTypeMap(type));

                typeMap.Identifiers = identifiers;
            }
        }

        #endregion Configuration

        #region Internal Helpers

        public static class InternalHelpers
        {
            /// <summary>
            /// Gets the identifiers for the given type. Returns NULL if not found.
            /// Results are cached for subsequent use and performance.
            /// </summary>
            /// <remarks>
            /// If no identifiers have been manually added, this method will attempt
            /// to first find an <see cref="Slapper.AutoMapper.Id"/> attribute on the <paramref name="type"/>
            /// and if not found will then try to match based upon any specified identifier conventions.
            /// </remarks>
            /// <param name="type">Type</param>
            /// <returns>Identifier</returns>
            public static IEnumerable<string> GetIdentifiers(Type type)
            {
                var typeMap = Cache.TypeMapCache.GetOrAdd(type, CreateTypeMap(type));

                return typeMap.Identifiers.Count() == 0 ? null : typeMap.Identifiers;
            }

            /// <summary>
            /// Get a Dictionary of a type's property names and field names and their corresponding PropertyInfo or FieldInfo.
            /// Results are cached for subsequent use and performance.
            /// </summary>
            /// <param name="type">Type</param>
            /// <returns>Dictionary of a type's property names and their corresponding PropertyInfo</returns>
            public static Dictionary<string, object> GetFieldsAndProperties(Type type)
            {
                var typeMap = Cache.TypeMapCache.GetOrAdd(type, CreateTypeMap(type));

                return typeMap.PropertiesAndFieldsInfo;
            }

            /// <summary>
            /// Creates a TypeMap for a given Type.
            /// </summary>
            /// <param name="type">Type</param>
            /// <returns>TypeMap</returns>
            public static Cache.TypeMap CreateTypeMap(Type type)
            {
                var conventionIdentifiers = Configuration.IdentifierConventions.Select(applyIdentifierConvention => applyIdentifierConvention(type)).ToList();

                var fieldsAndProperties = CreateFieldAndPropertyInfoDictionary(type);

                var identifiers = new List<string>();

                foreach (var fieldOrProperty in fieldsAndProperties)
                {
                    var memberName = fieldOrProperty.Key;

                    var member = fieldOrProperty.Value;

                    if (member is FieldInfo)
                    {
                        if (((FieldInfo)member).GetCustomAttributes(Configuration.IdentifierAttributeType, false).Length > 0)
                        {
                            identifiers.Add(memberName);
                        }
                        else if (conventionIdentifiers.Exists(x => x.ToLower() == memberName.ToLower()))
                        {
                            identifiers.Add(memberName);
                        }
                    }
                    else if (member is PropertyInfo)
                    {
                        if (((PropertyInfo)member).GetCustomAttributes(Configuration.IdentifierAttributeType, false).Length > 0)
                        {
                            identifiers.Add(memberName);
                        }
                        else if (conventionIdentifiers.Exists(x => x.ToLower() == memberName.ToLower()))
                        {
                            identifiers.Add(memberName);
                        }
                    }
                }

                var typeMap = new Cache.TypeMap(type, identifiers, fieldsAndProperties);

                return typeMap;
            }

            /// <summary>
            /// Creates a Dictionary of field or property names and their corresponding FieldInfo or PropertyInfo objects
            /// </summary>
            /// <param name="type">Type</param>
            /// <returns>Dictionary of member names and member info objects</returns>
            public static Dictionary<string, object> CreateFieldAndPropertyInfoDictionary(Type type)
            {
                var dictionary = new Dictionary<string, object>();

                var properties = type.GetProperties();

                foreach (var propertyInfo in properties)
                {
                    dictionary.Add(propertyInfo.Name, propertyInfo);
                }

                var fields = type.GetFields();

                foreach (var fieldInfo in fields)
                {
                    dictionary.Add(fieldInfo.Name, fieldInfo);
                }

                return dictionary;
            }

            /// <summary>
            /// Gets the Type of the Field or Property
            /// </summary>
            /// <param name="member">FieldInfo or PropertyInfo object</param>
            /// <returns>Type</returns>
            public static Type GetMemberType(object member)
            {
                Type type = null;

                if (member is FieldInfo)
                {
                    type = ((FieldInfo)member).FieldType;
                }
                else if (member is PropertyInfo)
                {
                    type = ((PropertyInfo)member).PropertyType;
                }

                return type;
            }

            /// <summary>
            /// Sets the value on a Field or Property
            /// </summary>
            /// <param name="member">FieldInfo or PropertyInfo object</param>
            /// <param name="obj">Object to set the value on</param>
            /// <param name="value">Value</param>
            public static void SetMemberValue(object member, object obj, object value)
            {
                if (member is FieldInfo)
                {
                    ((FieldInfo)member).SetValue(obj, value);
                }
                else if (member is PropertyInfo)
                {
                    ((PropertyInfo)member).SetValue(obj, value, null);
                }
            }

            /// <summary>
            /// Gets the value of the member
            /// </summary>
            /// <param name="member">FieldInfo or PropertyInfo object</param>
            /// <param name="obj">Object to get the value from</param>
            /// <returns>Value of the member</returns>
            public static object GetMemberValue(object member, object obj)
            {
                object value = null;

                if (member is FieldInfo)
                {
                    value = ((FieldInfo)member).GetValue(obj);
                }
                else if (member is PropertyInfo)
                {
                    value = ((PropertyInfo)member).GetValue(obj, null);
                }

                return value;
            }

            /// <summary>
            /// Gets a new or existing instance depending on whether an instance with the same identifiers already existing
            /// in the instance cache.
            /// </summary>
            /// <param name="type">Type of instance to get</param>
            /// <param name="properties">List of properties and values</param>
            /// <returns>
            /// Tuple of bool, object, int where bool represents whether this is a newly created instance,
            /// object being an instance of the requested type and int being the instance's identifier hash.
            /// </returns>
            public static Tuple<bool, object, int> GetInstance(Type type, IDictionary<string, object> properties)
            {
                var instanceCache = Cache.GetInstanceCache();

                var identifiers = GetIdentifiers(type);

                object instance = null;
                bool isNewlyCreatedInstance = false;

                int identifierHash = 0;

                if (identifiers != null)
                {
                    foreach (var identifier in identifiers)
                    {
                        if (properties.ContainsKey(identifier))
                        {
                            var identifierValue = properties[identifier];

                            identifierHash += identifierValue.GetHashCode() + type.GetHashCode();
                        }
                    }

                    if (identifierHash != 0)
                    {
                        if (instanceCache.ContainsKey(identifierHash))
                        {
                            instance = instanceCache[identifierHash];
                        }
                        else
                        {
                            instance = Activator.CreateInstance(type);

                            instanceCache.Add(identifierHash, instance);

                            isNewlyCreatedInstance = true;
                        }
                    }
                }

                if (instance == null)
                {
                    instance = Activator.CreateInstance(type);

                    identifierHash = Guid.NewGuid().GetHashCode();

                    instanceCache.Add(identifierHash, instance);

                    isNewlyCreatedInstance = true;
                }

                return new Tuple<bool, object, int>(isNewlyCreatedInstance, instance, identifierHash);
            }

            /// <summary>
            /// Populates the given instance's properties where the IDictionary key property names
            /// match the type's property names case insensitively.
            /// 
            /// Population of complex nested child properties is supported by underscoring "_" into the
            /// nested child properties in the property name.
            /// </summary>
            /// <param name="dictionary">Dictionary of property names and values</param>
            /// <param name="instance">Instance to populate</param>
            /// <param name="parentInstance">Optional parent instance of the instance being populated</param>
            /// <returns>Populated instance</returns>
            public static object Map(IDictionary<string, object> dictionary, object instance, object parentInstance = null)
            {
                var fieldsAndProperties = InternalHelpers.GetFieldsAndProperties(instance.GetType());

                foreach (var fieldOrProperty in fieldsAndProperties)
                {
                    var memberName = fieldOrProperty.Key.ToLower();

                    var member = fieldOrProperty.Value;

                    object value;

                    if (dictionary.TryGetValue(memberName, out value))
                    {
                        InternalHelpers.SetMemberValue(member, instance, value);
                    }
                    else
                    {
                        Type propertyType = InternalHelpers.GetMemberType(member);

                        if (propertyType.IsClass || propertyType.IsInterface)
                        {
                            var nestedDictionary = dictionary.Where(x => x.Key.ToLower().StartsWith(memberName + "_"));

                            if (nestedDictionary.Count() == 0)
                            {
                                if (parentInstance != null)
                                {
                                    if (parentInstance.GetType() == propertyType)
                                    {
                                        InternalHelpers.SetMemberValue(member, instance, parentInstance);
                                    }
                                }

                                continue;
                            }

                            var newDictionary = nestedDictionary.ToDictionary(pair => pair.Key.ToLower().Replace(memberName + "_", string.Empty), pair => pair.Value, StringComparer.OrdinalIgnoreCase);

                            object nestedInstance = InternalHelpers.GetMemberValue(member, instance);

                            if (nestedInstance == null && propertyType.IsClass)
                            {
                                nestedInstance = Activator.CreateInstance(propertyType);
                            }

                            Type genericCollectionType = typeof(IEnumerable<>);

                            if (propertyType.IsGenericType && genericCollectionType.IsAssignableFrom(propertyType.GetGenericTypeDefinition())
                                 || propertyType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == genericCollectionType))
                            {
                                var innerType = propertyType.GetGenericArguments().First();

                                Type baseListType = typeof(List<>);

                                Type listType = baseListType.MakeGenericType(innerType);

                                if (nestedInstance == null)
                                {
                                    nestedInstance = Activator.CreateInstance(listType);
                                }

                                var getInstanceResult = InternalHelpers.GetInstance(innerType, newDictionary);

                                // Is this a newly created instance? If false, then this item was retrieved from the instance cache.
                                bool isNewlyCreatedInstance = getInstanceResult.Item1;

                                object instanceToAddToCollectionInstance = getInstanceResult.Item2;

                                instanceToAddToCollectionInstance = Map(newDictionary, instanceToAddToCollectionInstance, instance);

                                if (isNewlyCreatedInstance)
                                {
                                    MethodInfo addMethod = listType.GetMethod("Add");

                                    addMethod.Invoke(nestedInstance, new[] { instanceToAddToCollectionInstance });
                                }
                                else
                                {
                                    MethodInfo containsMethod = listType.GetMethod("Contains");

                                    bool alreadyContainsInstance = (bool)containsMethod.Invoke(nestedInstance, new[] { instanceToAddToCollectionInstance });

                                    if (alreadyContainsInstance == false)
                                    {
                                        MethodInfo addMethod = listType.GetMethod("Add");

                                        addMethod.Invoke(nestedInstance, new[] { instanceToAddToCollectionInstance });
                                    }
                                }
                            }
                            else
                            {
                                nestedInstance = Map(newDictionary, nestedInstance, instance);
                            }

                            InternalHelpers.SetMemberValue(member, instance, nestedInstance);
                        }
                    }
                }

                return instance;
            }
        }

        #endregion Internal Helpers
    }
}