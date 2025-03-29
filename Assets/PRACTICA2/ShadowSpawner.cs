using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ShadowSpawner : MonoBehaviour
{
    public GameObject FishShadow;
    private ARPlane[] allPlanes;
    private HashSet<ARPlane> spawnedPlanes = new HashSet<ARPlane>(); // Guarda els plans on ja hem generat peixos

    void Start()
    {
        StartCoroutine(WaitForPlanesAndSpawn());
    }

    IEnumerator WaitForPlanesAndSpawn()
    {
        while (true)
        {
            allPlanes = GameObject.FindObjectsByType<ARPlane>(FindObjectsSortMode.InstanceID);

            foreach (ARPlane plane in allPlanes)
            {
                if (!spawnedPlanes.Contains(plane)) // Només afegir peixos si encara no hi ha en aquest pla
                {
                    Debug.Log("Generant peixos per a un nou ARPlane.");
                    SpawnFishOnPlane(plane);
                    spawnedPlanes.Add(plane); // Marquem aquest ARPlane com a ja fet
                }
            }

            yield return new WaitForSeconds(1f); // Espera per evitar sobrecàrrega
        }
    }

    void SpawnFishOnPlane(ARPlane plane)
    {
        Debug.Log("Spawnejant 5 peixos sobre un ARPlane.");
        for (int i = 0; i < 5; i++)
        {
            Vector3 position = plane.transform.position + new Vector3(Random.Range(-3.0f, 3.0f), 0, Random.Range(-3.0f, 3.0f));
            Instantiate(FishShadow, position, Quaternion.identity);
        }
    }
}
