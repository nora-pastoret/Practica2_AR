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
            // 1. Instanciar el pez en la misma posición y rotación que la sombra
            Instantiate(fishPrefab, transform.position, transform.rotation);

            // 2. Mostrar la información en la UI
            if (uiManager != null)
            {
                uiManager.ShowFishInfo(fishName, fishDescription);
            }
            else
            {
                Debug.LogWarning("UIManager no encontrado, no se puede mostrar la info.");
            }

            // 3. Destruir el objeto de la sombra
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("¡FishPrefab no asignado en el Inspector para esta sombra!");
        }
    }
}