using Newtonsoft.Json;
using RestSharp.Deserializers;
using RestSharp.Serializers;
using System.IO;

namespace DeleteBoilerplate.Common.Serializers
{
    public class NewtonsoftJsonSerializer : ISerializer, IDeserializer
    {
        private readonly JsonSerializer _serializer;

        public NewtonsoftJsonSerializer()
        {
            this._serializer = new JsonSerializer();
        }

        public NewtonsoftJsonSerializer(JsonSerializer serializer)
        {
            this._serializer = serializer;
        }

        public string ContentType
        {
            get => "application/json";
            set { }
        }

        public string Serialize(object obj)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var jsonTextWriter = new JsonTextWriter(stringWriter))
                {
                    _serializer.Serialize(jsonTextWriter, obj);

                    return stringWriter.ToString();
                }
            }
        }

        public T Deserialize<T>(RestSharp.IRestResponse response)
        {
            var content = response.Content;

            using (var stringReader = new StringReader(content))
            {
                using (var jsonTextReader = new JsonTextReader(stringReader))
                {
                    return _serializer.Deserialize<T>(jsonTextReader);
                }
            }
        }
    }
}
