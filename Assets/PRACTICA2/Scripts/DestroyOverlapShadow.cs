using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class DestroyOverlapShadow : MonoBehaviour
{
    public float checkRadius = 0.5f;

    void Start()
    {
        Collider[] overlapping = Physics.OverlapSphere(transform.position, checkRadius);
        foreach (var col in overlapping)
        {
            if (col.gameObject != this.gameObject && col.CompareTag("FishShadow"))
            {
                Debug.Log("Solapament detectat, esborrem.");
                Destroy(gameObject);
                break;
            }
        }
    }
}
