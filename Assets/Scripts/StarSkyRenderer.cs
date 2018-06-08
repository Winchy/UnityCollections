using UnityEngine;
using System.Collections;

public class StarSkyRenderer : MonoBehaviour {

	public Mesh StarMesh;
	public Shader StarSkyShader;
	private Material StarSkyMaterial;
	public Color Color0 = Color.white;
	public Color Color1 = Color.white;
	public Texture2D Sprite;
	public float minRadius;
	public float maxRadius;
	public float moveSpeed = 10.0f;

	private Vector3[] vertices;
	private Vector3[] normals;
	private Vector2[] uvs;

	private ComputeBuffer vertexBuffer;
	private ComputeBuffer pulseBuffer;
	private ComputeBuffer uvBuffer;

	// Use this for initialization
	void Start () {
		StarSkyMaterial = new Material (StarSkyShader);

		vertices = StarMesh.vertices;
		normals = StarMesh.normals;
		uvs = StarMesh.uv;

		vertexBuffer = new ComputeBuffer (vertices.Length, sizeof(float) * 3);
		pulseBuffer = new ComputeBuffer (normals.Length, sizeof(float) * 3);
		uvBuffer = new ComputeBuffer (uvs.Length, sizeof(float) * 2);
		vertexBuffer.SetData (vertices);
		pulseBuffer.SetData (normals);
		uvBuffer.SetData (uvs);

		StarSkyMaterial.SetBuffer ("starPoses", vertexBuffer);
		StarSkyMaterial.SetBuffer ("starPulses", pulseBuffer);
		StarSkyMaterial.SetBuffer ("starColors", uvBuffer);


		/*
		Debug.Log (vertices.Length);

		for (int i = 0; i < vertices.Length; i++) {
			Debug.Log (vertices [i]);
		}
		*/
	}

	void OnDestroy() {
		vertexBuffer.Release ();
		pulseBuffer.Release ();
		uvBuffer.Release ();
	}
	
	void OnRenderObject() {
		if (StarMesh == null)
			return;

		StarSkyMaterial.SetPass (0);
		StarSkyMaterial.SetTexture ("_MainTex", Sprite);
		StarSkyMaterial.SetColor ("_Color0", Color0);
		StarSkyMaterial.SetColor ("_Color1", Color1);
		StarSkyMaterial.SetFloat ("minRadius", minRadius);
		StarSkyMaterial.SetFloat ("maxRadius", maxRadius);
		StarSkyMaterial.SetFloat ("speed", moveSpeed);
		StarSkyMaterial.SetMatrix ("worldMatrix", transform.localToWorldMatrix);

		Graphics.DrawProcedural (MeshTopology.Points, vertices.Length);
	}
}
