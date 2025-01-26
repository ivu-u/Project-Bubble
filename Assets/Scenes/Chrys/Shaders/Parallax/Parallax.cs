using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {
    [SerializeField] private float baseSpeed;
    [SerializeField] private float speedSubtractive;
    [SerializeField] private List<Material> materials;

    private void Update() {
        if (Input.GetAxis("Horizontal") != 0) {
            float speed = Mathf.Round(Input.GetAxis("Horizontal")) * -1;
            for (int i = 0; i < materials.Count; i++) {
                materials[i].SetVector("_Offset", new Vector4(0f, speed * (baseSpeed * (1 - (speedSubtractive * i))), 0f, 0f));
            }
        }
    }
}
