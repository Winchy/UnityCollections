using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
public class StarSkyMesh : MonoBehaviour {

	public bool optimizeMesh;
	public bool makeNewInstance;

	public int VertexCount = 3000;

	private Vector3[] vertices;
	private Vector3[] normals;
	private Vector2[] uvs;

	private Mesh mesh;

	void Start() {
		
	}

	[ContextMenu("Generate Mesh")]
	void GenerateMesh() {
		mesh = new Mesh ();

		vertices = new Vector3[VertexCount];
		normals = new Vector3[VertexCount];
		uvs = new Vector2[VertexCount];

		Vector3 one = Vector3.one/2;
		for (int i = 0; i < VertexCount; i++) {
			vertices [i] = Vector3.Normalize(new Vector3 (Random.value, Random.value, Random.value) - one); //Star Positions
			normals [i] = Vector3.Normalize(new Vector3 (Random.value, Random.value, Random.value) - one);	//Star Velocity
			uvs[i] = new Vector2(Random.value, Random.value); //uv[0]: radius range; uv[1]: color range
		}

		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uvs;

		MeshFilter mf = GetComponent<MeshFilter> ();
		//GenerateMesh ();
		mf.mesh = mesh;

		SaveMesh ();
	}

	[ContextMenu("Save Mesh")]
	void SaveMesh() {
		#if UNITY_EDITOR
		if (mesh == null) {
			Debug.Log ("No mesh specified");
			return;
		}

		string path = EditorUtility.SaveFilePanel ("Save Separate Mesh Asset", "Asset", name, "asset");
		if (string.IsNullOrEmpty (path)) {
			return;
		}

		path = FileUtil.GetProjectRelativePath (path);

		Mesh meshToSave = makeNewInstance ? Object.Instantiate (mesh) : mesh;

		if (optimizeMesh) {
			;
		}

		AssetDatabase.CreateAsset (meshToSave, path);
		AssetDatabase.SaveAssets ();
		#endif
	}
}
