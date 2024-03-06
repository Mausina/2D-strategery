using System.Collections;
using UnityEngine;
using UnityEngine.AI; // For NavMeshAgent

public class BuilderController : MonoBehaviour
{
    public Animator animator;
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>(); // Make sure your Builder has a NavMeshAgent component
    }

    public void MoveToConstructionSite(Vector3 sitePosition, float buildTime)
    {
        StartCoroutine(MoveAndBuild(sitePosition, buildTime));
    }

    private IEnumerator MoveAndBuild(Vector3 sitePosition, float buildTime)
    {
        agent.SetDestination(sitePosition); // Set the destination to the construction site

        // Wait until the builder has reached the destination
        while (Vector3.Distance(transform.position, sitePosition) > agent.stoppingDistance)
        {
            yield return null; // Wait for the next frame before continuing the loop
        }

        // Upon arrival, trigger the building animation
        animator.SetBool("IsBuilding", true); // Assumes you have a bool parameter named 'IsBuilding' in your Animator

        // Simulate building time
        yield return new WaitForSeconds(buildTime);

        // Stop building animation
        animator.SetBool("IsBuilding", false);
    }
}
