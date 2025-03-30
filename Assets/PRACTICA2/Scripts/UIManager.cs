using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Necesario para TextMeshPro
using UnityEngine.UI; // Necesario para Button

public class UIManager : MonoBehaviour
{
    public GameObject infoPanel; // Arrastra aquí el Panel de la UI desde la jerarquía
    public TextMeshProUGUI fishNameText; // Arrastra aquí el TextMeshPro para el nombre
    public TextMeshProUGUI fishDescriptionText; // Arrastra aquí el TextMeshPro para la descripción
    public Button closeButton; // Arrastra aquí el botón de cerrar

    void Start()
    {
        // Asegúrate de que el panel esté oculto al inicio
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }

        // Añade la funcionalidad al botón de cerrar
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideFishInfo);
        }
        else
        {
            Debug.LogWarning("Botón de cerrar no asignado en UIManager.");
        }
    }

    // Función para mostrar la información
    public void ShowFishInfo(string name, string description)
    {
        if (infoPanel != null && fishNameText != null && fishDescriptionText != null)
        {
            fishNameText.text = name;
            fishDescriptionText.text = description;
            infoPanel.SetActive(true); // Muestra el panel
        }
        else
        {
            Debug.LogError("Algunos elementos de UI no están asignados en el UIManager.");
        }
    }

    // Función para ocultar la información (llamada por el botón)
    public void HideFishInfo()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false); // Oculta el panel
        }
    }
}