using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody sphereRB;
    private float moveInput;
    private float turnInput;
    public float fwSpeed;
    public float revSpeed;
    public float turnSpeed;

    public bool isPlayer;

    #region NOS
    public float nosSpeed = 1;
    public float nosMultiplier = 1.2f;
    public bool Nossable;
    public float nosTimer = 1f;
    #endregion

    #region groundcheck
    public bool isGrounded;
    public LayerMask groundLayer;
    #endregion

    #region Race Stuff
    public int chkpntCurr = 0, chkpntPrev = 0;
    public int currentLap, racePosition;
    public float distanceTraveled;
    #endregion

    #region AICar
    public float maxSpeed = 20f;
    public float maxSteeringAngle = 45f;
    public float checkpointRadius = 5f;
    public float avoidRadius = 10f;
    public float avoidAngle = 45f;
    public LayerMask obstacleLayer;

    private Rigidbody rb;
    #endregion

    public float oilMultiplier = 1;
    public bool Oiled;
    public float oilTimer;

    private void Start() {
        if(isPlayer) {
            sphereRB.transform.parent = null;
        }
        else {
            rb = GetComponent<Rigidbody>();
        }
        
    }

    private void Update() {
        if(isPlayer) {
            moveInput = Input.GetAxisRaw("Vertical");
            turnInput = Input.GetAxisRaw("Horizontal");


            moveInput *= moveInput > 0 ? fwSpeed : revSpeed;

            transform.position = sphereRB.transform.position;

            float newRotation = turnInput * nosSpeed * turnSpeed * Time.deltaTime * Input.GetAxisRaw("Vertical");
            transform.Rotate(0, newRotation, 0, Space.World);

            RaycastHit hit;
            isGrounded = Physics.Raycast(transform.position, -transform.up, out hit, groundLayer);
        }
        

        if(Nossable) {
            nosSpeed = nosMultiplier;
            nosTimer -= Time.fixedDeltaTime;

            if(nosTimer <= 0) {
                nosSpeed = 1;
                nosTimer = 1f;
                Nossable = false;
            }
        }

        if(Oiled) {
            oilMultiplier = 0.2f;
            oilTimer -= Time.fixedDeltaTime;

            if(oilTimer <= 0) {
                oilMultiplier = 1f;
                oilTimer = 1f;
                Oiled = false;
            }
        }
    }

    private void FixedUpdate() {
        if(isPlayer) {
            sphereRB.AddForce(transform.forward * moveInput * nosSpeed * oilMultiplier, ForceMode.Acceleration);
        }
        #region AICAR STUFF
        else {
            // Check if the car has reached the current checkpoint
            if (Vector3.Distance(transform.position, GameManager.instance.checkpoints[chkpntCurr].transform.position) < checkpointRadius)
            {
                // If the car has reached the last checkpoint, go back to the first one
                if (chkpntCurr == GameManager.instance.checkpoints.Length - 1)
                {
                    chkpntCurr = 0;
                }
                else
                {
                    chkpntCurr++;
                }
            }

            // Calculate the direction to the next checkpoint
            Vector3 targetDirection = (GameManager.instance.checkpoints[chkpntCurr].transform.position - transform.position).normalized;

            // Check for obstacles in front of the car
            RaycastHit hitInfo;
            bool obstacleDetected = Physics.SphereCast(transform.position, avoidRadius, transform.forward, out hitInfo, avoidRadius, obstacleLayer);

            // Calculate the steering angle
            float steeringAngle = 0f;
            if (obstacleDetected)
            {
                // Calculate the direction away from the obstacle
                Vector3 awayFromObstacle = Vector3.Reflect(hitInfo.normal, transform.forward);

                // Calculate the angle between the target direction and the direction away from the obstacle
                float angleToObstacle = Vector3.SignedAngle(targetDirection, awayFromObstacle, transform.up);

                // If the angle to the obstacle is within the avoid angle, steer away from the obstacle
                if (Mathf.Abs(angleToObstacle) < avoidAngle)
                {
                    steeringAngle = Mathf.Sign(angleToObstacle) * maxSteeringAngle;
                }
            }
            else
            {
                // Calculate the rotation towards the next checkpoint
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection, transform.up);

                // Smoothly rotate the car towards the next checkpoint
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxSteeringAngle * Time.fixedDeltaTime);
            }

            // Apply the driving force
            rb.AddForce(transform.forward * maxSpeed * nosSpeed * oilMultiplier);

            // Apply the steering torque
            rb.AddTorque(transform.up * steeringAngle * nosSpeed);
            
            // Check for collisions with other cars
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, avoidRadius, obstacleLayer);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].gameObject != gameObject)
                {
                    // Calculate the direction away from the other car
                    Vector3 awayFromOtherCar = transform.position - hitColliders[i].transform.position;
        
                    // Apply a force to the car in the opposite direction of the collision
                    rb.AddForce(awayFromOtherCar.normalized * maxSpeed * Time.fixedDeltaTime);
                }
            }
        }
        #endregion
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("NitroBox")) {
            // Debug.Log("NitroBox");
            // other.gameObject.SetActive(false);
            Nossable = true;
        }

        if(other.CompareTag("Checkpoint")) {

            if(isPlayer) {
                if(
                    chkpntCurr == GameManager.instance.checkpoints[GameManager.instance.checkpoints.Length - 1].index &&
                    chkpntPrev == GameManager.instance.checkpoints[GameManager.instance.checkpoints.Length - 2].index
                ) {
                    currentLap++;
                }
            }
            else {
                if(
                    chkpntCurr == 0 &&
                    chkpntPrev == GameManager.instance.checkpoints[GameManager.instance.checkpoints.Length - 1].index
                ) {
                    currentLap++;
                }
            }

            Debug.Log("Checkpoint");
            chkpntPrev = chkpntCurr; //pass new checkpoint, current becomes prev
            chkpntCurr = other.GetComponent<Checkpoint>().index; //current is replacedwith new checkpoint index
        }

        if(other.CompareTag("Oil")) {
            Oiled = true;
        }
    }
}
