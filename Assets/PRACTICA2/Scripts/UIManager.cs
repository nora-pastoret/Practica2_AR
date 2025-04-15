using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation; // Necesario para ARSession
using UnityEngine.XR.ARSubsystems; // Necesario para ARSessionState

public class UIManager : MonoBehaviour
{
    public GameObject infoPanel;
    public TextMeshProUGUI fishNameText;
    public TextMeshProUGUI fishDescriptionText;
    public Button closeButton;
    public Button restartButton; // << NUEVO: Referencia al botón de reiniciar

    // Referencias a componentes AR necesarios para reiniciar
    public ARSession arSession;         // << NUEVO: Asignar el objeto AR Session desde el Inspector
    public ARPlaneManager planeManager; // << NUEVO: Asignar el AR Plane Manager desde el Inspector
    public ShadowSpawner shadowSpawner; // << NUEVO: Asignar el objeto que tiene ShadowSpawner

    private GameObject currentFish;

    void Start()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideFishInfo);
        }
        else
        {
            Debug.LogWarning("Botón de cerrar no asignado en UIManager.");
        }

        // << NUEVO: Añadir listener para el botón de reiniciar
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartARSession);
        }
        else
        {
            Debug.LogWarning("Botón de reiniciar no asignado en UIManager.");
        }

        // << NUEVO: Buscar referencias si no están asignadas (opcional, mejor asignar en Inspector)
        if (arSession == null)
        {
            arSession = FindObjectOfType<ARSession>();
            if (arSession == null) Debug.LogError("ARSession no encontrado en la escena.");
        }
        if (planeManager == null)
        {
            planeManager = FindObjectOfType<ARPlaneManager>();
            if (planeManager == null) Debug.LogError("ARPlaneManager no encontrado en la escena.");
        }
        if (shadowSpawner == null)
        {
            shadowSpawner = FindObjectOfType<ShadowSpawner>();
            if (shadowSpawner == null) Debug.LogError("ShadowSpawner no encontrado en la escena.");
        }
    }

    public void SetCurrentFish(GameObject fish)
    {
        currentFish = fish;
    }

    public void ShowFishInfo(string name, string description)
    {
        // ... (código existente sin cambios)
        if (infoPanel != null && fishNameText != null && fishDescriptionText != null)
        {
            fishNameText.text = name;
            fishDescriptionText.text = description;
            infoPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Algunos elementos de UI no están asignados en el UIManager.");
        }
    }

    public void HideFishInfo()
    {
        // ... (código existente sin cambios)
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }

        if (currentFish != null)
        {
            Destroy(currentFish);
            currentFish = null;
        }
    }

    // << NUEVO: Método que se llamará al pulsar el botón Reiniciar
    public void RestartARSession()
    {
        Debug.Log("Reiniciando sesión AR...");

        // 1. Ocultar panel de info y destruir pez actual si existe
        HideFishInfo();

        // 2. Destruir todas las sombras de peces existentes
        GameObject[] existingShadows = GameObject.FindGameObjectsWithTag("FishShadow");
        Debug.Log($"Encontradas {existingShadows.Length} sombras para destruir.");
        foreach (GameObject shadow in existingShadows)
        {
            Destroy(shadow);
        }

        // 3. Resetear el estado interno del ShadowSpawner (opcional pero recomendado)
        if (shadowSpawner != null)
        {
            shadowSpawner.ResetSpawner(); // Necesitaremos añadir este método a ShadowSpawner
        }

        // 4. Resetear la sesión AR y la detección de planos
        if (arSession != null)
        {
            arSession.Reset(); // Esto reinicia el tracking y elimina los trackables existentes (planos)
        }
        else
        {
            Debug.LogError("ARSession no asignada, no se puede reiniciar.");
        }

        // Opcional: Deshabilitar y rehabilitar el Plane Manager puede ser una alternativa si Reset() no funciona como esperado
        // if (planeManager != null)
        // {
        //    planeManager.enabled = false;
        //    planeManager.enabled = true;
        // }

        Debug.Log("Sesión AR reiniciada. Buscando nuevos planos...");
    }
}