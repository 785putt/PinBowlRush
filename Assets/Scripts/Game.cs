using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Game : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject bowlingPinPrefab;
    public GameObject[] bowlingPins;
    // public Vector3 targetPosition;
    public DefineAction actionMap;
    public Vector3 translationTemplateVector;
    public GameObject gameManager;
    public GameState gameState;
    public Vector3 bowlingPinPosition;
    public List<Vector3> bowlingRespawnPosition;
    public GameObject bowlingBall;
    public int score = 0;
    public GameObject scorecount;
    [SerializeField] TextMeshProUGUI scoretext;
    public int count = 0;
    public AudioSource Alldown;

    void Start()
    {
        actionMap = new DefineAction();
        actionMap.Enable();
        bowlingPins = new GameObject[10];
        // gameState = GameState.ARScaning;
        bowlingRespawnPosition = new List<Vector3>();
        scorecount.SetActive(false);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        gameState = gameManager.GetComponent<Game>().gameState;
        if (bowlingPins[9] != null && gameState == GameState.ARScaning)
        {
            scorecount.SetActive(false);
            // Reset the score
            score = 0;
            bowlingPinPosition = bowlingPins[9].transform.position;
        }
        if (gameState == GameState.Gameplay)
        {
            scorecount.SetActive(true);
            // Debug.Log(score);
            scoretext.text = "Score: " + score.ToString();

            // Check if all bowling pins are inactive
            bool allPinsInactive = bowlingPins.All(pin => !pin.activeSelf);

            if (allPinsInactive)
            {
                Debug.Log("All pins are inactive and no longer visible in the game now.");
                StartCoroutine(ExampleCoroutine());
                RespawnBowlingPinSet();
            }
        }

        // if (gameState == GameState.GameOver)
        // {
        //     Debug.Log("REEEEEE");
        // }
    }
    // Function to spawn a set of bowling pins, Used only in the scanning phase as object pooling is used in the game phase
    public void SpawnBowlingPinSet(Vector3 targetPosition)
    {
        float translationConstant = -0.125f - 0.065f;
        int scalingFactor = 0;
        float altitudeOffset = 0f;
        int startingIndex = 0;
        float translationFactor = translationConstant / 2;
        for (int i = 0; i < 4; i++)
        {
            translationTemplateVector = new Vector3(translationConstant * i, altitudeOffset, 0) + targetPosition;
            bowlingPins[startingIndex] = Instantiate(bowlingPinPrefab, translationTemplateVector, Quaternion.identity);
            bowlingPins[startingIndex].GetComponent<Rigidbody>().isKinematic = true;
            bowlingRespawnPosition.Add(bowlingPins[startingIndex].transform.position);
            startingIndex++;
        }
        scalingFactor = 1;
        for (int i = 1; i < 4; i++)
        {
            translationTemplateVector = new Vector3((translationConstant * i) - (scalingFactor * translationFactor), altitudeOffset, -scalingFactor * translationConstant) + targetPosition;
            bowlingPins[startingIndex] = Instantiate(bowlingPinPrefab, translationTemplateVector, Quaternion.identity);
            bowlingPins[startingIndex].GetComponent<Rigidbody>().isKinematic = true;
            bowlingRespawnPosition.Add(bowlingPins[startingIndex].transform.position);
            startingIndex++;
        }
        scalingFactor = 2;
        for (int i = 2; i < 4; i++)
        {
            translationTemplateVector = new Vector3((translationConstant * i) - (scalingFactor * translationFactor), altitudeOffset, -scalingFactor * translationConstant) + targetPosition;
            bowlingPins[startingIndex] = Instantiate(bowlingPinPrefab, translationTemplateVector, Quaternion.identity);
            bowlingPins[startingIndex].GetComponent<Rigidbody>().isKinematic = true;
            bowlingRespawnPosition.Add(bowlingPins[startingIndex].transform.position);
            startingIndex++;
        }
        scalingFactor = 3;
        translationTemplateVector = new Vector3(scalingFactor * translationFactor, altitudeOffset, -scalingFactor * translationConstant) + targetPosition;
        bowlingPins[9] = Instantiate(bowlingPinPrefab, translationTemplateVector, Quaternion.identity);
        bowlingPins[9].GetComponent<Rigidbody>().isKinematic = true;
        bowlingRespawnPosition.Add(bowlingPins[startingIndex].transform.position);
    }
    // Function to activate the bowling pin set Rigidbody, used right after the scanning phase
    public void ActivateBowlingPinSet()
    {
        for (int i = 0; i < 10; i++)
        {
            bowlingPins[i].GetComponent<Rigidbody>().isKinematic = false;
        }
    }
    // Function to delete the bowling pin set, used only in the scanning phase and when the game was reset.
    public void DeleteBowlingPinSet()
    {
        for (int i = 0; i < bowlingPins.Count(); i++)
        {
            Destroy(bowlingPins[i]);
            // Debug.Log("Bowling Pin Destroyed: " + i);
        }
    }
    // Function to use object pooling to respawn the bowling pins
    public void RespawnBowlingPinSet()
    {
        for (int i = 0; i < bowlingPins.Count(); i++)
        {
            bowlingPins[i].SetActive(true);
            bowlingPins[i].transform.SetPositionAndRotation(bowlingRespawnPosition[i], Quaternion.identity);
        }
        Alldown.Play();
    }
    // Function to wait for 5 seconds
    IEnumerator ExampleCoroutine()
    {
        yield return new WaitForSeconds(5);
    }
}
