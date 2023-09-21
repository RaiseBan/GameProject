using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 2f;

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(transform.up * this.speed * Time.deltaTime);
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(transform.right * this.speed * Time.deltaTime * (-1));
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(transform.up * this.speed * Time.deltaTime * (-1));
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(transform.right * this.speed * Time.deltaTime);
        }
    }
}
