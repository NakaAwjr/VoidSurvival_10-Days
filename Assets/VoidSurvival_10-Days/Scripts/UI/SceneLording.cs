using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLording : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        SceneManager.activeSceneChanged += (Scene current, Scene next) =>
        {
            gameObject.SetActive(true);
        };
        SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
        {
            gameObject.SetActive(false);
        };
    }
}
