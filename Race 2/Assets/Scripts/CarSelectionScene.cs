using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CarSelectionScene : MonoBehaviour
{
    public MeshRenderer[] carMeshes;
    public int carMeshIndex = 0;
    public CarData carData;

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("MainMenuScene");
        }

        for(int i = 0; i < carMeshes.Length; i++) {
            if(i == carMeshIndex) {
                carMeshes[i].enabled = true;
            }
            else {
                carMeshes[i].enabled = false;
            }
        }
    }

    public void Prev() {
        if(carMeshIndex == 0) {
            carMeshIndex = 3;
        } else {carMeshIndex--;}
    }

    public void Next() {
        if(carMeshIndex == 3) {
            carMeshIndex = 0;
        } else {carMeshIndex++;}
    }
    public void Race() {
        carData.carMeshIndex = carMeshIndex;
        SceneManager.LoadScene("GameScene");
    }
}
