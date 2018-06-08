using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {

    public float moveSpeed = 1.0f;

    public float rotateSpeed = 120.0f;

    private float currentMoveSpeed;

    private Vector3 currentPosition;

    enum MouseState
    {
        
    }
    

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void LateUpdate()
    {
        if (!Input.GetMouseButton(1))
            return;

        if (currentPosition != transform.position)
        {
            currentMoveSpeed += 5.0f * Time.deltaTime;
            currentPosition = transform.position;
        }

        float x = Input.GetAxis("Horizontal") * currentMoveSpeed * Time.deltaTime;
        float y = Input.GetAxis("Vertical") * currentMoveSpeed * Time.deltaTime;

        if (x == 0.0f && y == 0.0f)
        {
            currentMoveSpeed = moveSpeed;
            currentPosition = transform.position;
        }
        else
        {
            var pos = transform.position;
            pos += x * transform.right;
            pos += y * transform.forward;

            transform.position = pos;
        }

        float ydiff = Input.GetAxis("Jump") * currentMoveSpeed * Time.deltaTime;
        var posy = transform.position;
        posy.y += ydiff;
        transform.position = posy;

        x = Input.GetAxisRaw("Mouse X") * rotateSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * x, Space.World);

        y = Input.GetAxisRaw("Mouse Y") * rotateSpeed * Time.deltaTime;
        transform.Rotate(-Vector3.right * y, Space.Self);
    }
}
