using System;
using UnityEngine;

namespace DMGToolBox.SaveData
{
    public class SaveDataManager
    {
        /// <summary>
        /// Initialize Save data manager
        /// </summary>
        /// <param name="savePath">Folder which save files will be created.</param>
        /// <returns>Operation result</returns>
        public static SaveLoadActionResult Init(string savePath = null)
        {
            _currentDirPath = string.IsNullOrEmpty(savePath) ? _currentDirPath : savePath;
            if (_instance != null)
                return SetPath(_currentDirPath);
            
            _instance = new SaveDataManager(_currentDirPath);
            return SaveLoadActionResult.Success;
        }
        
        /// <summary>
        /// Save a game data
        /// </summary>
        /// <param name="saveFileName">Game save filename</param>
        /// <param name="gameData">Game data to be saved.</param>
        /// <returns>Operation result</returns>
        public static SaveLoadActionResult Save(string saveFileName, IGameData gameData)
        {
            return _instance?.SaveGame(saveFileName, gameData) ?? SaveLoadActionResult.NotReady;
        }

        /// <summary>
        /// Load a game data
        /// </summary>
        /// <param name="saveFileName">Filename of data to be saved.</param>
        /// <param name="gameData">Game data result</param>
        /// <typeparam name="T">It should be a type of IGameData</typeparam>
        /// <returns>Operation result</returns>
        public static SaveLoadActionResult Load<T>(string saveFileName, out T gameData) where T : IGameData
        {
            gameData = default;
            return _instance == null ? SaveLoadActionResult.NotReady : _instance.LoadGame(saveFileName, out gameData);
        }

        /// <summary>
        /// Get current loaded game data
        /// </summary>
        /// <param name="gameData">Current game data</param>
        /// <typeparam name="T">It should be a type of IGameData</typeparam>
        /// <returns>Operation result</returns>
        public static SaveLoadActionResult GetCurrentData<T>(ref T gameData) where T : IGameData
        {
            return _instance?.LoadCurrentData(ref gameData) ?? SaveLoadActionResult.NotReady;
        }

        /// <summary>
        /// Update the save game data folder directory
        /// </summary>
        /// <param name="savePath">New directory</param>
        /// <returns>Operation result</returns>
        public static SaveLoadActionResult SetPath(string savePath)
        {
            if (_instance == null)
                return SaveLoadActionResult.NotReady;
            
            _currentDirPath = savePath;
            _instance._dataHandler.DirPath = _currentDirPath;

            return SaveLoadActionResult.Success;
        }
        
        /// <summary>
        /// Set save file Encryption key for additional security
        /// </summary>
        /// <param name="key">Encryption key</param>
        /// <returns>Operation result</returns>
        public static SaveLoadActionResult SetEncryptionKey(string key)
        {
            if (_instance == null)
                return SaveLoadActionResult.NotReady;
            
            _instance._dataHandler.EncryptionKey = key;

            return SaveLoadActionResult.Success;
        }
        
        private static SaveDataManager _instance;
        private static string _currentDirPath = Application.persistentDataPath;

        private SaveDataManager(string saveDirectory)
        {
            _currentDirPath = saveDirectory;
            _dataHandler = new FileDataHandler(_currentDirPath);
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
        
        private SaveLoadActionResult LoadGame<T>(string saveFileName, out T gameData) where T : IGameData
        {
            gameData = default;
            try
            {
                gameData = _dataHandler.Load<T>(saveFileName);

                if (gameData == null)
                    return SaveLoadActionResult.Failed;
                
                _currentData = gameData;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed loading operation :{e}");
                return SaveLoadActionResult.Failed;
            }

            gameData.Deserialize();
            return SaveLoadActionResult.Success;
        }
        
        private SaveLoadActionResult LoadCurrentData<T>(ref T gameData) where T : IGameData
        {
            if (_currentData == null)
                return SaveLoadActionResult.NotReady;
            
            _currentData.Deserialize();
            return SaveLoadActionResult.Success;
        }
    }
    
    public enum SaveLoadActionResult
    {
        Success,
        Failed,
        NotReady,
    }
}