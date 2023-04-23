using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NitroBox : MonoBehaviour
{
    // private void OnDisable() {
    //     Debug.Log("disabled");
    // }

    private void OnTriggerEnter(Collider other) {
        if(other.transform.CompareTag("Car")) {

        }
    }
}
