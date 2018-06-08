using UnityEngine;
using System.Collections;

public class SelfRotate : MonoBehaviour {

	public float RotateSpeed = 50.0f;
	public float normalL = 0.1f;

	private Vector3[] vertices;
	private Vector3[] normals;

	// Use this for initialization
	void Start () {
		vertices = GetComponent<MeshFilter> ().mesh.vertices;
		normals = GetComponent<MeshFilter> ().mesh.normals;

		vertices = new Vector3[0];

		for (int i = 0; i < vertices.Length; i++) {
			vertices [i] = (transform.localToWorldMatrix * vertices [i]);
			vertices [i] += transform.position;
			normals [i] = Vector3.Normalize(transform.localToWorldMatrix * normals [i]);
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3 (0, RotateSpeed * Time.deltaTime, 0));
	}

	void OnDrawGizmos() {
		if (vertices == null)
			return;
		for (int i = 0; i < vertices.Length; i++) {
			Gizmos.DrawLine ( vertices [i], normals [i] * normalL + vertices [i]);
		}
	}
}
