using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace fnf.Client.Serialization
{
    public class Serializer : ISerializer
    {
        private JsonSerializerSettings settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public T Deserialize<T>(string messageBody)
        {
            return JsonConvert.DeserializeObject<T>(messageBody,settings);
        }

        public string Serialize<T>(T message)
        {
            return JsonConvert.SerializeObject(message,settings);
        }
    }
}
