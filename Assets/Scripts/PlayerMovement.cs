using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{ 
    public int playerSpeed;
    public Transform vrCamera;
    public float toggleAngle;
    public bool moveForward;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


        if (vrCamera.eulerAngles.x >= toggleAngle && vrCamera.eulerAngles.x < 90.00f)
        {
            moveForward = true;
        }

        else
        {
            moveForward = false;
        }
        if (moveForward)
        {
            var forward = Camera.main.transform.forward;
            forward.y = 0;
            transform.position = transform.position + forward * playerSpeed * Time.deltaTime;
        }
    }
}
