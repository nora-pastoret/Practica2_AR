using UnityEngine;
using UnityEngine.InputSystem; // ¡Necesario para el Nuevo Sistema de Input!

public class InputManager : MonoBehaviour
{
    public Camera arCamera;

    // Guarda una referencia a la acción de "presionar" si usas un Input Actions Asset (opcional pero recomendado)
    // public InputAction primaryPressAction; // Ejemplo si usas Input Actions Asset
    // public InputAction primaryPositionAction; // Ejemplo si usas Input Actions Asset

    // void Awake() {
    //     // Si usas Input Actions Asset, búscalas aquí
    //     // primaryPressAction = inputActionsAsset.FindActionMap("UI").FindAction("Click");
    //     // primaryPositionAction = inputActionsAsset.FindActionMap("UI").FindAction("Point");
    // }

    // void OnEnable() {
    //     // primaryPressAction?.Enable();
    //     // primaryPositionAction?.Enable();
    // }

    // void OnDisable() {
    //     // primaryPressAction?.Disable();
    //     // primaryPositionAction?.Disable();
    // }

    void Update()
    {
        // Comprueba si hay un puntero activo (ratón, dedo en pantalla)
        if (Pointer.current == null)
            return; // Salir si no hay puntero

        // Comprueba si el botón/dedo principal fue presionado en ESTE frame
        if (Pointer.current.press.wasPressedThisFrame)
        {
            // Obtiene la posición del puntero en coordenadas de pantalla
            Vector2 screenPosition = Pointer.current.position.ReadValue();

            // Añade un log para confirmar que detecta el toque y la posición
            Debug.Log($"Tap detectado (New Input System) en: {screenPosition}");

            // Llama a la función HandleTap con la posición obtenida
            HandleTap(screenPosition);
        }

        // Si usaras Input Actions Asset, la lógica sería diferente, basada en eventos:
        // if (primaryPressAction != null && primaryPressAction.triggered) {
        //     Vector2 screenPosition = primaryPositionAction.ReadValue<Vector2>();
        //     HandleTap(screenPosition);
        // }
    }

    // La función HandleTap sigue siendo la misma, realiza el Raycast
    void HandleTap(Vector2 screenPosition)
    {
        if (arCamera == null)
        {
            Debug.LogError("AR Camera no asignada en InputManager!");
            return;
        }

        Ray ray = arCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;

        Debug.Log("Lanzando Raycast desde InputManager (New Input System)...");

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log($"Raycast golpeó: {hit.collider.gameObject.name} en la capa: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

            Debug.Log("Intentando obtener ShadowInteraction del objeto golpeado...");
            ShadowInteraction shadow = hit.collider.GetComponent<ShadowInteraction>();

            if (shadow != null)
            {
                Debug.Log("¡ShadowInteraction encontrado! Llamando a RevealFish...");
                shadow.RevealFish();
            }
            else
            {
                Debug.LogWarning($"El objeto golpeado ({hit.collider.gameObject.name}) NO tiene el script ShadowInteraction.");
            }
        }
        else
        {
            Debug.Log("Raycast no golpeó ningún objeto con Collider.");
        }
    }
}