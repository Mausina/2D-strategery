using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowScript : MonoBehaviour
{
    public DetectionZone zone;
    Animator animator;


    public bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _hasTarget; }
        private set { _hasTarget = value;
         animator.SetBool(AnimationStrings.hasTarget, value);
        }


    }

    private void Update()
    {
        HasTarget = zone.detectedColliders.Count > 0;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
}
