using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinMove : MonoBehaviour {

    public float h = 1.0f;
    public float f = 1.0f;
    public int direction = 0;

    Vector3 startPosition;

	// Use this for initialization
	void Start () {
        startPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        transform.position = startPosition + h * Mathf.Sin(Time.time * f) * (direction == 0? transform.right : transform.up);
    }
}
