using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles events sent to player scripts and communications to outside
/// Main hub for player data
/// </summary>
public partial class Player : MonoBehaviour {
    
    // input action maps
    private PlayerActionMap _playerActionMap;
    public PlayerActionMap PlayerActionMap => _playerActionMap;

    #region Data Attributes
    [SerializeField] private PlayerData _data;
    public PlayerData Data => Data;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _jumpPower;
    [SerializeField] private float _throwDelay;
    [SerializeField] private float _throwSpeed;
    [SerializeField] private float _airMoveSpeed;

    // Accessors
    public float MoveSpeed => _moveSpeed;
    public float JumpPower => _jumpPower;
    public float ThrowDelay => _throwDelay;
    public float ThrowSpeed => _throwSpeed;
    public float AirMoveSpeed => _airMoveSpeed;
    #endregion

    void Awake() {
        _playerActionMap = new PlayerActionMap();
        InitializeAttributes(); 
    }

    void Start() {
        _t = transform;
        _rb = GetComponent<Rigidbody>();
        _coll = GetComponent<Collider>();

        // subscribe to events
        _playerActionMap.Movement.Jump.performed += Jump;
        _playerActionMap.Actions.ShootBubble.performed += Shoot;
        _playerActionMap.Actions.MoveRing.performed += MoveRing;
        _playerActionMap.Actions.MoveRing.canceled += MoveRing;
        _playerActionMap.Enable();
    }

    protected void InitializeAttributes() {
        _moveSpeed = _data.MoveSpeed;
        _jumpPower = _data.JumpPower;
        _throwDelay = _data.ThrowDelay;
        _throwSpeed = _data.ThrowSpeed;
        _airMoveSpeed = _data.AirMoveSpeed;
    }

    //public PlayerActionMap GetActionMap() {
    //    return _playerActionMap;
    //}
}
