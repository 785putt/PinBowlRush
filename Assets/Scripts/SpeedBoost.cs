using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBooster : MonoBehaviour
{
    // Start is called before the first frame update
    // public static SpeedBooster instance; // Singleton instance
    public GameObject speedBoosterPrefab;
    private GameObject[] bowlingBallPrefabs;
    public Rigidbody bowlingBallRigidBody;
    public GameObject gameManager;
    public GameState readGameState;
    private bool speedIncreased = false;
    [SerializeField] private float forcePowerConstant = 20f;

    void Start()
    {
        gameManager = GameObject.Find("AR Manager");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        try
        {
            bowlingBallPrefabs = GameObject.FindGameObjectsWithTag("Bowling Ball");
            bowlingBallRigidBody = bowlingBallPrefabs[0].GetComponent<Rigidbody>();
        }
        catch
        {
            // Debug.Log("Error");
        }
        if (speedIncreased == true)
        {
            bowlingBallRigidBody.AddForce((speedBoosterPrefab.transform.forward * forcePowerConstant), ForceMode.Force);
            speedIncreased = false;
            Debug.Log("Speed");
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Speed Boost" || collision.gameObject.tag == "Bowling Ball")
        {
            speedIncreased = true;
            // Debug.Log("SPEED INCREASED");
        }
    }
}