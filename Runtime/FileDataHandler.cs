using System;
using System.IO;
using UnityEngine;

namespace DMGToolBox.SaveData
{
    public class FileDataHandler : IDataHandler
    {
        private readonly string _dirPath;
        private string _encryptionKey;
        
        /// <summary>
        /// FileDataHandler constructor
        /// </summary>
        /// <param name="dataDirPath">Folder path to save data files</param>
        /// <param name="encryptionKey">File data encryption key</param>
        public FileDataHandler(string dataDirPath, string encryptionKey = null)
        {
            _encryptionKey = string.IsNullOrEmpty(encryptionKey) ? null : encryptionKey;
            _dirPath = dataDirPath;
            
            Debug.Log($"Save file location :{_dirPath}");
        }

        /// <summary>
        /// Set encryption key
        /// </summary>
        public string EncryptionKey
        {
            set => _encryptionKey = value;
        }

        /// <summary>
        /// Load a saved file
        /// </summary>
        /// <param name="saveFilename">Save data filename</param>
        /// <returns></returns>
        public T Load<T>(string saveFilename) where T : IGameData
        {
            string fullPath = Path.Combine(_dirPath, saveFilename);
            T loadedData = default;
            
            if (!File.Exists(fullPath))
            {
                Debug.LogError($"Save path not existing :{fullPath}");
                return default;
            }
            
            try
            {
                using FileStream stream = new(fullPath, FileMode.Open);
                using StreamReader reader = new(stream);

                string dataToLoad = reader.ReadToEnd();

                if (_encryptionKey != null)
                    dataToLoad = EncryptDecrypt(dataToLoad);

                loadedData = JsonUtility.FromJson<T>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed loading data :{e}");
            }

            return loadedData;
        }

        /// <summary>
        /// Save a game data to file.
        /// </summary>
        /// <param name="saveFilename">Save data filename</param>
        /// <param name="gameData">Game data to save</param>
        /// <exception cref="UnityException">Errors when saving</exception>
        public void Save(string saveFilename, IGameData gameData)
        {
            string fullPath = Path.Combine(_dirPath, saveFilename);
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ??
                                          throw new UnityException("Error getting directory.."));

                string dataToStore = JsonUtility.ToJson(gameData, true);
                
                if (_encryptionKey != null)
                    dataToStore = EncryptDecrypt(dataToStore);

                using FileStream stream = new(fullPath, FileMode.Create);
                using StreamWriter writer = new(stream);
                
                writer.Write(dataToStore);
            }
            catch (Exception e)
            {
                throw new UnityException($"Error occured when writing save file:{fullPath} ({e})");
            }
            
            Debug.Log($"Data file successfully saved to {fullPath}");
        }

        /// <summary>
        /// Encrypt / Decrypt the data
        /// </summary>
        /// <param name="data">Data to encrypt/decrypt</param>
        /// <returns>Result data</returns>
        private string EncryptDecrypt(string data)
        {
            if (_encryptionKey == null)
                return data;
            
            string modifiedData = string.Empty;
            for (int i = 0; i < data.Length; i++)
                modifiedData += (char) (data[i] ^ _encryptionKey[i % _encryptionKey.Length]);

            return modifiedData;
        }
    }
}