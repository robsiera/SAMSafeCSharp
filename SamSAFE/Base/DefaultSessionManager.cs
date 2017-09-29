using SamSAFE.Interfaces;

namespace SamSAFE.Base
{
    public class DefaultSessionManager : ISessionManager
    {
        public void DehydrateSession(IModel model)
        {
            if (string.IsNullOrEmpty(model.__token))
            {
                //todo:  safe.defaultSessionManager[model.__token] = model.__session;
            }
        }

        public string RehydrateSession(string token)
        {
            string session = token;
            return session;
        }
    };
}