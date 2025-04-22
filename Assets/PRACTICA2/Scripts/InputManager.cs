using UnityEngine;
using UnityEngine.InputSystem; //nou input system

public class InputManager : MonoBehaviour
{
    public Camera arCamera;
    private UIManager uiManager;//per indicar que no es pugui interactuar amb shadows si està obert el panell d'info


    void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    void Update()
    {
        // comprova si s'está tocant la pantalla. Punter activat
        if (Pointer.current == null || uiManager == null || uiManager.IsInfoPanelActive())
            return; // Sortir si no hi ha Punter o si el panell de info està actiu

        // comprova que el botó està presionat en aquell frame
        if (Pointer.current.press.wasPressedThisFrame)
        {
            //obté la posició del punter en coordenades de pantalla
            Vector2 screenPosition = Pointer.current.position.ReadValue();

            //truca a la funció HandleTap amb la posició obtenida
            HandleTap(screenPosition);
        }

    }

    //funció HandleTap que realitza el Raycast
    void HandleTap(Vector2 screenPosition)
    {
        if (arCamera == null)
        {
            Debug.LogError("AR Camera no asignada en InputManager!");
            return;
        }

        Ray ray = arCamera.ScreenPointToRay(screenPosition);
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit))
        {

            ShadowInteraction shadow = hit.collider.GetComponent<ShadowInteraction>();

            if (shadow != null)
            {
                shadow.RevealFish();
            }

        }

    }
}