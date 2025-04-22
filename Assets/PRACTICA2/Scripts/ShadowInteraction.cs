using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Necesari pel TextMeshPro

public class ShadowInteraction : MonoBehaviour
{
    public GameObject[] fishPrefabs; // Array dels prefabs dels peixos
    public string[] fishNames; // Array dels noms dels peixos
    [TextArea(3, 10)]
    public string[] fishDescriptions; // Array de les descripcions dels peixos

    private UIManager uiManager; // Referencia al gestor de UI

    void Start()
    {
        //busca la UIManager en la escena.
        uiManager = FindObjectOfType<UIManager>();

    }

    //función es truca des de altre script quan toqui l'ombra
    public void RevealFish()
    {
        if (fishPrefabs != null && fishPrefabs.Length > 0)
        {
            // selecciona un peix random
            int randomIndex = Random.Range(0, fishPrefabs.Length);
            GameObject selectedFishPrefab = fishPrefabs[randomIndex];
            string selectedFishName = fishNames[randomIndex];
            string selectedFishDescription = fishDescriptions[randomIndex];

            // Instanciar el pez en la posición de la sombra
            GameObject fish = Instantiate(selectedFishPrefab, transform.position, transform.rotation);

            //fixar la cam per que es mogui amb ella
            if (Camera.main != null)
            {
                fish.transform.SetParent(Camera.main.transform, false);
                fish.transform.localPosition = new Vector3(0, 0.2f, 1.0f);
                fish.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            }
            else
            {
                Debug.LogWarning("no s'ha detectat una cam principal");
            }

            //envia la ref del peix a la UIManager
            uiManager.SetCurrentFish(fish);
            uiManager.ShowFishInfo(selectedFishName, selectedFishDescription);

            Destroy(gameObject);
        }

    }
}