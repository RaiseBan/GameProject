using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Объект, за которым будет следить камера (Player)

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void LateUpdate()
    {
        Vector3 temp = transform.position;
        temp.x = target.transform.position.x;
        temp.y = target.transform.position.y;
        transform.position = temp;
    }
}