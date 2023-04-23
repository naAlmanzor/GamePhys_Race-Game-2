using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    public CarController playerCar;
    public List<CarController> cars;
    public Checkpoint[] checkpoints;
    public int totalLaps = 3;
    public TextMeshProUGUI lapCountText;
    public TextMeshProUGUI positionText;

    // public Cinemachine camera;
    public Cinemachine.CinemachineVirtualCamera cam;


    public static GameManager instance;

    public CarData carData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // cars = RaceManager.instance.cars;
        //set playercar bool based on index
        for(int i = 0; i < cars.Count; i++) {
            if(i == carData.carMeshIndex) {
                cars[i].isPlayer = true;
                playerCar = cars[i];
            }
        }

        if(playerCar != null) {
            cam.LookAt = playerCar.transform;
            cam.Follow = playerCar.transform;
        }
        

        // Set lap count UI text
        lapCountText.SetText((playerCar.currentLap + 1).ToString());
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Debug.Log(playerCar.currentLap + 1);
        if (playerCar.currentLap >= totalLaps)
        {
            SceneManager.LoadScene("WinScene");
            Debug.Log("Player Wins");
        }
        lapCountText.SetText((playerCar.currentLap + 1).ToString());
        positionText.SetText(playerCar.racePosition.ToString());

        if(Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("MainMenuScene");
        }
    }
}
