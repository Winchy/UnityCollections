using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUInstancingExperiments : MonoBehaviour {

    public Mesh meshToDraw;

    public Material material;

    public GameObject proto;

    public int size = 10;

    public float interval = 2.0f;

    private Matrix4x4[] matrices;

    Vector3 pos = Vector3.zero;
    public Vector3 scale = Vector3.one;
    Quaternion rotation = Quaternion.identity;
    float halfS;
    List<Vector4> colors;

    MaterialPropertyBlock props;

    private void Start()
    {
        props = new MaterialPropertyBlock();
        colors = new List<Vector4>(size * size * size);
        matrices = new Matrix4x4[size * size * size];
    }

    // Update is called once per frame
	void Update () {
        DrawInstanced();
	}

    void DrawInstanced()
    {
        
        halfS = (size - 1.0f) / 2.0f;
        colors.Clear();
        float colorDiff = 1.0f / (float)size;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                for (int k = 0; k < size; k++)
                {
                    pos.x = (i - halfS) * interval;
                    pos.y = (j - halfS) * interval;
                    pos.z = (k - halfS) * interval;
                    var r = Quaternion.AngleAxis(Time.time * 100.0f, new Vector3(i, j, k));
                    int idx = i * size * size + j * size + k;
                    matrices[idx] = Matrix4x4.TRS(pos, r, scale);
                    colors.Add(new Vector4(colorDiff * (float)i, colorDiff * (float)j, colorDiff * (float)k, 1.0f));
                    
                }
            }
        }
        props.SetVectorArray("_Color", colors);
        Graphics.DrawMeshInstanced(meshToDraw, 0, material, matrices, matrices.Length, props);
    }
}
