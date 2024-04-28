using System;
using UnityEngine;

namespace DMGToolBox.SaveData
{
    public class SaveDataManager
    {
        public static SaveLoadActionResult Save(string saveFileName, IGameData gameData) =>
            Instance.SaveGame(saveFileName, gameData);
        public static IGameData Load(string saveFileName) => 
            Instance.LoadGame(saveFileName);
        public static IGameData CurrentData => 
            Instance.LoadCurrentData();

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
        private IGameData _currentData;
        
        private SaveLoadActionResult SaveGame(string saveFileName, IGameData gameData)
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
        
        private IGameData LoadGame(string saveFileName)
        {
            try
            {
                IGameData newData = _dataHandler.Load(saveFileName);
                
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
        
        private IGameData LoadCurrentData()
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