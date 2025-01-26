using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actor Data/Player")]
public class PlayerData : ScriptableObject {
    [Header("Movement Variables")]
    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private float _jumpPower = 5;
    [SerializeField] private float _throwDelay = 5;
    [SerializeField] private float _throwSpeed = 5;

    // Accessors
    public float MoveSpeed => _moveSpeed;
    public float JumpPower => _jumpPower;
    public float ThrowDelay => _throwDelay;
    public float ThrowSpeed => _throwSpeed;
}
