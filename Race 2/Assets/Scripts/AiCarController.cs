using UnityEngine;

public class AiCarController : MonoBehaviour
{
    public float maxSpeed = 20f;
    public float maxSteeringAngle = 45f;
    public float checkpointRadius = 5f;
    public float avoidRadius = 10f;
    public float avoidAngle = 45f;
    public LayerMask obstacleLayer;

    private Rigidbody rb;
    private int currentCheckpointIndex = 0;
    
    #region NOS
    public float nosSpeed = 1;
    public float nosMultiplier = 1.2f;
    public bool Nossable;
    public float nosTimer = 1f;
    #endregion

    public int chkpntCurr = 0, chkpntPrev = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        if(Nossable) {
            nosSpeed = nosMultiplier;
            nosTimer -= Time.deltaTime;

            if(nosTimer <= 0) {
                nosSpeed = 1;
                nosTimer = 1f;
                Nossable = false;
            }
        }
    }

    private void FixedUpdate()
    {
        // Check if the car has reached the current checkpoint
        if (Vector3.Distance(transform.position, GameManager.instance.checkpoints[currentCheckpointIndex].transform.position) < checkpointRadius)
        {
            // If the car has reached the last checkpoint, go back to the first one
            if (currentCheckpointIndex == GameManager.instance.checkpoints.Length - 1)
            {
                currentCheckpointIndex = 0;
            }
            else
            {
                currentCheckpointIndex++;
            }
        }

        // Calculate the direction to the next checkpoint
        Vector3 targetDirection = (GameManager.instance.checkpoints[currentCheckpointIndex].transform.position - transform.position).normalized;

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
        rb.AddForce(transform.forward * maxSpeed * nosSpeed);

        // Apply the steering torque
        rb.AddTorque(transform.up * steeringAngle * nosSpeed);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("NitroBox")) {
            // Debug.Log("NitroBox");
            // other.gameObject.SetActive(false);
            Nossable = true;
        }

        if(other.CompareTag("Checkpoint")) {
            Debug.Log("Checkpoint");
            chkpntPrev = chkpntCurr; //pass new checkpoint, current becomes prev
            chkpntCurr = other.GetComponent<Checkpoint>().index; //current is replacedwith new checkpoint index
        }
    }
}
