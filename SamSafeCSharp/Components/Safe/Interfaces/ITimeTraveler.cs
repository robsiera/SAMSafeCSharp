namespace SamSafeCSharp.Components
{
    public interface ITimeTraveler
    {
        int Cursor { get; set; }

        DefaultSnapshotStore SnapshotStore { get; set; }

        void Init(string path, string next);

        int SaveSnapshot(IModel model, string dataset);

        string GetSnapshot(int i);

        string DisplayTimeTravelControls(string representation);
      
    }
}
