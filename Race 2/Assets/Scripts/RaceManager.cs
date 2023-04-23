using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    private Checkpoint[] checkpoints; // list of checkpoint transforms
    public float lapDistance = 100f; // distance of one lap
    public float maxDistance = 10000f; // maximum distance allowed before lap reset

    public List<CarController> cars; // list of all CarController scripts in the scene


    // public static RaceManager instance;

    void Start()
    {
        checkpoints = GameManager.instance.checkpoints;

        // populate the cars list with all CarController scripts in the scene
        cars = new List<CarController>();
        foreach (GameObject carObject in GameObject.FindGameObjectsWithTag("Car"))
        {
            CarController carController = carObject.GetComponent<CarController>();
            if (carController != null)
            {
                cars.Add(carController);
            }
        }
    }

    void Update()
    {
        // loop through all cars and update their race position
        for (int i = 0; i < cars.Count; i++)
        {
            // calculate the distance from the car to the next checkpoint
            float distanceToNextCheckpoint = Vector3.Distance(cars[i].transform.position, checkpoints[cars[i].chkpntCurr].transform.position);

            // check if the car has passed the current checkpoint
            if (distanceToNextCheckpoint < 5f)
            {
                // cars[i].chkpntCurr++;
                if (cars[i].chkpntCurr >= checkpoints.Length)
                {
                    // cars[i].chkpntCurr = 0;
                    // cars[i].currentLap++;

                    // check if the car has finished the race
                    // if (cars[i].currentLap > 3)
                    // {
                    //     // TODO: handle finished cars
                    //     Debug.Log(cars[i] + " has finished!");
                    //     continue;
                    // }
                }
            }

            if (cars[i].currentLap >= 3)
            {
                // TODO: handle finished cars
                Debug.Log(cars[i] + " has finished!");
                // continue;
            }

            // calculate the distance traveled by the car
            float totalDistance = (cars[i].currentLap * lapDistance) + distanceToNextCheckpoint + ((checkpoints.Length - cars[i].chkpntCurr - 1) * lapDistance);

            // check if the car has gone too far and reset its lap count
            if (totalDistance > maxDistance)
            {
                cars[i].currentLap = 0;
                cars[i].chkpntCurr = 0;
            }

            // update the car's distance traveled
            cars[i].distanceTraveled = totalDistance;

            // update the car's race position
            cars[i].racePosition = 1;
            for (int j = 0; j < cars.Count; j++)
            {
                if (i != j && cars[i].currentLap == cars[j].currentLap && cars[i].chkpntCurr == cars[j].chkpntCurr)
                {
                    float distanceToNextCheckpointJ = Vector3.Distance(cars[j].transform.position, checkpoints[cars[j].chkpntCurr].transform.position);
                    if (distanceToNextCheckpoint > distanceToNextCheckpointJ)
                    {
                        cars[i].racePosition++;
                    }
                    else if (distanceToNextCheckpoint < distanceToNextCheckpointJ)
                    {
                        cars[j].racePosition++;
                    }
                }
                else if (i != j && cars[i].currentLap > cars[j].currentLap)
                {
                    cars[i].racePosition++;
                }
                else if (i != j && cars[i].currentLap == cars[j].currentLap && cars[i].chkpntCurr > cars[j].chkpntCurr)
                {
                    cars[i].racePosition++;
                }
            }
        }
    }
}