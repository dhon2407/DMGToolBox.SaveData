
namespace DMGToolBox.SaveData
{
    /// <summary>
    /// Game data interface class
    /// </summary>
    public interface IGameData
    {
        void Serialize();
        void Deserialize();
    }
}