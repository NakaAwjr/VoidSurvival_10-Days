using System.Threading.Tasks;
using UnityEngine;

public class LocalSaveService : ISaveService
{
    public async Task SaveAsync(string key, SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }
    public async Task<SaveData> LoadAsync(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            string json = PlayerPrefs.GetString(key);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data;
        }
        return null;
    }
}