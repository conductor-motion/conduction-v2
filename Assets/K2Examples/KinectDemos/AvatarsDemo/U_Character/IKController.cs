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

    public Transform rightFootTarget;
    public Transform leftFootTarget;

    public Transform rightKneeTarget;
    public Transform leftKneeTarget;

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

        animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 0.75f);
        animator.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbowTarget.position);

        animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 0.75f);
        animator.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbowTarget.position);


        /*animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0.75f);
        animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootTarget.position);

        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0.75f);
        animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootTarget.position);

        animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, 0.5f);
        animator.SetIKHintPosition(AvatarIKHint.RightKnee, rightKneeTarget.position);

        animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, 0.5f);
        animator.SetIKHintPosition(AvatarIKHint.LeftKnee, leftKneeTarget.position);*/
    }
}
