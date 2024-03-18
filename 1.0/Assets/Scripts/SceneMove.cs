using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Assuming worldSceneIndexes[0] is the index of the first world and worldSceneIndexes[1] is the index of the second world
            WorldsManager.Instance.SwitchWorld();
        }
    }
}
