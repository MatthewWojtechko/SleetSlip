/*
 * Moves this gameobject left and right according to the mouse position. Movement is 
 * constrained at minimum and maximum positions.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float maxRight = 3.65f;
    public float minLeft = -3.65f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float xMouseScreenPos = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;

        if (xMouseScreenPos < minLeft)
            xMouseScreenPos = minLeft;
        else if (xMouseScreenPos > maxRight)
            xMouseScreenPos = maxRight;

        this.transform.position = new Vector2(xMouseScreenPos, this.transform.position.y);
    }
}
