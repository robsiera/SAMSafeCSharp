namespace SamSAFE.Interfaces
{
    public interface ISessionManager
    {
        void DehydrateSession(IModel model);

        string RehydrateSession(string token);
    }
}
