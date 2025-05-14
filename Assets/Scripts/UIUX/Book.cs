using System;
using UnityEngine;
using UnityEngine.InputSystem;


public class Book : MonoBehaviour
{
    public bool bookopened = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.B))
        {
            bookopened = true;
            Debug.Log("Book opened" + bookopened);
        }
        else
        {
            bookopened = false;
            Debug.Log("Book closed" + bookopened);
        }
    }
}
