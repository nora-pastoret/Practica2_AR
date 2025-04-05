using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ShadowSpawner : MonoBehaviour
{
    
    public GameObject FishShadow;
    
    public float fishDensity = 1f; // numero de peixos per metre quadrat - per exemple, 1 peix per m²

    // Guarda el nombre de peixos ja instanciats per cada ARPlane
    private Dictionary<ARPlane, int> fishCountByPlane = new Dictionary<ARPlane, int>();

    void OnEnable()
    {
        // Ens subscrivim als canvis de plans del ARPlaneManager
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

    // Gestió d'events quan hi ha nous plans, actualitzacions o eliminacions
    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        // Per als nous plans:
        foreach (ARPlane plane in args.added)
        {
            fishCountByPlane[plane] = 0;
            plane.boundaryChanged += OnPlaneBoundaryChanged;
            UpdateFishOnPlane(plane);
        }

        // Per als plans actualitzats:
        foreach (ARPlane plane in args.updated)
        {
            UpdateFishOnPlane(plane);
        }

        // Neteja per als plans eliminats:
        foreach (ARPlane plane in args.removed)
        {
            if (fishCountByPlane.ContainsKey(plane))
                fishCountByPlane.Remove(plane);
        }
    }

    // Quan la frontera del pla canvia (és a dir, pot haver creixement)
    void OnPlaneBoundaryChanged(ARPlaneBoundaryChangedEventArgs eventArgs)
    {
        UpdateFishOnPlane(eventArgs.plane);
    }

    // Actualitza el nombre de peixos d'un pla segons la seva mida
    void UpdateFishOnPlane(ARPlane plane)
    {
        // Calcular àrea del pla (aproximació amb extents, que són la meitat de la mida)
        float area = (plane.extents.x * 2) * (plane.extents.y * 2);
        int desiredFishCount = Mathf.FloorToInt(area * fishDensity);

        // Obtenir el nombre actual de peixos instanciats per aquest pla
        int currentFishCount = fishCountByPlane.ContainsKey(plane) ? fishCountByPlane[plane] : 0;
        int fishToSpawn = desiredFishCount - currentFishCount;

        if (fishToSpawn > 0)
        {
            // Instanciem els nous peixos necessaris
            for (int i = 0; i < fishToSpawn; i++)
            {
                Vector3 spawnPos = GetRandomPointInPlane(plane);
                // Es pot fer fill del pla per mantenir la posició relativa en cas de moviment o actualització
                Instantiate(FishShadow, spawnPos, Quaternion.identity, plane.transform);
            }
            fishCountByPlane[plane] = desiredFishCount;
        }
    }

    // Retorna un punt aleatori dins del pla utilitzant els extents
    Vector3 GetRandomPointInPlane(ARPlane plane)
    {
        float randomX = Random.Range(-plane.extents.x, plane.extents.x);
        float randomZ = Random.Range(-plane.extents.y, plane.extents.y);
        Vector3 localPos = new Vector3(randomX, 0, randomZ);
        return plane.transform.TransformPoint(localPos);
    }
}
