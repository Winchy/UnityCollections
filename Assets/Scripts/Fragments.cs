using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fragments : MonoBehaviour {

    Mesh mesh;
    Material material;

    List<GameObject> gos;

	// Use this for initialization
	void Start () {
        gos = new List<GameObject>();

        mesh = GetComponent<MeshFilter>().mesh;
        material = GetComponent<MeshRenderer>().material;

        var vertices = mesh.vertices;
        var uvs = mesh.uv;
        var normals = mesh.normals;
        var triangles = mesh.triangles;

        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            Mesh m = new Mesh();
            
            Vector3[] vs = new Vector3[3];
            vs[0] = vertices[triangles[i]];
            vs[1] = vertices[triangles[i + 1]];
            vs[2] = vertices[triangles[i + 2]];
            m.vertices = vs;

            Vector2[] u = new Vector2[3];
            u[0] = uvs[triangles[i]];
            u[1] = uvs[triangles[i + 1]];
            u[2] = uvs[triangles[i + 2]];
            m.uv = u;

            Vector3[] ns = new Vector3[3];
            ns[0] = normals[triangles[i]];
            ns[1] = normals[triangles[i + 1]];
            ns[2] = normals[triangles[i + 2]];
            m.normals = ns;

            int[] t = { 0, 1, 2 };
            m.triangles = t;

            GameObject go = new GameObject();
            MeshFilter newMeshFilter = go.AddComponent<MeshFilter>();
            newMeshFilter.mesh = m;
            
            go.AddComponent<MeshRenderer>().material = material;
            gos.Add(go);
        }

        GetComponent<MeshRenderer>().enabled = false;

        Scater();
	}

    void Scater()
    {
        for (int i = 0; i < gos.Count; ++i)
        {
            var go = gos[i];
            go.transform.position = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            go.transform.eulerAngles = new Vector3(Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f));
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Scater();
        }
    }
	
	void FixedUpdate()
    {
        for (int i = 0; i < gos.Count; ++i)
        {
            var go = gos[i];
            go.transform.position = Vector3.Lerp(go.transform.position, Vector3.zero, Time.fixedDeltaTime * 1.0f);
            go.transform.rotation = Quaternion.Slerp(go.transform.rotation, Quaternion.identity, Time.deltaTime * 1.0f);
        }
    }
}
