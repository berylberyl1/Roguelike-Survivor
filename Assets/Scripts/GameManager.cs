using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // Singleton

    public enum GameState // Game states
    {
        Gameplay,
        Paused,
        GameOver,
        LevelUp
    }

    public GameState currentState; // Current game state

    public GameState previousState; // Previous game state

    [Header("Screens")]
    public GameObject pauseScreen; // Pause screen UI
    public GameObject resultScreen; // Result screen UI
    public GameObject levelUpScreen; // Level up screen UI

    [Header("Current Stat Displays")]
    public Text currentHealthDisplay;
    public Text currentRecoveryDisplay;
    public Text currentMoveSpeedDisplay;
    public Text currentMightDisplay;
    public Text currentProjectileSpeedDisplay;
    public Text currentMagnetDisplay;

    [Header("Result Screen Displays")]
    public Image chosenCharacterImage;
    public Text chosenCharacterName;
    public Text levelReachedDisplay;
    public Text timeSurvivedDisplay;
    public List<Image> chosenWeaponsUI = new List<Image>(6);
    public List<Image> chosenPassiveItemsUI = new List<Image>(6);

    [Header("Stopwatch")]
    public float timeLimit;
    public float stopwatchTime;
    public Text stopwatchDisplay;

    public bool isGameOver = false; // Is the game over? 

    public bool choosingUpgrade = false; // Is the player choosing upgrades?

    public GameObject playerObject; // Player

    void Awake() 
    {
        if(instance == null) // If instance is null
        {
            instance = this; // Set instance to this
        }
        else 
        {
            Debug.LogWarning( this + "already exists, destroying object!");
            Destroy(gameObject);
        }

        DisableScreens(); // Disable all screens
    }

    void Update() 
    {
        switch(currentState)
        // Switch between game states
        {
            case GameState.Gameplay:
                // Do gameplay stuff
                CheckForPauseAndResume(); // Check for pause and resume
                UpdateStopwatch(); // Update stopwatch
                break;

            case GameState.Paused:
                // Do paused stuff
                CheckForPauseAndResume(); // Check for pause and resume
                break;

            case GameState.GameOver:
                // Do game over stuff
                if(!isGameOver) // If the game is not over
                {
                    isGameOver = true; // Set game over to true
                    Time.timeScale = 0f; // Stop game
                    Debug.Log("Game Over");
                    DisplayResults(); // Display results
                }
                break;

            case GameState.LevelUp:
                // Do level up stuff
                if(!choosingUpgrade) // If the player is not choosing upgrades
                {
                    choosingUpgrade = true; // Set choosing upgrade to true
                    Time.timeScale = 0f; // Stop game
                    Debug.Log("Level Up");
                    levelUpScreen.SetActive(true); // Show level up screen
                }
                break;

            default:
                Debug.LogWarning("Invalid game state");
                break;
        }
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState; // Set current state to new state
    }

    public void PauseGame()
    {
        previousState = currentState; // Set previous state to current state
        ChangeState(GameState.Paused); // Set current state to paused
        Time.timeScale = 0f; // Stop time
        pauseScreen.SetActive(true); // Show pause screen
        Debug.Log("Game paused");
    }

    public void ResumeGame()
    {
        if(currentState == GameState.Paused) // If the game is paused
        {
            ChangeState(previousState); // Set current state to previous state
            Time.timeScale = 1f; // Start time
            pauseScreen.SetActive(false); // Hide pause screen
            Debug.Log("Game resumed");
        }
    }

    void CheckForPauseAndResume() // Check for pause and resume
    {
        if(Input.GetKeyDown(KeyCode.Escape)) // If escape key is pressed
        {
            if(currentState == GameState.Paused) // If the game is in paused state
            {
                ResumeGame(); // Resume the game
            }
            else if(currentState == GameState.Gameplay) // If the game is in gameplay state
            {
                PauseGame(); // Pause the game
            }
        }
    }

    void DisableScreens()
    {
        pauseScreen.SetActive(false); // Hide pause screen
        resultScreen.SetActive(false); // Hide result screen
        levelUpScreen.SetActive(false); // Hide level up screen
    }

    public void GameOver()
    {
        timeSurvivedDisplay.text = stopwatchDisplay.text; // Set time survived display
        ChangeState(GameState.GameOver); // Set current state to game over
    }

    void DisplayResults()
    {
        resultScreen.SetActive(true); // Show result screen
    }

    public void AssignChosenCharacterUI(CharacterScriptableObject chosenCharacterData)
    {
        chosenCharacterImage.sprite = chosenCharacterData.Icon; // Set chosen character icon
        chosenCharacterName.text = chosenCharacterData.name; // Set chosen character name
    }

    public void AssignLevelReachedUI(int levelReachedData)
    {
        levelReachedDisplay.text = levelReachedData.ToString(); // Set level reached
    }

    public void AssignChosenWeaponsAndPassiveItemsUI( List<Image> chosenWeaponsData, List<Image> chosenPassiveItemsData)
    {
        if( chosenWeaponsData.Count != chosenWeaponsUI.Count || chosenPassiveItemsData.Count != chosenPassiveItemsUI.Count)
        {
            Debug.LogWarning("Chosen weapons and passive items data count does not match UI count");
            return;
        }
        
        for(int i = 0; i < chosenWeaponsData.Count; i++) // Set chosen weapons
        {
            if(chosenWeaponsData[i].sprite) // If the chosen weapon has a sprite
            {
                chosenWeaponsUI[i].enabled = true; // Enable the chosen weapon UI
                chosenWeaponsUI[i].sprite = chosenWeaponsData[i].sprite; // Set the chosen weapon UI sprite
            }
            else
            {
                chosenWeaponsUI[i].enabled = false; // Disable the chosen weapon UI
            }
        }

        for(int i = 0; i < chosenPassiveItemsData.Count; i++) // Set chosen passive items
        {
            if(chosenPassiveItemsData[i].sprite) // If the chosen passive item has a sprite
            {
                chosenPassiveItemsUI[i].enabled = true; // Enable the chosen passive item UI
                chosenPassiveItemsUI[i].sprite = chosenPassiveItemsData[i].sprite; // Set the chosen passive item UI sprite
            }
            else
            {
                chosenPassiveItemsUI[i].enabled = false; // Disable the chosen passive item UI
            }
        }
    }

    void UpdateStopwatch()
    {
        stopwatchTime += Time.deltaTime; // Set stopwatch time

        UpdateStopwatchDisplay(); // Update stopwatch display

        if(stopwatchTime >= timeLimit) // If stopwatch time is greater than or equal to time limit
        {
            playerObject.SendMessage("Kill"); // Send message to player object to kill player
        }
    }

    public void UpdateStopwatchDisplay()
    {
        int minutes = Mathf.FloorToInt(stopwatchTime / 60f); // Set minutes
        int seconds = Mathf.FloorToInt(stopwatchTime % 60f); // Set seconds

        stopwatchDisplay.text = string.Format("{0:00}:{1:00}", minutes, seconds); // Set stopwatch display
    }

    public void StartLevelUp()
    {
        ChangeState(GameState.LevelUp); // Set current state to level up
        playerObject.SendMessage("RemoveAndApplyUpgrades"); // Send message to player object to apply upgrade options
    }

    public void EndLevelUp()
    {
        choosingUpgrade = false; // Set choosing upgrade to false
        Time.timeScale = 1f; // Start time
        levelUpScreen.SetActive(false); // Hide level up screen
        ChangeState(GameState.Gameplay); // Set current state to gameplay
        Debug.Log("Level Up ended");
    }
}
