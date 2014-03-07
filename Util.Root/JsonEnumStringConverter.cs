
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Util.Root
{
    public class JsonEnumStringConverter : Newtonsoft.Json.Converters.StringEnumConverter
    {
        //public override void WriteJson<Tw>(JsonWriter writer, object value, JsonSerializer serializer)
        //{
        //    if (value is Action)
        //    {
        //        writer.WriteValue(Enum.GetName(typeof(T), (T)value));// or something else
        //        return;
        //    }

        //    base.WriteJson(writer, value, serializer);
        //}
    }
}
