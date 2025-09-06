using System.Threading.Tasks;

/// <summary>
/// セーブシステム
/// ローカルセーブやクラウドセーブなど、保存方法を抽象化する
/// </summary>
public interface ISaveService<T> where T : class
{
    Task SaveAsync(string key, T data);
    Task<T> LoadAsync(string key);
}
