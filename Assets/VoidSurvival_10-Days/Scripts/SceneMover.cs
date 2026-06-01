using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class SceneMover : MonoBehaviour
{
    [SerializeField] private string m_SceneName;
    [SerializeField] private Vector2 spawnPoint;
    private async void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneLording.Instance.OpenDialog();
            AsyncOperation _loadOp = SceneManager.LoadSceneAsync(m_SceneName);
            _loadOp.allowSceneActivation = false;
            await UniTask.WaitUntil(() => _loadOp.progress >= 0.9f);
            other.transform.position = spawnPoint;
            CrossPlatformInputManager.SetAxisZero("Horizontal");
            CrossPlatformInputManager.SetAxisZero("Vertical");
            _loadOp.allowSceneActivation = true;
            await UniTask.WaitUntil(() => _loadOp.isDone);
            SceneLording.Instance.CloseDialog();
        }
    }
}
