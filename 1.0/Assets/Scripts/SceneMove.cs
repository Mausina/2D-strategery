using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMove : MonoBehaviour
{
    public string sceneToLoad;
    ScenaLoader scenaLoader;
    public float transitionTime = 1f;
    private void Start()
    {
        scenaLoader = new ScenaLoader();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            StartCoroutine(StartTransit());
        }
    }

    private IEnumerator StartTransit()
    {
        scenaLoader.StartAnimation();
        yield return new WaitForSeconds(scenaLoader.transitionTime);

        // Load the new scene
        SceneManager.LoadScene(sceneToLoad);
    }
}

