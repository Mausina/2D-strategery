using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeRemover : StateMachineBehaviour
{
    public float fadeTime = 0.5f;
    private float timeElapsed = 0f;
    SpriteRenderer spriteRenderer;
    GameObject objToRemove;
    Color StartColar;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timeElapsed = 0f;
        spriteRenderer = animator.GetComponent<SpriteRenderer>();
        StartColar = spriteRenderer.color;
        objToRemove = animator.gameObject;

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       timeElapsed += Time.deltaTime;
        float newAlpha = StartColar.a * (1 -(timeElapsed / fadeTime));

        spriteRenderer.color = new Color(StartColar.r, StartColar.g, StartColar.b, newAlpha);

        if (timeElapsed > fadeTime) 
        {
            Destroy(objToRemove);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

}
