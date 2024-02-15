using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRunner : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject bowlingBallPrefab;
    private List<GameObject> addtionalSpawnedObjects;
    // Start is called before the first frame update
    void Start()
    {
        addtionalSpawnedObjects = new List<GameObject>();
        Vector3 spawnPosition = new Vector3(0, 0.065f, 2);
        gameManager.GetComponent<Game>().SpawnBowlingPinSet(spawnPosition);
        gameManager.GetComponent<Game>().ActivateBowlingPinSet();
        addtionalSpawnedObjects.Add(Instantiate(bowlingBallPrefab, new Vector3(0, 0.125f + 0.065f, -4), Quaternion.identity));
        addtionalSpawnedObjects[0].GetComponent<Rigidbody>().isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

}
