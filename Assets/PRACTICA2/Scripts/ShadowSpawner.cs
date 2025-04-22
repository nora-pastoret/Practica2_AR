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
    private ARPlaneManager planeManager; // Guardar referencia para OnDisable

    void Awake() //Awake per trobar references abans de OnEnable
    {
        planeManager = FindObjectOfType<ARPlaneManager>();
    }

    void OnEnable()
    {
        if (planeManager != null)
        {
            planeManager.planesChanged += OnPlanesChanged;
        }
    }

    void OnDisable()
    {
        if (planeManager != null)
        {
            planeManager.planesChanged -= OnPlanesChanged;
            foreach (var plane in fishCountByPlane.Keys)
            {
                if (plane != null) //comprovar si el pla encara existeix
                {
                    plane.boundaryChanged -= OnPlaneBoundaryChanged;
                }
            }
        }
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        foreach (ARPlane plane in args.added)
        {
            if (!fishCountByPlane.ContainsKey(plane)) 
            {
                fishCountByPlane[plane] = 0;
                plane.boundaryChanged += OnPlaneBoundaryChanged;
                UpdateFishOnPlane(plane);
            }
        }

        foreach (ARPlane plane in args.updated)
        {
            if (fishCountByPlane.ContainsKey(plane) && plane.subsumedBy == null) //asegurar si el pla encara es valid
            {
                UpdateFishOnPlane(plane);
            }
        }

        foreach (ARPlane plane in args.removed)
        {
            //destruir ombres del pla
            foreach (Transform child in plane.transform)
            {
                if (child.CompareTag("FishShadow"))
                {
                    Destroy(child.gameObject);
                }
            }
            if (plane != null) 
            {
                plane.boundaryChanged -= OnPlaneBoundaryChanged;//limits del pla
            }
            if (fishCountByPlane.ContainsKey(plane))
            {
                fishCountByPlane.Remove(plane);
            }
        }
    }

    void OnPlaneBoundaryChanged(ARPlaneBoundaryChangedEventArgs eventArgs)
    {
        UpdateFishOnPlane(eventArgs.plane);
    }

    void UpdateFishOnPlane(ARPlane plane)
    {
        if (plane == null || plane.subsumedBy != null || plane.trackingState != UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
        {
            return;
        }

        //calcula l'area del pla i quan pexios tocarien
        float area = plane.size.x * plane.size.y; 
        int desiredFishCount = Mathf.FloorToInt(area * fishDensity);

        int currentFishCount = fishCountByPlane.ContainsKey(plane) ? fishCountByPlane[plane] : 0;
        int fishToSpawn = desiredFishCount - currentFishCount;

        if (fishToSpawn <= 0) return; 



        //pels solapaments
        int tries = 0;
        int spawned = 0;
        int maxTries = fishToSpawn * 10; // intents

        while (spawned < fishToSpawn && tries < maxTries)
        {
            Vector3 spawnPos = GetRandomPointInPlaneBounds(plane); // basant en els limits

            if (IsPositionFree(spawnPos, fishRadius))
            {
                GameObject fish = Instantiate(FishShadow, spawnPos, Quaternion.identity, plane.transform);
                fish.tag = "FishShadow"; // per detectar-ho més endavant si es necessita
                spawned++;
            }
            tries++;
        }

        if (fishCountByPlane.ContainsKey(plane))
            fishCountByPlane[plane] += spawned;
        else
            fishCountByPlane[plane] = spawned;
    }

    // Retorna un punto aleatorio dentro de los l?mites del plano  retorna un punt aleatori dins dels lim dels plans
    Vector3 GetRandomPointInPlaneBounds(ARPlane plane)
    {
        // genera un punt aleatori dins del rectangle
        float randomX = Random.Range(-plane.size.x / 2f, plane.size.x / 2f);
        float randomZ = Random.Range(-plane.size.y / 2f, plane.size.y / 2f);
        Vector3 localPos = new Vector3(randomX, 0, randomZ);

        return plane.transform.TransformPoint(localPos);
    }

    bool IsPositionFree(Vector3 position, float radius)
    {
        Collider[] overlaps = Physics.OverlapSphere(position, radius);
        foreach (Collider col in overlaps)
        {
            if (col.CompareTag("FishShadow") && col.transform.position != position) 
                return false;
        }
        return true;
    }

    public void ResetSpawner()
    {
        fishCountByPlane.Clear();

    }
}