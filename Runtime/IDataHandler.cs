namespace DMGToolBox.SaveData
{
    public interface IDataHandler
    {
        T Load<T>(string saveFilename) where T : IGameData;
        void Save(string saveFilename, IGameData gameData);
        string EncryptionKey { set; }
    }
}