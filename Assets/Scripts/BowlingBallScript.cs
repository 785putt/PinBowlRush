using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BowlingBallScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject gameManager;
    public DefineAction actionMap;
    public GameState currentGameState;
    public Rigidbody bowlingBallRigidBody;
    public GameObject mainCamera;
    [SerializeField] private float forcePowerConstant = 50f;
    public bool isThrown = false;
    public bool strafed = false;
    private bool isReset = false;
    private Vector2 joystickValue;
    private Vector2 strafeValue;
    private List<float> joyStickValues = new List<float>();
    private List<float> strafeValues = new List<float>();
    public Vector3 initialBowlingBallPosition;
    public Vector3 lookDirection;
    public AudioSource BowlRolling;
    void Start()
    {
        actionMap = new DefineAction();
        actionMap.Enable();
        gameManager = GameObject.Find("AR Manager");
        mainCamera = GameObject.Find("Main Camera");
        bowlingBallRigidBody = GetComponent<Rigidbody>();
        bowlingBallRigidBody.mass = 1f;
        // bowlingBallRigidBody.freezeRotation = true;
        // Face the ball towards the camera
        lookDirection = transform.position - mainCamera.transform.position;
        // Rotate the object to face away from the camera
        transform.rotation = Quaternion.LookRotation(-lookDirection);
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.25f, transform.position.z);
        // Initial ball position
        initialBowlingBallPosition = transform.position;
        Debug.Log("Initial Ball Position: " + initialBowlingBallPosition);
        currentGameState = gameManager.GetComponent<Game>().gameState;
        bowlingBallRigidBody.isKinematic = false;
        // if (currentGameState == GameState.Gameplay)
        // {
        //     bowlingBallRigidBody.isKinematic = false;
        // }
        // else
        // {
        //     bowlingBallRigidBody.isKinematic = true;
        // }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentGameState = gameManager.GetComponent<Game>().gameState;
        // Debug.Log("GameState: " + currentGameState);
        // Debug.Log("Joystick Value: " + joystickValue);
        joystickValue = actionMap.BowlingBall.Throwing.ReadValue<Vector2>();
        if (currentGameState == GameState.Gameplay)
        {
            // Move the ball up slightly on the y axis
            if (joystickValue == Vector2.zero && isThrown == false)
            {

                try
                {
                    bowlingBallRigidBody.AddForce(this.transform.forward * (joyStickValues.Average() * forcePowerConstant), ForceMode.Impulse);
                    isThrown = true;
                    joyStickValues = new List<float>();
                    StartCoroutine(respawnBowlingBall(10));
                    // Debug.Log("Ball Thrown");
                }
                catch
                {
                    // Debug.Log("No value to throw");
                }
            }
            else
            {
                if (joystickValue != Vector2.zero)
                {
                    // Debug.Log("Throwing the ball");
                    // joyStickValues.Add(joystickValue.x);
                    joyStickValues.Add(joystickValue.y);
                }
            }
            if (isThrown == true && strafed == false)
            {
                strafeValue = actionMap.BowlingBall.Strafing.ReadValue<Vector2>();
                // Debug.Log("Strafe Value: " + strafeValue);
                if (strafeValue != Vector2.zero)
                {
                    strafeValues.Add(strafeValue.x);
                }
                try
                {
                    BowlRolling.Play();
                    bowlingBallRigidBody.AddRelativeForce(Vector3.left * (forcePowerConstant / 2) * strafeValues.Average(), ForceMode.Impulse);
                    strafed = true;
                    strafeValues = new List<float>();
                }
                catch
                {
                    // Debug.Log("No value to strafe");

                }
            }
        }
        else
        {
            // bowlingBallRigidBody.isKinematic = true;

        }
        // ehe

    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bowling Pin")
        {
            StartCoroutine(respawnBowlingBall(5));
        }
    }
    IEnumerator respawnBowlingBall(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
    }

}
