/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateFish : MonoBehaviour
{
    public float velocidadDeRotacion = 20f;

    void Update()
    {

       if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Vector2 mov = Input.GetTouch(0).deltaPosition;
            float x = mov.x * Mathf.Deg2Rad * velocidadDeRotacion;
            float y = mov.y * Mathf.Deg2Rad * velocidadDeRotacion;
            transform.RotateAround(Vector3.zero, Vector3.up, -x);
            transform.RotateAround(Vector3.zero, Vector3.up, y);

        }
    }
}*/

using UnityEngine;
using UnityEngine.InputSystem; // Nou sistema d'input

public class RotateFish : MonoBehaviour
{
    public float velocidadDeRotacion = 0.2f;
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

