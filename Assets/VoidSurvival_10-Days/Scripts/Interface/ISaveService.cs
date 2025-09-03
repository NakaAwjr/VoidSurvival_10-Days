using System.Threading.Tasks;

/// <summary>
/// セーブシステム
/// ローカルセーブやクラウドセーブなど、保存方法を抽象化する
/// </summary>
public interface ISaveService
{
    Task SaveAsync(string key, SaveData data);
    Task<SaveData> LoadAsync(string key);
}
