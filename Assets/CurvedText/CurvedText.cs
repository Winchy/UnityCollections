using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CurvedText : Text
{
	public float radius = 0.5f;
    public float wrapAngle = 360.0f;
    public float scaleFactor = 100.0f;
    public bool faceInside = true;
    public bool cylinder = true;

    float actualRadius;
    Vector3 radiusVector;

    private float circumference
	{
		get
		{
			if(_radius != radius || _scaleFactor != scaleFactor)
			{
				_circumference = 2.0f*Mathf.PI*radius * scaleFactor;
				_radius = radius;
				_scaleFactor = scaleFactor;
                actualRadius = radius * scaleFactor;
                radiusVector = actualRadius * Vector3.forward;
            }

			return _circumference;
		}
	}
	private float _radius = -1;
	private float _scaleFactor = -1;
	private float _circumference = -1;

#if UNITY_EDITOR
    void OnValidate()
	{
        base.OnValidate();
        if (radius <= 0) radius = 0.001f;
        if (scaleFactor <= 0) scaleFactor = 0.001f;
    }
#endif

    protected override void OnPopulateMesh(VertexHelper vh)
	{	
		base.OnPopulateMesh(vh);

        List<UIVertex> stream = new List<UIVertex>();
        vh.GetUIVertexStream(stream);
		for (int i = 0; i < stream.Count; i++)
		{
			UIVertex v = stream[i];

            if (cylinder)
            {
                //v.position.x * 360.0f / circumference为radiusVector绕y轴旋转的角度
                if (faceInside)
                {
                    var offset = Quaternion.Euler(0.0f, v.position.x * 360.0f / circumference, 0) * radiusVector;
                    v.position = new Vector3(offset.x, v.position.y, offset.z);
                    v.position -= radiusVector;
                }
                else
                {
                    var offset = Quaternion.Euler(0.0f, v.position.x * 360.0f / circumference, 0) * radiusVector;
                    v.position = new Vector3(offset.x, v.position.y, -offset.z);
                    v.position += radiusVector;
                }
            }
            else
            {
                float percentCircumference = v.position.x / circumference;
                Vector3 offset = Quaternion.Euler(0.0f, 0.0f, -percentCircumference * 360.0f) * Vector3.up;
                v.position = offset * radius * scaleFactor + offset * v.position.y;
                v.position += Vector3.down * radius * scaleFactor;
            }
            stream[i] = v;
		}

        vh.Clear();
        vh.AddUIVertexTriangleStream(stream);
	}

    void Update()
	{
		if(radius <= 0.0f)
		{
			radius = 0.001f;
		}
		
		rectTransform.sizeDelta = new Vector2(circumference*wrapAngle/360.0f,rectTransform.sizeDelta.y);
	}
}
