using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Necesario si usas TextMeshPro para el texto

public class ShadowInteraction : MonoBehaviour
{
    public GameObject fishPrefab; // Arrastra aquí el prefab del PEZ correspondiente a esta sombra
    public string fishName = "Nombre del Pez"; // Nombre para mostrar
    [TextArea(3, 10)] // Hace el campo de texto más grande en el Inspector
    public string fishDescription = "Descripción detallada del pez...";

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
        if (fishPrefab != null)
        {
            // Instanciar el pez en la posición de la sombra
            GameObject fish = Instantiate(fishPrefab, transform.position, transform.rotation);

            // Fijarlo a la cámara para que se mueva con ella
            if (Camera.main != null)
            {
                fish.transform.SetParent(Camera.main.transform, false);
                fish.transform.localPosition = new Vector3(0, 0, 1.0f);
            }
            else
            {
                Debug.LogWarning("No se encontró la cámara principal.");
            }

            // Enviar la referencia del pez al UIManager
            if (uiManager != null)
            {
                uiManager.SetCurrentFish(fish);
                uiManager.ShowFishInfo(fishName, fishDescription);
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
            Debug.LogError("¡FishPrefab no asignado en el Inspector para esta sombra!");
        }
    }

}