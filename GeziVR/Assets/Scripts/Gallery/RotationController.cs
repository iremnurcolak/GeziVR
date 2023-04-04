using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationController : MonoBehaviour
{
    float rotSpeed = 10f;
    //for development in editor
    //float rotSpeed = 20f;
    public void OnMouseDrag()
    {
        //for development in editor
        float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;
        float rotY = Input.GetAxis("Mouse Y") * rotSpeed * Mathf.Deg2Rad;

        //for mobile
        //float rotX = Input.GetTouch(0).deltaPosition.x * rotSpeed * Mathf.Deg2Rad;
        //float rotY = Input.GetTouch(0).deltaPosition.y * rotSpeed * Mathf.Deg2Rad;

        transform.RotateAround(Vector3.up, -rotX);
        transform.RotateAround(Vector3.right, rotY);
    }
  
}
