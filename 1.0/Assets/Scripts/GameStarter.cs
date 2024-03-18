using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour
{
    public int firstWorldIndex;  // Index of the initial world scene
    public int secondWorldIndex; // Index of the second world scene

    void Start()
    {
        LoadSceneIfNotLoaded(firstWorldIndex); // Load the initial world
        LoadSceneIfNotLoaded(secondWorldIndex); // Load the second world
    }

    private void LoadSceneIfNotLoaded(int sceneIndex)
    {
        if (!SceneManager.GetSceneByBuildIndex(sceneIndex).isLoaded)
        {
            StartCoroutine(LoadSceneAsync(sceneIndex));
        }
        else
        {
            Debug.LogWarning($"Scene at index {sceneIndex} is already loaded.");
        }
    }

    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        Debug.Log($"Starting to load scene {sceneIndex}");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false; // Prevents the scene from activating immediately after loading

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f && !asyncLoad.allowSceneActivation)
            {
                asyncLoad.allowSceneActivation = true; // Allow activation when ready
            }
            yield return null;
        }

        Debug.Log($"Finished loading scene {sceneIndex}");
        // Optionally, activate the scene if it needs to be the active scene
        // Note: Only one scene can be "active" at a time, which affects where new game objects are instantiated
    }
}
