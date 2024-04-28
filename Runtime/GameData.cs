using System;

namespace DMGToolBox.SaveData
{
    /// <summary>
    /// Game data Base class
    /// </summary>
    public abstract class GameData
    {
        public abstract void Serialize();
        public abstract void Deserialize();
    }
}