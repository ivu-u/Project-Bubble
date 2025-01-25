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


    [SerializeField] private PlayerData _data;
    public PlayerData Data => Data;

    void Awake() {
        _playerActionMap = new PlayerActionMap();
        InitializeAttributes(); 
    }

    void Start() {
        _t = transform;

        // subscribe to events
        _playerActionMap.Movement.Walk.performed += PlayerMovement;
    }

    protected void InitializeAttributes() {

    }
}
