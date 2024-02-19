using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenaLoader : MonoBehaviour
{
    public float transitionTime = 1f;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void StartAnimation()
    {
        if (animator != null)
            animator.SetTrigger(AnimationStrings.startTriger);
        else
            Debug.LogError("Animator is not assigned.");
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadScene(int levelIndex)
    {
        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }
}
