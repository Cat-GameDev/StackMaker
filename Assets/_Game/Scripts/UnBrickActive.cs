using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnBrickActive : MonoBehaviour
{
    [SerializeField] private GameObject Yellow;
    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Player")
        {
            Yellow.gameObject.SetActive(true);
        }
    }
}
