/*
 * Moves an icicle downwards uniformly.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardMove : MonoBehaviour
{
    public float speed;
    public float minY;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector2(this.transform.position.x, this.transform.position.y - (speed * Time.deltaTime));

        if (this.transform.position.y < minY)
            GameObject.Destroy(this.gameObject);
    }
}
