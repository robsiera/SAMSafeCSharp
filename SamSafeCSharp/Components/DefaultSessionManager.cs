namespace SamSafeCSharp.Components
{
    public class DefaultSessionManager
    {
        public void DehydrateSession(object model)
        {
            /*if (model.__token)
            {
                safe.defaultSessionManager[model.__token] = model.__session;
            }*/
        }

        public object RehydrateSession(object token)
        {
            object session = token;
            return session;
        }
    };
}