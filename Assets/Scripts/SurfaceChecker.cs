using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class SurfaceChecker : MonoBehaviour
{
    private bool canPlace = false;
    public Pose placementPose;
    public ARRaycastManager arRaycastManager;
    public GameObject targetIndicator;
    public Camera xrCamera;
    public GameObject bowlingBallPrefab;
    public GameObject bowlingPinPrefab;
    public GameObject speedBoosterPrefab;
    public DefineAction actionMap;
    public GameObject choiceUI;
    public GameObject gameManager;
    public GameObject placingObjectButton;
    public GameObject startButton;
    public GameObject menuHolder;
    public GameState readGameState;
    public Difficulty readDifficultyState;
    public GameObject deleteButton;
    public GameObject countdown;
    [SerializeField] TextMeshProUGUI countdownText;
    private float countdownTime;
    private bool isTimerRunning = false;
    public GameObject Easy;
    public GameObject Medium;
    public GameObject Hard;
    public GameObject throwButton;
    public GameObject strafeButton;
    public GameObject gameover;
    [SerializeField] TextMeshProUGUI overtext;
    public GameObject restartButton;
    private List<GameObject> additionalSpawnedObject;
    private bool bowlingBallSpawned = false;
    private bool bowlingPinSpawned = false;
    private bool bowlingBallRespawned = false;
    private bool isRespawning = false; // Add this line
    const int maxBoosters = 2;
    public AudioSource kuru;
    // Start is called before the first frame update
    // [SerializeField] TextMeshProUGUI scoretext;
    // private BowlingBallScript bowlingBallScript;

    void Start()
    {
        actionMap = new DefineAction();
        actionMap.Enable();
        choiceUI = bowlingPinPrefab;
        additionalSpawnedObject = new List<GameObject>();
        throwButton.SetActive(false);
        strafeButton.SetActive(false);
        countdown.SetActive(false);
        gameover.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        readGameState = gameManager.GetComponent<Game>().gameState;
        if (readGameState == GameState.ARScaning)
        {
            UpdatePlacementPose();
            UpdateTargetIndicator();
            if (actionMap.ARPlacingState.PlacingObject.triggered && canPlace)
            {
                PlaceObject();
            }
            // else if (actionMap.ARPlacingState.ChangingObject.triggered)
            // {
            //     changePlacingObject();
            // }
            if (actionMap.ARPlacingState.DeleteObject.triggered)
            {
                deletePlacingObject();
            }

            // if (bowlingPinSpawned == true)
            // {
            //     Debug.Log("Distance: " + Vector3.Distance(gameManager.GetComponent<Game>().bowlingPinPosition, placementPose.position));
            // }
            if (Vector3.Distance(gameManager.GetComponent<Game>().bowlingPinPosition, placementPose.position) > 5f && bowlingPinSpawned == true)
            {
                placingObjectButton.SetActive(true);
            }
            else if (bowlingPinSpawned == false)
            {
                placingObjectButton.SetActive(true);
            }
            else if (bowlingPinSpawned == true && bowlingBallSpawned == false)
            {
                placingObjectButton.SetActive(false);
            }
            else if (bowlingPinSpawned == true && bowlingBallSpawned == true && additionalSpawnedObject.Count() < maxBoosters + 1)
            {
                placingObjectButton.SetActive(true);
            }
            else
            {
                placingObjectButton.SetActive(false);
            }
            if (bowlingPinSpawned == true && bowlingBallSpawned == true && additionalSpawnedObject.Count() >= maxBoosters + 1)
            {
                startButton.SetActive(true);
                if (actionMap.ARPlacingState.StartGame.triggered)
                {
                    StartGameAct();
                }
            }
            else
            {
                startButton.SetActive(false);
            }
        }
        if (readGameState == GameState.Gameplay && additionalSpawnedObject[0].activeSelf == false && bowlingBallRespawned == false)
        {
            StartCoroutine(RespawnBall());
        }

        if (readGameState == GameState.Gameplay)
        {
            Difficulties();
        }
        if (readGameState == GameState.GameOver)
        {
            RestartGame();
        }
    }
    IEnumerator RespawnBall()
    {
        bowlingBallRespawned = true;
        yield return new WaitForSeconds(3);
        Debug.Log("Respawning Ball");
        Destroy(additionalSpawnedObject[0]);
        additionalSpawnedObject[0] = Instantiate(bowlingBallPrefab, additionalSpawnedObject[0].GetComponent<BowlingBallScript>().initialBowlingBallPosition,
        Quaternion.LookRotation(-additionalSpawnedObject[0].GetComponent<BowlingBallScript>().lookDirection));
        bowlingBallRespawned = false;

    }
    void StartGameAct()
    {
        gameManager.GetComponent<Game>().gameState = GameState.Gameplay;
        // Debug.Log("Start Game Action Triggered");  // Add this line
        // Make menuHolder visible
        startButton.SetActive(false);
        menuHolder.SetActive(true);
        deleteButton.SetActive(false);
        targetIndicator.SetActive(false);
        placingObjectButton.SetActive(false);
        gameover.SetActive(false);
        // additionalSpawnedObject[0].GetComponent<Rigidbody>().isKinematic = false;
        // Activate the bowling pins
        // Debug.Log("Bowling Pin Set Activated");
    }

    void UpdatePlacementPose()
    {
        Vector3 screenCenter = xrCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        List<ARRaycastHit> hits = new List<ARRaycastHit>();
        // Debug.log("Screen Center: " + screenCenter)
        arRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);
        canPlace = hits.Count > 0;
        if (canPlace)
        {
            // placePose uses the first hit in the list
            placementPose = hits[0].pose;
            // Debug.Log("Placement Pose: " + placementPose);
            // Debug.Log("Hits: " + hits);
            // Debug.Log("Hits Count: " + hits.Count);
            var cameraForward = xrCamera.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }
    void UpdateTargetIndicator()
    {
        if (canPlace && readGameState == GameState.ARScaning)
        {
            targetIndicator.SetActive(true);
            targetIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            targetIndicator.SetActive(false);
        }
    }
    void PlaceObject()
    {
        if (choiceUI == bowlingBallPrefab && !bowlingBallSpawned)
        {
            bowlingBallSpawned = true;
            additionalSpawnedObject.Add(Instantiate(choiceUI, placementPose.position, placementPose.rotation));
            // additionalSpawnedObject[additionalSpawnedObject.Count() - 1].GetComponent<Rigidbody>().isKinematic = true;
            choiceUI = speedBoosterPrefab;
        }
        else if (choiceUI == bowlingPinPrefab && !bowlingPinSpawned)
        {
            bowlingPinSpawned = true;
            gameManager.GetComponent<Game>().SpawnBowlingPinSet(placementPose.position);
            choiceUI = bowlingBallPrefab;
        }
        else if (choiceUI == speedBoosterPrefab && additionalSpawnedObject.Count() < maxBoosters + 1)
        {
            additionalSpawnedObject.Add(Instantiate(choiceUI, placementPose.position, placementPose.rotation));
            // additionalSpawnedObject[additionalSpawnedObject.Count() - 1].GetComponent<Rigidbody>().isKinematic = true;
            // choiceUI = bowlingPinPrefab;
        }
        else
        {
            // Debug.Log("No Object Placed");
        }

    }
    void deletePlacingObject()
    {
        for (int i = 0; i < additionalSpawnedObject.Count(); i++)
        {
            Destroy(additionalSpawnedObject[i]);
            Debug.Log("Additional Object Destroyed: " + i);
        }
        bowlingBallSpawned = false;
        gameManager.GetComponent<Game>().DeleteBowlingPinSet();
        bowlingPinSpawned = false;
        additionalSpawnedObject.Clear();
        choiceUI = bowlingPinPrefab;
    }

    // Function to start timer depends on which difficulties was selected
    void Difficulties()
    {
        gameManager.GetComponent<Game>().gameState = GameState.Gameplay;
        //readDifficultyState = gameManager.GetComponent<Game>().difficulties;
        if (actionMap.ARSelectingState.Easy.triggered)
        {
            readDifficultyState = Difficulty.Easy;
            Debug.Log("Easy Gameplay");
            TimerSet();
            menuHolder.SetActive(false);
            countdown.SetActive(true);
            throwButton.SetActive(true);
            strafeButton.SetActive(true);

        }
        else if (actionMap.ARSelectingState.Medium.triggered)
        {
            readDifficultyState = Difficulty.Medium;
            Debug.Log("Medium Gameplay");
            TimerSet();
            menuHolder.SetActive(false);
            countdown.SetActive(true);
            throwButton.SetActive(true);
            strafeButton.SetActive(true);
        }
        else if (actionMap.ARSelectingState.Hard.triggered)
        {
            readDifficultyState = Difficulty.Hard;
            Debug.Log("Hard Gameplay");
            TimerSet();
            menuHolder.SetActive(false);
            countdown.SetActive(true);
            throwButton.SetActive(true);
            strafeButton.SetActive(true);
        }
    }

    // Function to set timer based on difficulty above
    void TimerSet()
    {
        // float countdownTime;

        switch (readDifficultyState)
        {
            case Difficulty.Easy:
                countdownTime = 4 * 60; // 4 minutes
                break;

            case Difficulty.Medium:
                countdownTime = 3 * 60; // 3 minutes
                break;

            case Difficulty.Hard:
                countdownTime = 90; // 1.5 minutes
                break;

            default:
                countdownTime = 0;
                Debug.LogError("Invalid difficulty state!");
                break;
        }

        Debug.Log($"Setting timer countdown to {countdownTime} seconds for {readDifficultyState} difficulty.");
        UpdateTimerText(countdownTime);
        StartTimer();
    }
    void StartTimer()
    {
        if (!isTimerRunning)
        {
            StartCoroutine(CountdownTimer());
        }
    }

    // Insert Score function

    IEnumerator CountdownTimer()
    {
        isTimerRunning = true;

        while (countdownTime > 0)
        {
            yield return new WaitForSeconds(1f);
            countdownTime--;

            UpdateTimerText(countdownTime);
        }

        // Timer reached zero, you can add any actions here
        Debug.Log("Timer reached zero!");

        isTimerRunning = false;

        gameManager.GetComponent<Game>().gameState = GameState.GameOver;
        throwButton.SetActive(false);
        strafeButton.SetActive(false);
        gameover.SetActive(true);
        kuru.Play();
        // ShowScore();
    }
    void UpdateTimerText(float time)
    {
        // Assuming timerText is a Text component on a UI element
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // void ShowScore()
    // {
    //     // Access coin value from CoinCollecting script
    //     int bowlingScore = bowlingBallScript.Score;

    //     scoreText.text = "Coins Collected: " + bowlingScore.ToString();
    //     scoreText.enabled = true;
    // }

    void RestartGame()
    {
        if (actionMap.ARSelectingState.Restart.triggered)
        {
            Debug.Log("Game has restarted successfully");
            // Hide Game Over UI
            gameover.SetActive(false);
            // Restart the game as needed (reset variables, objects, etc.)
            countdown.SetActive(false);

            // Ensure additional spawned objects are destroyed
            deletePlacingObject();

            deleteButton.SetActive(true);

            readGameState = gameManager.GetComponent<Game>().gameState;
            gameManager.GetComponent<Game>().gameState = GameState.ARScaning;

        }
    }
}
