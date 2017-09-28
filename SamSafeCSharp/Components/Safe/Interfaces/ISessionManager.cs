namespace SamSafeCSharp.Components
{
    public interface ISessionManager
    {
        void DehydrateSession(IModel model);

        string RehydrateSession(string token);
    }
}
