using System;
using UnityEngine;

namespace DMGToolBox.SaveData
{
    public class SaveDataManager
    {
        public static SaveLoadActionResult Save(string saveFileName, GameData gameData) =>
            Instance.SaveGame(saveFileName, gameData);
        public static GameData Load(string saveFileName) => 
            Instance.LoadGame(saveFileName);

        public static void SetEncryptionKey(string key)
        {
            Instance._dataHandler.EncryptionKey = key;
        }
        
        private static SaveDataManager _instance;
        private static SaveDataManager Instance =>
            _instance ??= new SaveDataManager(Application.persistentDataPath);

        private SaveDataManager(string saveDirectory)
        {
            _dataHandler = new FileDataHandler(saveDirectory);
        }

        private readonly IDataHandler _dataHandler;
        private GameData _loadedData;
        
        private SaveLoadActionResult SaveGame(string saveFileName, GameData gameData)
        {
            try
            {
                _dataHandler.Save(saveFileName, gameData);
            }
            catch (Exception _)
            {
                return SaveLoadActionResult.Failed;
            }

            return SaveLoadActionResult.Success;
        }
        
        private GameData LoadGame(string saveFileName)
        {
            try
            {
                _loadedData = _dataHandler.Load(saveFileName);
            }
            catch (Exception _)
            {
                return null;
            }

            return _loadedData;
        }
    }

    public enum SaveLoadActionResult
    {
        Success,
        Failed
    }
}