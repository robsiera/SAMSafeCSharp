using Newtonsoft.Json;

namespace SamSafeCSharp.Helpers
{
    public static class JsHelpers
    {
        public static class JSON
        {
            public static string stringify(object obj)
            {
                //todo: implement serialize to string
                return JsonConvert.SerializeObject(obj);
            }
        }

        public static string orDefault(this string value, string defaultValue)
        {
            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }
        
    }
}