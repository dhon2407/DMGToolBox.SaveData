using System;
using UnityEngine;

namespace DMGToolBox.SaveData
{
    public class SaveDataManager
    {
        public static SaveLoadActionResult Save(string saveFileName, IGameData gameData) =>
            Instance.SaveGame(saveFileName, gameData);
        public static T Load<T>(string saveFileName) where T : IGameData => 
            Instance.LoadGame<T>(saveFileName);
        public static IGameData CurrentData => 
            Instance.LoadCurrentData();

        public static void SetPath(string savePath)
        {
            _currentDirPath = savePath;
            Instance._dataHandler.DirPath = _currentDirPath;
        }
        
        public static void SetEncryptionKey(string key)
        {
            Instance._dataHandler.EncryptionKey = key;
        }
        
        private static SaveDataManager _instance;
        private static SaveDataManager Instance =>
            _instance ??= new SaveDataManager(_currentDirPath);

        private static string _currentDirPath = Application.persistentDataPath;

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
            catch (Exception e)
            {
                Debug.LogError($"Failed saving operation :{e}");
                return SaveLoadActionResult.Failed;
            }

            return SaveLoadActionResult.Success;
        }
        
        private T LoadGame<T>(string saveFileName) where T : IGameData
        {
            try
            {
                _currentData = _dataHandler.Load<T>(saveFileName);
                
                if (_currentData == null)
                    return _currentData;
                
                _currentData.Deserialize();
                
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed loading operation :{e}");
                return default;
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