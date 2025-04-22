using UnityEngine;
using UnityEngine.InputSystem; 

public class RotateFish : MonoBehaviour
{
    public float velocidadDeRotacion = 0.02f;
    private Vector2? posicioAnterior = null;

    void Update()
    {
        if (Pointer.current == null || Pointer.current.press == null || !Pointer.current.press.isPressed)
        {
            posicioAnterior = null;
            return;
        }

        Vector2 posicioActual = Pointer.current.position.ReadValue();

        if (posicioAnterior != null)
        {
            Vector2 delta = posicioActual - posicioAnterior.Value;

            // Rotació horitzontal (en l’eix Y)
            float angleY = -delta.x * velocidadDeRotacion;
            transform.Rotate(0f, angleY, 0f, Space.Self);
        }

        posicioAnterior = posicioActual;
    }
}

