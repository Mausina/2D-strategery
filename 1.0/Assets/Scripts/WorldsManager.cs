using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldsManager : MonoBehaviour
{
    public static WorldsManager Instance;

    // Indices of your two worlds.
    public int firstWorldIndex;
    public int secondWorldIndex;
    private int currentActiveWorldIndex;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            currentActiveWorldIndex = firstWorldIndex;
            Debug.Log("WorldsManager is set up and persistent across scenes.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SwitchWorld()
    {
        // Get the index of the world that we want to switch to.
        int newWorldIndex = (currentActiveWorldIndex == firstWorldIndex) ? secondWorldIndex : firstWorldIndex;

        // Ensure the world we're switching to is not already loaded.
        if (!SceneManager.GetSceneByBuildIndex(newWorldIndex).isLoaded)
        {
            StartCoroutine(LoadSceneAsync(newWorldIndex, true));
        }
        else
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(newWorldIndex));
            Debug.Log($"Switched to world {newWorldIndex}.");
        }

        currentActiveWorldIndex = newWorldIndex;
    }

    private IEnumerator LoadSceneAsync(int sceneIndex, bool setActiveOnLoad)
    {
        Debug.Log($"Starting to load scene {sceneIndex}");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
        asyncLoad.allowSceneActivation = false; // Start with the scene activation set to false

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f && !asyncLoad.allowSceneActivation)
            {
                // If we're ready to activate the scene and the setActiveOnLoad flag is true
                if (setActiveOnLoad)
                {
                    asyncLoad.allowSceneActivation = true; // Now allow activation
                }
            }
            yield return null;
        }

        Debug.Log($"Finished loading scene {sceneIndex}");

        if (setActiveOnLoad)
        {
            // If setActiveOnLoad is true, set the loaded scene as the active scene
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneIndex));
        }

        // Additional logic after loading can go here (e.g., deactivating cameras from other scenes)
    }


}
