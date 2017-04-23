using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateScript : MonoBehaviour
{
    public float RotateSpeed = 10;
    public Transform RotateAround;

	void Update ()
    {
        transform.RotateAround(RotateAround.position, Vector3.up, RotateSpeed * Time.deltaTime);
	}
}
