using UnityEngine;
//using UnityEditor;
using System.Collections;

public class StarGenerator : MonoBehaviour {

	public float minRadius = 0.0f;
	public float maxRadius = 1.0f;

	public Texture2D pulseTex;
	public Texture2D posTex;

	public ComputeShader computeShader;
	public Material starMaterial;

	public ComputeBuffer outputBuffer;

	const int groupSize = 8;
	const int threads = 8 * 8 * 8 * 8;

	int kernel;

	private Mesh mesh;

	struct data {
		public Vector4 pulse;
		public Vector4 pos;
	};

	void ReleaseBuffers() {
		outputBuffer.Release ();
	}

	// Use this for initialization
	void Start () {

		kernel = computeShader.FindKernel ("CSMain");

		outputBuffer = new ComputeBuffer (threads, sizeof(float) * 8);

		computeShader.SetBuffer (kernel, "outputBuffer", outputBuffer);
		computeShader.SetTexture (kernel, "PulseTex", pulseTex);
		computeShader.SetTexture (kernel, "PosTex", posTex);
		computeShader.SetFloat ("minRadius", minRadius);
		computeShader.SetFloat ("maxRadius", maxRadius);

		computeShader.Dispatch (kernel, groupSize, groupSize, 1);

		data[] stars = new data[threads];
		outputBuffer.GetData (stars);
		for (int i = 0; i < stars.Length; i++) {
			//Debug.Log (stars [i].pos);
		}
	}

	void OnRenderObject() {
		starMaterial.SetPass (0);
		starMaterial.SetBuffer ("buf_Points", outputBuffer);

		Graphics.DrawProcedural (MeshTopology.Points, threads);
	}

	void OnDestroy() {
		outputBuffer.Release ();
	}
}
