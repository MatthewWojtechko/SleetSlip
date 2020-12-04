/*
 * This program spawns icicles at an accelrating rate at the top of the screen. 
 * Icicles spawn in "trails" of random lengths.
 * A trail is when an icicle purposely spawns close to the last icicle spawned. 
 * When spawning the first icicle in a trail, this program takes into account the player's position. 
 * It decides whether to spawn the new trail in the vicinity depending on a given probability.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardsSpawn : MonoBehaviour
{

    public float currentWaitTime = 0.3f;     // How long to wait between spawns
    public float easyWaitTime = 0.3f;        // Spawn rate at beginning of game
    public float hardWaitTime = 0.1f;        // The fastest the spawn rate gets
    public float timeForMaxDifficulty = 150; // Number of seconds until spawn rate should be fastest 

    public float minX = -3.65f;              // Max and min X coordinates
    public float maxX = 3.65f;
    public float yPosition = 5.62f;          // How high to spawn

    public int minTrailNumber = 1;           // Range of how long trails can be
    public int maxTrailNumber = 5;
    public float minTrailDistance = -4;      // How much horizontal space between one icicle and the one previously spawned in the trail
    public float maxTrailDistance = 4;

    public GameObject icicle;
    public Transform playerTrans;
    public GameLoop gameLoopScript;

    // These coordinates divide the horizontal space into three thirds.
    // A left third, a middle third, and a right third.
    public float leftMinX = -3.65f;
    public float leftMaxX = -1.22f;
    public float midMinX = -1f;
    public float midMaxX = 1f;
    public float rightMinX = 1.22f;
    public float rightMaxX = 3.65f;

    // Probability the next trail be in the third the player is currently in.
    public float playerBias = 0.8f;


    private float targetTrailLength = 0;     // How long the current trail should get
    private float currentTrailLength = 0;    // How long it is right now
    private float lastIcicleSpawnX = 0;      // Where did the last icicle spawn (the X coordinate)
    private bool isSpawnTime = true;         // Should we spawn right now?


    void OnEnable()
    {
        isSpawnTime = true; // Reset for a new game
    }

    void OnDisable()
    {
        // Hide all the icicles when the game is over
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Increase difficulty
        if (Time.realtimeSinceStartup - gameLoopScript.gameStartTime > timeForMaxDifficulty) // When it's time, set the spawn rate to the hardest rate.
            currentWaitTime = hardWaitTime;
        else // As the duration of playtime reaches the max difficulty time, linearly decrease the spawn wait time
            currentWaitTime = easyWaitTime - ((gameLoopScript.getPlayTime() / timeForMaxDifficulty) * (easyWaitTime - hardWaitTime));  

        // Control spawning
        if (isSpawnTime)
        {
            if (currentTrailLength == targetTrailLength) // Reset the trail count if need be
            {
                beginNewTrail();
            }
            spawn();
            StartCoroutine(waitSpawn());
        }
    }

    // Set variables for a new trail
    void beginNewTrail()
    {
        targetTrailLength = Random.Range(minTrailNumber, maxTrailNumber);
        currentTrailLength = 0;
    }

    // Spawn a new icicle. If the first icicle in a trail, determine whether to spawn somewhere in the player's vicinity
    // or not. If not the first in the trail, then spawn a random horizontal offest away from the last icicle in the trail.
    void spawn()
    {
        float xPosition;        // New icicle's position


        if (currentTrailLength == 0) // Is this the first icicle in a new trail?
        {
            // Whether to spawn in player's vicinity, or away from them
            bool spawnNearPlayer = false;
            if (Random.Range(0f, 1f) < playerBias)
                spawnNearPlayer = true;

            // Figures out which area the player is in, then spawn either there or away from there, depending on the above boolean.
            if (playerTrans.position.x < leftMaxX)                  // Is player at left?
            {
                if (spawnNearPlayer)
                    xPosition = Random.Range(leftMinX, leftMaxX);
                else
                    xPosition = Random.Range(rightMinX, rightMaxX);
            }
            else if (playerTrans.position.x > rightMinX)            // Then, is palyer at right?
            {
                if (spawnNearPlayer)
                    xPosition = Random.Range(rightMinX, rightMaxX);
                else
                    xPosition = Random.Range(leftMinX, leftMaxX);
            }
            else                                                    // Then, player is at center.
            {
                if (spawnNearPlayer)
                    xPosition = Random.Range(leftMaxX, rightMinX);
                else // Spawn away from player at center, so choose either left or right, randomly
                {
                    // The edge of the center is determined by the mid variable in this case, as these variables should be more extreme than the rightMax and leftMin variables, therefore keeping the center extra open.
                    if (Random.Range(0f, 1f) < 0.5f) 
                        xPosition = Random.Range(leftMinX, midMinX);
                    else 
                        xPosition = Random.Range(midMaxX, rightMaxX);
                }
            }
        }
        else  // This is not the first icicle in a new trail
        {
            xPosition = lastIcicleSpawnX + Random.Range(minTrailDistance, maxTrailDistance); // Offset from the last spawn position
        }

        // Instantiate new icicle
        GameObject newIcicle = GameObject.Instantiate(icicle);
        newIcicle.transform.position = new Vector2(xPosition, yPosition);
        newIcicle.transform.parent = this.transform; // Child the icicle to this game object

        // Keep track
        lastIcicleSpawnX = newIcicle.transform.position.x;
        currentTrailLength++;
    }

    // Sets the spawnTime flag to false, preventing a spawn right now. When the proper time elapses,
    // it sets it to true, to allow a spawn now.
    IEnumerator waitSpawn()
    {
        isSpawnTime = false;
        yield return new WaitForSeconds(currentWaitTime);
        isSpawnTime = true;
    }
}
