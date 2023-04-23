using System;
using System.Collections;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public static event Action LapEvent;

    public int index;
    private CarController player;

    private void Start() {
        player = GameManager.instance.playerCar;
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) {
            if(player.chkpntPrev == 3 && player.chkpntCurr == 4) {
            
                LapEvent.Invoke();
            }
        }
    }

}
