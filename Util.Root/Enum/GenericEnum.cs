/*.................................................................................  

  .................................................................................   */

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Sitewire.Util
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Enum<T>
        where T : struct
    {

        /* ``````````````````````````````````````````````````````````````` */
        /// <summary>
        /// Gets a value indicating whether this instance is enum.
        /// </summary>
        /// <value><c>true</c> if this instance is enum; otherwise, <c>false</c>.</value>
        public static bool IsEnum { get { return typeof(T).IsEnum; } }
        /* ``````````````````````````````````````````````````````````````` */
        /// <summary>
        /// Gets a value indicating whether this instance is flags.
        /// </summary>
        /// <value><c>true</c> if this instance is flags; otherwise, <c>false</c>.</value>
        public static bool IsFlags { get { return Attribute.IsDefined(typeof(T).Module, typeof(FlagsAttribute)); } }
        /* ``````````````````````````````````````````````````````````````` */
        /// <summary>
        /// Parses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T Parse(string value) { return (T)Enum.Parse(typeof(T), value); }
        /* ``````````````````````````````````````````````````````````````` */
        /// <summary>
        /// Parses the description.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T ParseDescription(string value)
        {
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                if (Enum<T>.DescriptionValue(item) == value) { return item; }
            }
            return default(T);
        }
        /* ``````````````````````````````````````````````````````````````` */
        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <returns></returns>
        public static IList<T> GetValues()
        {
            IList<T> list = new List<T>();
            foreach (object value in Enum.GetValues(typeof(T)))
            {
                list.Add((T)value);
            }
            return list;
        }
        /* ``````````````````````````````````````````````````````````````` */
        /// <summary>
        /// Gets the custome attributes.
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static A[] GetCustomeAttributes<A>(T value) where A : Attribute
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            return (A[])fieldInfo.GetCustomAttributes(typeof(A), false);

        }
        /* ``````````````````````````````````````````````````````````````` */
        /// <summary>
        /// Gets the custome attributes.
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static A GetSingleAttribute<A>(T value) where A : Attribute
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());
            A[] col = (A[])fieldInfo.GetCustomAttributes(typeof(A), false);

            return col[0];
        }
        /* ``````````````````````````````````````````````````````````````` */
        /// <summary>
        /// Descriptions the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string DescriptionValue(T value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0) { return attributes[0].Description; }
            else { return value.ToString(); }
        }
        /* ``````````````````````````````````````````````````````````````` */
        /// <summary>
        /// Enums the type description.
        /// </summary>
        /// <returns></returns>
        public static string EnumTypeDescription()
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])typeof(T).GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0) { return attributes[0].Description; }
            else { return typeof(T).ToString(); }
        }
        /* ``````````````````````````````````````````````````````````````` */
        /// <summary>
        /// Enums the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static int EnumValueAsInt(T value) { return Convert.ToInt32(Enum.Parse(typeof(T), value.ToString())); }

        /* ``````````````````````````````````````````````````````````````` */
        /// <summary>
        /// Enums the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static object EnumValue(T value) { return Enum.Parse(typeof(T), value.ToString()); }
        /* ``````````````````````````````````````````````````````````````` */
        /// <summary>
        /// Constructs a collection of selected enums out of 
        /// a multi-value enum (one using the FlagsAttrbute).
        /// <remarks>
        /// The method will do the following:
        /// 1. Check if T is an enum and throw an argument 
        ///    exception if it is not.
        /// 2. Iterate through the list of possible values 
        ///    of the enum and check it against the passed 
        ///    in value.
        /// 3. If a ‘None’ value exists and no matching 
        ///    enum is found then a check will be done 
        ///    to return the ‘None’ value.
        /// </remarks>
        /// </summary>
        /// <example>
        /// A typical enum used with this method.
        /// [Flags]
        /// enum RecurrenceDay
        /// {
        ///    None = 0,
        ///    Monday = 1,
        ///    Tuesday = 2,
        ///    Wednesday = 4,
        ///    Thursday = 8,
        ///    Friday = 16,
        ///    Saturday = 32,
        ///    Sunday = 64,
        ///    All = Monday | Tuesday | Wednesday | Thursday | Friday | 
        ///    Saturday | Sunday
        /// }
        /// </example>
        /// <typeparam name=”T”>Generic type of the enum.</typeparam>
        /// <param name=”enumInputValue”>The selected enums value.</param>
        /// <returns>A collection of selected enums.</returns>
        public static List<T> SelectedFlags<T>(T value)
        {

            if (!typeof(T).IsEnum) { throw new ArgumentException("Argument is is not an Enum"); }

            Array enumValues = Enum.GetValues(typeof(T));
            long inputValueLong = Convert.ToInt64(value);

            List<T> list = new List<T>();

            foreach (T enumValue in enumValues)
            {
                long enumValueLong = Convert.ToInt64(enumValue);
                if ((enumValueLong & inputValueLong) == enumValueLong && enumValueLong != 0)
                {
                    list.Add(value);
                }
            }

            return list;
        }

        public static bool IsFlagSelected(T value)
        {

            long inputValueLong = Convert.ToInt64(value);

            foreach (T enumValue in Enum.GetValues(typeof(T)))
            {
                long enumValueLong = Convert.ToInt64(enumValue);
                if ((enumValueLong & inputValueLong) == enumValueLong && enumValueLong != 0)
                {
                    return true;
                }
            }
            return false;

        }



    }

}






