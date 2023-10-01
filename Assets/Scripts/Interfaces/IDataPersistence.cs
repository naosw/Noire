/// <summary>
/// Interface for saving and loading game data. An object that implements this
/// interface will be recognized at run time and its data will be serialized/deserialized
/// upon game save
/// </summary>

public interface IDataPersistence
{
    void LoadData(GameData data);
    void SaveData(GameData data);
}