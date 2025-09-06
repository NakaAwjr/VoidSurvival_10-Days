using System.Threading.Tasks;
using UnityEngine;

public class LocalSaveService<T> : ISaveService<T> where T : class
{
    public async Task SaveAsync(string key, T data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
        Debug.Log(json);
    }
    public async Task<T> LoadAsync(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            string json = PlayerPrefs.GetString(key);
            T data = JsonUtility.FromJson<T>(json);
            return data;
        }
        return null;
    }
}