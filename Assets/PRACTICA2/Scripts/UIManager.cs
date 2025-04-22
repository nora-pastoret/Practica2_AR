using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation; 
using UnityEngine.XR.ARSubsystems;
using System; 

public class UIManager : MonoBehaviour
{
    public GameObject infoPanel;
    public TextMeshProUGUI fishNameText;
    public TextMeshProUGUI fishDescriptionText;
    public Button closeButton;
    public Button restartButton; 

    public ARSession arSession;        
    public ARPlaneManager planeManager; 
    public ShadowSpawner shadowSpawner; 

    private GameObject currentFish;

    void Start()
    {
        infoPanel.SetActive(false);
        closeButton.onClick.AddListener(HideFishInfo);
        restartButton.onClick.AddListener(RestartARSession);
        arSession = FindObjectOfType<ARSession>();
        planeManager = FindObjectOfType<ARPlaneManager>();
        shadowSpawner = FindObjectOfType<ShadowSpawner>();

    }

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

    //metode que es trucara al clicar el boto de reiniciar
    public void RestartARSession()
    {

        HideFishInfo(); //oculta el panel de info i destrueix el peix

        //destrueix les ombres dels peixos
        GameObject[] existingShadows = GameObject.FindGameObjectsWithTag("FishShadow");
        foreach (GameObject shadow in existingShadows)
        {
            Destroy(shadow);
        }

        //resetear el joc
        if (shadowSpawner != null)
        {
            shadowSpawner.ResetSpawner(); 
        }
        if (arSession != null)
        {
            arSession.Reset(); 
        }

    }

    public bool IsInfoPanelActive()//per enviar la si està activat al InputManager
    {
        return infoPanel != null && infoPanel.activeSelf;
    }
}