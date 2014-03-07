using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Util.Root
{
    public static class EnumHelpers
    {
        #region Attrib Utils
        public static string GetDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }

        public static string GetAbbreviation(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            Abbreviation[] attributes =
                (Abbreviation[])fi.GetCustomAttributes(typeof(Abbreviation), false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].AbbreviatedValue;
            }
            else
            {
                return value.ToString();
            }
        } 
        #endregion



        #region BitMask Utils
        public static bool Contains<T>(T flag, params T[] flags) where T : struct
        {
            if (flags != null)
            {
                int flagValue = (int)(object)flag;
                for (int i = 0; i < flags.Length; i++)
                {
                    int flagsValue = (int)(object)flags[i];

                    if ((flagsValue & flagValue) != 0)
                    {
                        return true;
                    }
                }
            }
            else
            {
                throw new NullReferenceException();
            }
            return false;
        }
        public static void Include<T>(ref T flags, params T[] against)
        {
            if (against != null)
            {
                for (int i = 0; i < against.Length; i++)
                {

                    int flagsValue = (int)(object)flags;
                    int flagValue = (int)(object)against[i];

                    flags = (T)(object)(flagsValue | flagValue);
                }
            }
            else
            {
                throw new NullReferenceException();
            }
        }

        public static void Exclude<T>(ref T flags, T flagtoremove)
        {
            int flagsValue = (int)(object)flags;
            int flagValue = (int)(object)flagtoremove;

            flags = (T)(object)(flagsValue & (~flagValue));
        }
        #endregion

    }
}
