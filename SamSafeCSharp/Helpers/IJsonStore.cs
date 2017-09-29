using Microsoft.AspNetCore.Http;

namespace SamSafeCSharp.Helpers
{
    public interface IJsonStore
    {
        T GetObjectFromJson<T>(string sessionposts);
        void SetObjectAsJson(string sessionposts, object data);
    }

    public class SessionStore : IJsonStore
    {
        private readonly IHttpContextAccessor _httpContextAccesssor;

        public SessionStore(IHttpContextAccessor httpContextAccesssor)
        {
            _httpContextAccesssor = httpContextAccesssor;
        }

        public T GetObjectFromJson<T>(string sessionposts)
        {
            return _httpContextAccesssor.HttpContext.Session.GetObjectFromJson<T>(sessionposts);
        }

        public void SetObjectAsJson(string key, object data)
        {
            _httpContextAccesssor.HttpContext.Session.SetObjectAsJson(key, data);
        }
    }
}