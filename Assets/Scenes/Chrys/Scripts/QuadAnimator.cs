using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadAnimator : MonoBehaviour
{
    [SerializeField] private MeshRenderer renderer;

    //warning: these will be changed in the animator
    [SerializeField] private Texture mainTex;

    #region References
    private const string _mainTexRef = "_MainTex";

    private int _mainTexID;
    #endregion Referencess

    private MaterialPropertyBlock _mpb;

    private void Awake() {
        _mainTexID = Shader.PropertyToID(_mainTexRef);
        
        _mpb = new();
        renderer.GetPropertyBlock(_mpb);
    }

    // Update is called once per frame
    void Update()
    {
        _mpb.SetTexture(_mainTexID, mainTex);
        renderer.SetPropertyBlock(_mpb);
    }
}
