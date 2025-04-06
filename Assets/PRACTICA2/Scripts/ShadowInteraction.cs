using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Necesario si usas TextMeshPro para el texto

public class ShadowInteraction : MonoBehaviour
{
    public GameObject[] fishPrefabs; // Array de prefabs de peces
    public string[] fishNames; // Array de nombres de peces
    [TextArea(3, 10)]
    public string[] fishDescriptions; // Array de descripciones de peces

    private UIManager uiManager; // Referencia al gestor de UI

    void Start()
    {
        // Busca el UIManager en la escena. Asegúrate de que solo haya uno.
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("No se encontró un UIManager en la escena.");
        }
    }

    // Esta función será llamada desde otro script cuando se toque esta sombra
    public void RevealFish()
    {
        if (fishPrefabs != null && fishPrefabs.Length > 0)
        {
            // Seleccionar un pez al azar
            int randomIndex = Random.Range(0, fishPrefabs.Length);
            GameObject selectedFishPrefab = fishPrefabs[randomIndex];
            string selectedFishName = fishNames[randomIndex];
            string selectedFishDescription = fishDescriptions[randomIndex];

            // Instanciar el pez en la posición de la sombra
            GameObject fish = Instantiate(selectedFishPrefab, transform.position, transform.rotation);

            // Fijarlo a la cámara para que se mueva con ella
            if (Camera.main != null)
            {
                fish.transform.SetParent(Camera.main.transform, false);
                fish.transform.localPosition = new Vector3(0, 0.2f, 1.0f);
                fish.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);



            }
            else
            {
                Debug.LogWarning("No se encontró la cámara principal.");
            }

            // Enviar la referencia del pez al UIManager
            if (uiManager != null)
            {
                uiManager.SetCurrentFish(fish);
                uiManager.ShowFishInfo(selectedFishName, selectedFishDescription);
            }
            else
            {
                Debug.LogWarning("UIManager no encontrado, no se puede mostrar la info.");
            }

            // Destruir el objeto sombra
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("¡No hay prefabs de peces asignados en el Inspector!");
        }
    }
}