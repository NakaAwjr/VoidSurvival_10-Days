using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class SceneMover : MonoBehaviour
{
    [SerializeField] private string m_SceneName;
    [SerializeField] private Vector2 spawnPoint;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(m_SceneName);
            other.transform.position = spawnPoint;
            CrossPlatformInputManager.SetAxisZero("Horizontal");
            CrossPlatformInputManager.SetAxisZero("Vertical");
        }
    }
}
