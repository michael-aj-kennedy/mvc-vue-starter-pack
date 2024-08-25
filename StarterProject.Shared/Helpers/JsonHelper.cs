using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace StarterProject.Shared.Helpers;

public static class JsonHelper
{
    public static readonly Encoding Encoding = new UTF8Encoding(false, true);

    public static JsonSerializer Serializer { get; set; }

    private static readonly JsonSerializerSettings jsonSerializerSettings;

    static JsonHelper()
    {
        jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
#if DEBUG
            Formatting = Formatting.Indented,
#endif
        };

        jsonSerializerSettings.Converters.Add(new IsoDateTimeConverter());

        Serializer = JsonSerializer.Create(jsonSerializerSettings);


    }

    public static T DeserializeJson<T>(Stream stream)
    {
        using (var streamReader = new StreamReader(stream, Encoding))
        {
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                return (T)Serializer.Deserialize(jsonTextReader, typeof(T));
            }
        }
    }

    public static string ToJsonNet(this object obj)
    {
        return JsonConvert.SerializeObject(obj, jsonSerializerSettings);
    }

    public static string ToJsonNet(this object obj, Formatting formatting)
    {
        return JsonConvert.SerializeObject(obj, formatting, jsonSerializerSettings);
    }

    public static T JsonNetToObject<T>(this string jsonString)
    {
        return JsonConvert.DeserializeObject<T>(jsonString, jsonSerializerSettings);
    }

    public static Task<T> DeserializeJsonAsync<T>(Task<Stream> streamAsync)
    {
        return streamAsync.ContinueWith(streamTask => DeserializeJson<T>(streamTask.Result));
    }
}