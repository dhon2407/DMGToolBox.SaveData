## How to use

- Your game data class inherit from the interface IGameData, and must have and attribute of [Serializable].
- Since this system uses JsonUtility to serialize data, it is limited to atomic datatypes, plain class or struct with the [Serializable] attribute. (*see sample below*)
- In order to support types such as Dictionary, create a simple structure and use <mark>Serialize()</mark> and <mark>Deserialize()</mark> method to setup the data.
- You need to initialize the system by calling <mark>SaveDataManager.Init()</mark>
- Save game data using <mark>SaveDataManager.Save()</mark>
- Load game data using <mark>SaveDataManager.Load()</mark>

### *This just a simple save data system please comment somewhere for improvement ideas.*


### Sample

```
[Serializable]
public class TestData : IGameData
{
    #region Game saved data

    [SerializeField] private string name;
    [SerializeField] private uint level;
    [SerializeField] private float exp;
    [SerializeField] private BoolData[] boolDic;

    #endregion

    #region Data accessors

    public string Name
    {
        get => name;
        set => name = value;
    }

    public uint Level
    {
        get => level;
        set => level = value;
    }
    public float EXP
    {
        get => exp;
        set => exp = value;
    }
    
    public Dictionary<string, bool> Bools { get; private set; } = new();

    #endregion
        
    public void Serialize()
    {
        boolDic = new BoolData[Bools.Count];
        uint index = 0;
        foreach (KeyValuePair<string, bool> boolPair in Bools)
            boolDic[index++] = new BoolData { key = boolPair.Key, value = boolPair.Value };
    }

    public void Deserialize()
    {
        Bools.Clear();
        foreach (BoolData boolData in boolDic)
            Bools[boolData.key] = boolData.value;
    }
    
    [Serializable]
    private struct BoolData
    {
        public string key;
        public bool value;
    }
}
```