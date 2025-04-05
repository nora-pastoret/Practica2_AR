using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ShadowSpawner : MonoBehaviour
{
    public GameObject FishShadow;
    public float fishDensity = 1f; // número de peixos per m²
    public float fishRadius = 0.4f; // Radi utilitzat per comprovar solapaments

    private Dictionary<ARPlane, int> fishCountByPlane = new Dictionary<ARPlane, int>();

    void OnEnable()
    {
        ARPlaneManager planeManager = FindObjectOfType<ARPlaneManager>();
        if (planeManager != null)
        {
            planeManager.planesChanged += OnPlanesChanged;
        }
    }

    void OnDisable()
    {
        ARPlaneManager planeManager = FindObjectOfType<ARPlaneManager>();
        if (planeManager != null)
        {
            planeManager.planesChanged -= OnPlanesChanged;
        }
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        foreach (ARPlane plane in args.added)
        {
            fishCountByPlane[plane] = 0;
            plane.boundaryChanged += OnPlaneBoundaryChanged;
            UpdateFishOnPlane(plane);
        }

        foreach (ARPlane plane in args.updated)
        {
            UpdateFishOnPlane(plane);
        }

        foreach (ARPlane plane in args.removed)
        {
            if (fishCountByPlane.ContainsKey(plane))
                fishCountByPlane.Remove(plane);
        }
    }

    void OnPlaneBoundaryChanged(ARPlaneBoundaryChangedEventArgs eventArgs)
    {
        UpdateFishOnPlane(eventArgs.plane);
    }

    void UpdateFishOnPlane(ARPlane plane)
    {
        //calcula l'area del pla i quan pexios tocarien
        float area = (plane.extents.x * 2) * (plane.extents.y * 2);
        int desiredFishCount = Mathf.FloorToInt(area * fishDensity);

        int currentFishCount = fishCountByPlane.ContainsKey(plane) ? fishCountByPlane[plane] : 0;
        int fishToSpawn = desiredFishCount - currentFishCount;

        //pels solapaments
        int tries = 0;
        int spawned = 0;
        int maxTries = fishToSpawn * 5;

        while (spawned < fishToSpawn && tries < maxTries)
        {
            Vector3 spawnPos = GetRandomPointInPlane(plane);

            if (IsPositionFree(spawnPos, fishRadius))
            {
                GameObject fish = Instantiate(FishShadow, spawnPos, Quaternion.identity, plane.transform);
                fish.tag = "FishShadow"; // per detectar-ho més endavant si vols
                spawned++;
            }

            tries++;
        }

        if (fishCountByPlane.ContainsKey(plane))
            fishCountByPlane[plane] += spawned;
        else
            fishCountByPlane[plane] = spawned;
    }

    // Retorna un punt aleatori dins del pla utilitzant els extents
    Vector3 GetRandomPointInPlane(ARPlane plane)
    {
        float randomX = Random.Range(-plane.extents.x, plane.extents.x);
        float randomZ = Random.Range(-plane.extents.y, plane.extents.y);
        Vector3 localPos = new Vector3(randomX, 0, randomZ);
        return plane.transform.TransformPoint(localPos);
    }

    // Comprova si la posició està lliure de objecte shadow
    bool IsPositionFree(Vector3 position, float radius)
    {
        Collider[] overlaps = Physics.OverlapSphere(position, radius);
        foreach (Collider col in overlaps)
        {
            if (col.CompareTag("FishShadow"))
                return false;
        }
        return true;
    }
}
