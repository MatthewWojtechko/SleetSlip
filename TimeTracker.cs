/*
 * Continually updates a time-displaying Text component, and provides a static function
 * for formatting a given number of seconds into something more presentable.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeTracker : MonoBehaviour
{
    public Text scoreDisplay;
    public GameLoop gameLoopScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Update text
        if (!gameLoopScript.playerHit)
            scoreDisplay.text = formatTime(Time.realtimeSinceStartup - gameLoopScript.gameStartTime);
    }

    // Formats a given number of seconds into one of these:
    // mm:ss.d
    // When there are no minutes yet, that part is omitted. The other digits are always present, irregardless.
    public static string formatTime(float timeInSeconds)
    {
        string finalString;

        // Get the proper minute and second digits
        int numMinutes = (int)(timeInSeconds / 60);
        float numSeconds = Mathf.Round(((timeInSeconds) % 60) * 10) / 10;

        // Format the number of seconds to always have a decimal place
        string numSecondsString;
        if (numSeconds % 1 == 0) // Is this a whole number?
            numSecondsString = numSeconds + ".0";
        else
            numSecondsString = numSeconds + "";
        if (numSeconds < 10)
            numSecondsString = 0 + numSecondsString;

        // If there are no minutes, do not display the minutes
        if (numMinutes > 0)
            finalString = numMinutes + ":" + numSecondsString;
        else
            finalString = "" + numSecondsString;

        return finalString;
    }
}
