using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject infoPanel;
    public TextMeshProUGUI fishNameText;
    public TextMeshProUGUI fishDescriptionText;
    public Button closeButton;

    // Variable para almacenar el pez que se ha fijado a la cámara
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
    }

    // Método para asignar el pez actual
    public void SetCurrentFish(GameObject fish)
    {
        currentFish = fish;
    }

    public void ShowFishInfo(string name, string description)
    {
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

    // Al cerrar el panel, destruir el pez si existe
    public void HideFishInfo()
    {
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
}
