using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour
{
    Animator animator;
    
    public Transform rightHandTarget;
    public Transform leftHandTarget;

    public Transform rightElbowTarget;
    public Transform leftElbowTarget;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        //float reach = animator.GetFloat("RightHandReach");
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0.75f);
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
        
        animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0.75f);
        animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);

    }
}
