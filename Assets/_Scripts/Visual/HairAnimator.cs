using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairAnimator : MonoBehaviour {
    [SerializeField] private MeshRenderer renderer;

    //warning: these will be changed in the animator
    [SerializeField] private Texture gradientTex;
    [SerializeField] private Texture weightTex;
    [SerializeField] private Texture outlineTex;
    [SerializeField] private Texture maskTex;

    #region References
    private const string _weightMapRef = "_Weight_Map";
    private const string _gradientMapRef = "_Gradient_Map";
    private const string _outlineRef = "_Outline_Map";
    private const string _maskRef = "_HairMask";

    private int _weightMapRefID;
    private int _gradientMapRefID;
    private int _outlineRefID;
    private int _maskRefID;
    #endregion References

    private MaterialPropertyBlock _mpb;

    private void Awake() {
        _weightMapRefID = Shader.PropertyToID(_weightMapRef);
        _gradientMapRefID = Shader.PropertyToID(_gradientMapRef);
        _outlineRefID = Shader.PropertyToID(_outlineRef);
        _maskRefID = Shader.PropertyToID(_maskRef);

        _mpb = new();
        renderer.GetPropertyBlock(_mpb);
    }

    // Update is called once per frame
    void Update()
    {
        _mpb.SetTexture(_weightMapRefID, weightTex);
        _mpb.SetTexture(_gradientMapRefID, gradientTex);
        _mpb.SetTexture(_outlineRefID, outlineTex);
        _mpb.SetTexture(_maskRefID, maskTex);
        renderer.SetPropertyBlock(_mpb);
    }
}
