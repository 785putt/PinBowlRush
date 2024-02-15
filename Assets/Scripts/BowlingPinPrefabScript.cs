using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowlingPinPrefabScript : MonoBehaviour
{
    // Start is called before the first frame update
    // public static BowlingPinPrefabScript instance; // Singleton instance
    public GameObject bowlingPinPrefab;
    public GameObject gameManager;
    public GameState readGameState;
    public AudioSource bowling2;
    void Start()
    {
        gameManager = GameObject.Find("AR Manager");
        readGameState = gameManager.GetComponent<Game>().gameState;
        if (readGameState == GameState.Gameplay)
        {
            bowlingPinPrefab.GetComponent<Rigidbody>().isKinematic = false;
        }
        else
        {
            bowlingPinPrefab.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        readGameState = gameManager.GetComponent<Game>().gameState;
        if (readGameState == GameState.Gameplay)
        {
            bowlingPinPrefab.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        else
        {

        }
    }
    IEnumerator OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bowling Ball" || collision.gameObject.tag == "Bowling Pin")
        {
            bowling2.Play();
            yield return new WaitForSeconds(5);
            bowlingPinPrefab.SetActive(false);
            gameManager.GetComponent<Game>().score++;
        }
    }
}
