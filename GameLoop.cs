/*
 * The most complicated script in this game. It controls the opening screen, the gameplay, and the ending 
 * screen, and keeps track which state it is currently in.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameState { INTRO, GAMEPLAY, PLAYAGAIN };

public class GameLoop : MonoBehaviour
{
    // Stuff to display during certain states
    // The items in the three arrays always appear in the state corresponding to the variable name.
    // The other items are displayed conditionally.
    public GameObject[] openingScreen;
    public GameObject playerCharacter;
    public Text highScoreDisplay;
    public GameObject[] gameplayScreen;
    public GameObject[] endingScreen;
    public Text yourScoreDisplay;
    public GameObject newHighScoreDisplay;
    public GameObject turboNotice;

    // Minimum score to display the turboNotice text
    public float turboNoticeThreshold = 80; 
    
    // Keep track of what is going on
    public GameState currentState = GameState.INTRO;
    public bool playerHit = false;
    public float gameStartTime = 0;
    public bool turboMode = false; // extra fast mode
    private float currentPlayerScore;
    private bool endgameInitiated = false;   // Has the lose scene started?
    private bool stateReady = false;         // Whether everything necessary for the current state has already been done

    // Vairables needed for the scene when the player loses
    public SpriteRenderer playerSR;
    public Sprite livingS;
    public Sprite deadS;
    public AudioSource deathSound;
    public ParticleSystem deathParticles;
    public float deathTime = 2f;

    public AudioSource gameBGM;
    public AudioSource menuBGM;
    public AudioSource victorySFX;

    // The particles and colliders on the penguin correspond to whether the gameplay is regular or turbo.
    public BoxCollider2D[] regPenguinColliders;
    public BoxCollider2D[] turboPenguinColliders;
    public GameObject regPenTrail;
    public GameObject turboPenTrail;

    // Special values for turbo mode
    public float turboMusicSpeed = 1.25f;
    public float turboDeathTime = 4f;
    public float turboTimeScale = 2.5f;

    // Effect to play at certain time intervals
    public ParticleSystem[] timeScoreParticles;
    private int timeScoreParticlesIndex = 0;  // Keeps track of which effect is next


    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject X in gameplayScreen) // Remove all game objects form gameplay, just to be safe
        {
            X.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Control which state we are in
        switch (currentState)
        {
            case GameState.INTRO:
                introState();
                break;

            case GameState.GAMEPLAY:
                gameplayState();
                break;

            case GameState.PLAYAGAIN:
                playAgainState();
                break;
        }
    }

    // The first screen the player sees. Invites the player to click and displays the current high score.
    void introState()
    {
        // Do this when we first enter this state
        if (!stateReady)
        {
            foreach (GameObject X in openingScreen) // display all needed game objects
            {
                X.SetActive(true);
            }
            highScoreDisplay.text = "High Score: " + TimeTracker.formatTime(PlayerPrefs.GetFloat("HighScore")); // display high score in proper format

            stateReady = true;
            menuBGM.Play();
        }

        // These are what get us out of the state
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            killIntroState();
            turboMode = false;
        }
        else if (Input.GetKey(KeyCode.T))
        {
            killIntroState();
            turboMode = true;
        }
    }

    // This controls the gameplay.
    void gameplayState()
    {
        // Do this when we first enter this state
        if (!stateReady)
        {
            foreach (GameObject X in gameplayScreen) // display all needed game objects
            {
                X.SetActive(true);
            }
            timeScoreParticlesIndex = 0;
            playerSR.sprite = livingS;
            playerHit = false;
            gameStartTime = Time.realtimeSinceStartup;
            if (turboMode)
                gameBGM.pitch = turboMusicSpeed;
            else
                gameBGM.pitch = 1;
            gameBGM.Play();

            

            // Set proper game speed, player particles, and player hurtboxes, depending on whether secret turbo mode is active.
            if (turboMode)
            {
                Time.timeScale = turboTimeScale;
                regPenTrail.SetActive(false);
                turboPenTrail.SetActive(true);
                foreach (BoxCollider2D X in regPenguinColliders)
                {
                    X.enabled = false;
                }
                foreach (BoxCollider2D X in turboPenguinColliders)
                {
                    X.enabled = true;
                }
            }
            else
            {
                Time.timeScale = 1;
                regPenTrail.SetActive(true);
                turboPenTrail.SetActive(false);
                foreach (BoxCollider2D X in regPenguinColliders)
                {
                    X.enabled = true;
                }
                foreach (BoxCollider2D X in turboPenguinColliders)
                {
                    X.enabled = false;
                }
            }

            stateReady = true;
        }

        // These are what get us out of the state
        if (playerHit && !endgameInitiated)
        {
            currentPlayerScore = Time.realtimeSinceStartup - gameStartTime;
            StartCoroutine(penguinDeath());
            endgameInitiated = true;
        }

        // Play encouraging particles if/when the time passes a milestone
        if (!playerHit)
        {
            if (Time.realtimeSinceStartup - gameStartTime >= 30 && timeScoreParticlesIndex == 0)
            {
                timeScoreParticles[timeScoreParticlesIndex++].Play();
                victorySFX.Play();
            }
            else if (Time.realtimeSinceStartup - gameStartTime >= 60 && timeScoreParticlesIndex == 1)
            {
                timeScoreParticles[timeScoreParticlesIndex++].Play();
                victorySFX.Play();
            }
            else if (Time.realtimeSinceStartup - gameStartTime >= 90 && timeScoreParticlesIndex == 2)
            {
                timeScoreParticles[timeScoreParticlesIndex++].Play();
                victorySFX.Play();
            }
            else if (Time.realtimeSinceStartup - gameStartTime >= 120 && timeScoreParticlesIndex == 3)
            {
                timeScoreParticles[timeScoreParticlesIndex++].Play();
                victorySFX.Play();
            }
            else if (Time.realtimeSinceStartup - gameStartTime >= 150 && timeScoreParticlesIndex == 4)
            {
                timeScoreParticles[timeScoreParticlesIndex++].Play();
                victorySFX.Play();
            }
            else if (Time.realtimeSinceStartup - gameStartTime >= 180 && timeScoreParticlesIndex == 5)
            {
                timeScoreParticles[timeScoreParticlesIndex++].Play();
                victorySFX.Play();
            }
            else if (Time.realtimeSinceStartup - gameStartTime >= 240 && timeScoreParticlesIndex == 6)
            {
                timeScoreParticles[timeScoreParticlesIndex++].Play();
                victorySFX.Play();
            }
        }
    }

    // This is the screen that plays after every round.
    void playAgainState()
    {
        // Do this when we first enter this state.
        if (!stateReady)
        {
            foreach (GameObject X in endingScreen) // display all needed game objects
            {
                X.SetActive(true);
            }

            // Display player score, high score, and a notice if they beat it
            yourScoreDisplay.text = "Your Score: " + TimeTracker.formatTime(currentPlayerScore); // display player Score in proper format
            highScoreDisplay.gameObject.SetActive(true);
            if (currentPlayerScore > PlayerPrefs.GetFloat("HighScore"))
            {
                newHighScoreDisplay.SetActive(true);
                highScoreDisplay.text = "Old High Score: " + TimeTracker.formatTime(PlayerPrefs.GetFloat("HighScore")); // display high score in proper format
                PlayerPrefs.SetFloat("HighScore", currentPlayerScore);
            }
            else
            {
                highScoreDisplay.text = "Current High Score: " + TimeTracker.formatTime(PlayerPrefs.GetFloat("HighScore")); // display high score in proper format
            }

            // If you do good, be informed of turbo mode
            if (currentPlayerScore > turboNoticeThreshold)
            {
                turboNotice.SetActive(true);
            }

            menuBGM.Play();
            stateReady = true;
        }

        // These are what get us out of this state.
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
        {
            killPlayAgainState();
            turboMode = false;
        }
        else if (Input.GetKey(KeyCode.T))
        {
            killPlayAgainState();
            turboMode = true;
        }
    }

    // Returns how many seconds have elapsed this round.
    public float getPlayTime()
    {
        return Time.realtimeSinceStartup - gameStartTime;
    }

    // Plays the "scene" for when the player loses. Sets a boolean to inform when it's over and 
    // therefore time for the gameover state.
    IEnumerator penguinDeath()
    {
        // Stop the music, and do the ice block freeze effect
        gameBGM.Stop();
        playerSR.sprite = deadS;
        deathSound.Play();
        deathParticles.Play();

        // Wait a few seconds so the player can see
        if (turboMode)
            yield return new WaitForSeconds(turboDeathTime);
        else
            yield return new WaitForSeconds(deathTime);

        currentState = GameState.PLAYAGAIN;
        foreach (GameObject X in gameplayScreen) // remove all game objects for this state
        {
            X.SetActive(false);
        }

        endgameInitiated = false;
        stateReady = false;
    }

    // The actions that should be taken as we leave the gameover state
    private void killPlayAgainState()
    {
        foreach (GameObject X in endingScreen) // remove all game objects for this old state
        {
            X.SetActive(false);
        }
        highScoreDisplay.gameObject.SetActive(false);
        newHighScoreDisplay.SetActive(false);
        turboNotice.SetActive(false);

        currentState = GameState.GAMEPLAY;
        menuBGM.Stop();
        stateReady = false;
    }

    // The actions that should be taken as we leave the intro screen
    private void killIntroState()
    {
        foreach (GameObject X in openingScreen) // remove all game objects for this old state
        {
            X.SetActive(false);
        }

        currentState = GameState.GAMEPLAY;
        stateReady = false;
        menuBGM.Stop();
    }

}
