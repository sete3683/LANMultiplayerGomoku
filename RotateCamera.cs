using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public Transform focus;

    float _degree;
    float _radius;
    float _speed = 7f;

    void Awake()
    {
        _degree = 0;
        _radius = Mathf.Abs(transform.position.z);
    }

    void Update()
    {
        CircularMotion();
    }

    void CircularMotion()
    {
        _degree += Time.deltaTime * _speed;

        if (_degree < 360)
        {
            float cameraRadian = Mathf.Deg2Rad * _degree;
            float cameraX = _radius * Mathf.Sin(cameraRadian);
            float cameraZ = _radius * Mathf.Cos(cameraRadian);

            transform.position = focus.position + new Vector3(cameraX, transform.position.y, cameraZ);
            transform.LookAt(focus);
        }
        else
        {
            _degree = 0;
        }
    }
}
