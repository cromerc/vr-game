using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public float DropSpeed = 3.0f;
    public float RiseSpeed = 2.0f;

    private Vector3 _originalPosition;
    private Vector3 _targetPosition;
    private bool falling = false;

    // Start is called before the first frame update
    void Start()
    {
        _originalPosition = transform.position;
        _targetPosition = _originalPosition;
        _targetPosition.y -= 4.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (falling)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPosition, DropSpeed * Time.deltaTime);

            if (transform.position == _targetPosition)
            {
                falling = false;
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, _originalPosition, RiseSpeed * Time.deltaTime);

            if (transform.position == _originalPosition)
            {
                falling = true;
            }
        }
    }
}
