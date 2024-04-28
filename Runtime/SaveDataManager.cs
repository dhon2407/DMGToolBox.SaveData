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
        public static GameData LoadCurrentData() => 
            Instance.LoadCurrentDataImpl();

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
        private GameData _currentData;
        
        private SaveLoadActionResult SaveGame(string saveFileName, GameData gameData)
        {
            try
            {
                gameData.Serialize();
                _dataHandler.Save(saveFileName, gameData);
                
                _currentData ??= gameData;
            }
            catch (Exception)
            {
                return SaveLoadActionResult.Failed;
            }

            return SaveLoadActionResult.Success;
        }
        
        private GameData LoadGame(string saveFileName)
        {
            try
            {
                GameData newData = _dataHandler.Load(saveFileName);
                
                if (newData == null)
                    return null;
                
                if (_currentData == null)
                {
                    _currentData = newData;
                    _currentData.Deserialize();
                }
                
            }
            catch (Exception)
            {
                return null;
            }

            return _currentData;
        }
        
        private GameData LoadCurrentDataImpl()
        {
            if (_currentData == null)
                throw new UnityException("No loaded data..");
            
            _currentData.Deserialize();
            return _currentData;
        }
    }
    
    

    public enum SaveLoadActionResult
    {
        Success,
        Failed
    }
}