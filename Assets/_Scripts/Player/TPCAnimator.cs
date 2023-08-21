using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ThirdPersonControllerV2))]
public class TPCAnimator : MonoBehaviour
{
    [Header("Animation")]
    public Animator CharacterAnimator;

    private ThirdPersonControllerV2 tpc;

    void Awake() {
        tpc = GetComponent<ThirdPersonControllerV2>();
    }

    void Update() {
        if(CharacterAnimator != null) {
            UpdateAnimations();
        }
    }

    // Current animations are hardcoded, this script should be adapted in the future to be
    // adaptable.
    void UpdateAnimations() {
        CharacterAnimator.SetBool("Grounded", tpc.isGrounded);
        CharacterAnimator.SetBool("Jump", tpc.jumped);
        CharacterAnimator.SetFloat("Speed", tpc.currentSpeed);
        CharacterAnimator.SetFloat("MotionSpeed", 1.2f);
    }

}
