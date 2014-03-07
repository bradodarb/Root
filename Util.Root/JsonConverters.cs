using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;


namespace Util.Root
{


    public class JsonConverterMap : Newtonsoft.Json.JsonConverter
    {
        private readonly Dictionary<Type, JsonConvertType> _map;


        public JsonConverterMap(Dictionary<Type, JsonConvertType> map)
        {
            _map = map;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
           

           
        }

         public override bool CanConvert(Type objectType)
        {
            return _map.Keys.Any(t => t == objectType);
        }

         public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
         {
             throw new NotImplementedException();
         }
    }
}
