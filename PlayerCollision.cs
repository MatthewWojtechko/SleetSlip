/*
 * Simply informs the GameLoop script that the player was hit.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerCollision : MonoBehaviour
{
    public GameLoop gameLoopScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D()
    {
        if (!gameLoopScript.playerHit)
            gameLoopScript.playerHit = true;
    }
}
