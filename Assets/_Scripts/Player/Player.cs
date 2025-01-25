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

    // Accessors
    public float MoveSpeed => _moveSpeed;
    public float JumpPower => _jumpPower;
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
        _playerActionMap.Actions.Attack.performed += Attack;
        _playerActionMap.Enable();
    }

    protected void InitializeAttributes() {
        _moveSpeed = _data.MoveSpeed;
        _jumpPower = _data.JumpPower;
    }
}
