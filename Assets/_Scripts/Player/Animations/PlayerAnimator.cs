using System.Collections;
using System.Collections.Generic;
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
    }

    private void PlayerRef_OnShootBubble(Vector3 _, Transform __) {
        animator.SetTrigger(throwParam);
    }

    private void PlayerRef_OnJump() {
        animator.SetTrigger(jumpParam);
    }
}