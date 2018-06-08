using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDepth : MonoBehaviour {

    void Awake()
    {
        Camera.main.depthTextureMode = DepthTextureMode.Depth;
    }
    
}
