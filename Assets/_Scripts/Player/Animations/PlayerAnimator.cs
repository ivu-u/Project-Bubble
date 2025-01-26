using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    private const string WALK_SPEED_PARAM = "WalkSpeed",
                         V_SPEED_PARAM = "VerticalSpeed",
                         THROW_PARAM = "Throw",
                         JUMP_PARAM = "Jump";

    [SerializeField] private Animator animator;
    [SerializeField] private Player playerRef;

    private int walkSpeedParam,
                vSpeedParam,
                throwParam,
                jumpParam;
    
    [Header("Position Oscillation Settings")]
    public float positionAmplitude = 1.0f;  // How far the object moves up and down
    public float positionFrequency = 1.0f;  // How fast the object moves

    [Header("Scale Oscillation Settings")]
    public float scaleAmplitude = 0.5f;  // How much the object's scale changes
    public float scaleFrequency = 1.5f;  // How fast the scale oscillates

    public float offset = 2f;
    
    private float bobTimer;
    private float childBobTimer;

    void Awake() {
        walkSpeedParam = Animator.StringToHash(WALK_SPEED_PARAM);
        vSpeedParam = Animator.StringToHash(V_SPEED_PARAM);
        throwParam = Animator.StringToHash(THROW_PARAM);
        jumpParam = Animator.StringToHash(JUMP_PARAM);

        playerRef.OnShootBubble += PlayerRef_OnShootBubble;
        playerRef.OnJump += PlayerRef_OnJump;
    }

    void Update() {
        animator.SetFloat(walkSpeedParam, Mathf.Abs(playerRef.WalkSpeed));
        animator.SetFloat(vSpeedParam, Mathf.Abs(playerRef.VerticalSpeed));
        Oscillate();
    }

    private void PlayerRef_OnShootBubble(Vector3 _, Transform __) {
        animator.SetTrigger(throwParam);
    }

    private void PlayerRef_OnJump() {
        animator.SetTrigger(jumpParam);
    }

    private void Oscillate() {
        if (Math.Abs(playerRef.WalkSpeed) > 0.1 && Math.Abs(playerRef.VerticalSpeed) < 0.1) {
            bobTimer += Time.deltaTime * positionFrequency;
            float stretchFactor = 1 + Mathf.Sin(bobTimer) * positionAmplitude;
            transform.GetChild(0).localScale =
                new Vector3(1 / stretchFactor, stretchFactor, transform.GetChild(0).localScale.z);

            childBobTimer += Time.deltaTime * scaleFrequency;
            float bobOffset = Mathf.Sin(childBobTimer + offset) * scaleAmplitude;
            transform.localPosition =
                new Vector3(transform.localPosition.x, bobOffset, transform.localPosition.z);
        }
        else {
            transform.localPosition = Vector3.zero;
            transform.GetChild(0).localScale = Vector3.one;
        }
    }
}