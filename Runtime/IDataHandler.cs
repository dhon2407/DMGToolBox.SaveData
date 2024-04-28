namespace DMGToolBox.SaveData
{
    public interface IDataHandler
    {
        GameData Load(string saveFilename);
        void Save(string saveFilename, GameData gameData);
        string EncryptionKey { set; }
    }
}