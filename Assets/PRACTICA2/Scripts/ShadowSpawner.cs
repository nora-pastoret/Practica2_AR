using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ShadowSpawner : MonoBehaviour
{
    public GameObject FishShadow;
    public float fishDensity = 1f; // n?mero de peces por m?
    public float fishRadius = 0.4f; // Radio utilizado para comprobar solapamientos

    private Dictionary<ARPlane, int> fishCountByPlane = new Dictionary<ARPlane, int>();
    private ARPlaneManager planeManager; // Guardar referencia para OnDisable

    void Awake() // Usar Awake para encontrar referencias antes de OnEnable
    {
        planeManager = FindObjectOfType<ARPlaneManager>();
        if (planeManager == null)
        {
            Debug.LogError("ARPlaneManager no encontrado!");
        }
    }

    void OnEnable()
    {
        if (planeManager != null)
        {
            planeManager.planesChanged += OnPlanesChanged;
            Debug.Log("ShadowSpawner suscrito a planesChanged.");
        }
    }

    void OnDisable()
    {
        if (planeManager != null)
        {
            planeManager.planesChanged -= OnPlanesChanged;
            Debug.Log("ShadowSpawner desuscrito de planesChanged.");
            // Tambi?n desuscribirse de boundaryChanged si los planos a?n existen
            foreach (var plane in fishCountByPlane.Keys)
            {
                if (plane != null) // Comprobar si el plano a?n existe
                {
                    plane.boundaryChanged -= OnPlaneBoundaryChanged;
                }
            }
        }
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        Debug.Log($"Planes changed: Added={args.added.Count}, Updated={args.updated.Count}, Removed={args.removed.Count}");

        foreach (ARPlane plane in args.added)
        {
            if (!fishCountByPlane.ContainsKey(plane)) // Evitar a?adir si ya existe por alguna raz?n
            {
                Debug.Log($"Plano a?adido: {plane.trackableId}, suscribiendo a boundaryChanged.");
                fishCountByPlane[plane] = 0;
                plane.boundaryChanged += OnPlaneBoundaryChanged;
                UpdateFishOnPlane(plane);
            }
        }

        foreach (ARPlane plane in args.updated)
        {
            if (fishCountByPlane.ContainsKey(plane) && plane.subsumedBy == null) // Asegurarse que el plano a?n es v?lido
            {
                // Podr?as querer actualizar solo si el boundary o tama?o cambi? significativamente
                // Debug.Log($"Plano actualizado: {plane.trackableId}.");
                UpdateFishOnPlane(plane);
            }
        }

        foreach (ARPlane plane in args.removed)
        {
            Debug.Log($"Plano eliminado: {plane.trackableId}, desuscribiendo y eliminando del diccionario.");
            // Destruir sombras asociadas a este plano
            foreach (Transform child in plane.transform)
            {
                if (child.CompareTag("FishShadow"))
                {
                    Destroy(child.gameObject);
                }
            }
            // Desuscribirse del evento y eliminar del diccionario
            if (plane != null) // El objeto podr?a haber sido destruido ya
            {
                plane.boundaryChanged -= OnPlaneBoundaryChanged;
            }
            if (fishCountByPlane.ContainsKey(plane))
            {
                fishCountByPlane.Remove(plane);
            }
        }
    }

    void OnPlaneBoundaryChanged(ARPlaneBoundaryChangedEventArgs eventArgs)
    {
        Debug.Log($"Boundary cambiado para el plano: {eventArgs.plane.trackableId}");
        UpdateFishOnPlane(eventArgs.plane);
    }

    void UpdateFishOnPlane(ARPlane plane)
    {
        if (plane == null || plane.subsumedBy != null || plane.trackingState != UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
        {
            // No generar peces en planos no v?lidos, subsumidos o que no se est?n trackeando activamente
            return;
        }

        // calcula el ?rea del plano y cu?ntos peces tocar?an
        float area = plane.size.x * plane.size.y; // Usar plane.size es m?s directo que extents
        int desiredFishCount = Mathf.FloorToInt(area * fishDensity);

        int currentFishCount = fishCountByPlane.ContainsKey(plane) ? fishCountByPlane[plane] : 0;
        int fishToSpawn = desiredFishCount - currentFishCount;

        if (fishToSpawn <= 0) return; // No necesitamos generar m?s peces

        Debug.Log($"Actualizando plano {plane.trackableId}: Area={area:F2}, Desired={desiredFishCount}, Current={currentFishCount}, Spawning={fishToSpawn}");


        // Para los solapamientos
        int tries = 0;
        int spawned = 0;
        int maxTries = fishToSpawn * 10; // Aumentar intentos si el espacio es denso

        while (spawned < fishToSpawn && tries < maxTries)
        {
            Vector3 spawnPos = GetRandomPointInPlaneBounds(plane); // Usar un m?todo basado en l?mites

            if (IsPositionFree(spawnPos, fishRadius))
            {
                // Instanciar como hijo del plano para que se mueva con ?l
                GameObject fish = Instantiate(FishShadow, spawnPos, Quaternion.identity, plane.transform);
                fish.tag = "FishShadow"; // para detectarlo m?s adelante si vols
                spawned++;
            }
            tries++;
        }

        if (spawned > 0)
            Debug.Log($"Generados {spawned} peces en el plano {plane.trackableId}. Intentos: {tries}.");


        if (fishCountByPlane.ContainsKey(plane))
            fishCountByPlane[plane] += spawned;
        else
            fishCountByPlane[plane] = spawned; // Esto no deber?a pasar si se a?adi? correctamente en OnPlanesChanged
    }

    // Retorna un punto aleatorio dentro de los l?mites del plano
    Vector3 GetRandomPointInPlaneBounds(ARPlane plane)
    {
        // Genera un punto aleatorio dentro del rect?ngulo delimitador 2D del plano
        float randomX = Random.Range(-plane.size.x / 2f, plane.size.x / 2f);
        float randomZ = Random.Range(-plane.size.y / 2f, plane.size.y / 2f);
        Vector3 localPos = new Vector3(randomX, 0, randomZ);

        // Convierte la posici?n local (relativa al centro del plano) a posici?n mundial
        return plane.transform.TransformPoint(localPos);
    }


    // Comprueba si la posici?n est? libre de otros objetos shadow
    bool IsPositionFree(Vector3 position, float radius)
    {
        // Considera solo la capa donde est?n las sombras si las tienes en una capa espec?fica
        // int layerMask = 1 << LayerMask.NameToLayer("TuCapaDeSombras");
        // Collider[] overlaps = Physics.OverlapSphere(position, radius, layerMask);

        Collider[] overlaps = Physics.OverlapSphere(position, radius);
        foreach (Collider col in overlaps)
        {
            // Comprobar si el objeto encontrado es OTRA sombra (no uno mismo si el prefab tuviera este script)
            if (col.CompareTag("FishShadow") && col.transform.position != position) // Evita auto-colisi?n inicial
                return false;
        }
        return true;
    }

    // << NUEVO: M?todo p?blico para resetear el estado interno
    public void ResetSpawner()
    {
        Debug.Log("Reseteando ShadowSpawner...");
        // Limpiar el diccionario. Las sombras f?sicas se destruyen desde UIManager
        fishCountByPlane.Clear();

        // Desuscribirse de eventos de planos que puedan quedar referenciados (aunque Reset() deber?a eliminarlos)
        // El c?digo en OnDisable/OnPlanesChanged(removed) deber?a manejar esto si se llama correctamente.
        // Por seguridad, podr?amos iterar y desuscribir, pero puede ser complejo si los planos ya no existen.
        // Confiaremos en que ARSession.Reset() + OnPlanesChanged(removed) limpien correctamente.
    }
}