namespace DMGToolBox.SaveData
{
    public interface IDataHandler
    {
        IGameData Load(string saveFilename);
        void Save(string saveFilename, IGameData gameData);
        string EncryptionKey { set; }
    }
}