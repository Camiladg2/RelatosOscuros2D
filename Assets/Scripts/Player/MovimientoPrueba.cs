using UnityEngine;

public class MovimientoPrueba : MonoBehaviour
{
    public float velocidad = 5f;

    void Update()
    {
        // Detecta las flechas del teclado o WASD
        float movimientoHorizontal = Input.GetAxis("Horizontal");
        float movimientoVertical = Input.GetAxis("Vertical");

        // Crea el vector de movimiento (X, Y, Z)
        // Nota: Si tu juego es 2D, cambia esto por: new Vector3(movimientoHorizontal, movimientoVertical, 0);
        Vector3 direccion = new Vector3(movimientoHorizontal, 0, movimientoVertical);

        // Mueve la esfera de forma fluida
        transform.Translate(direccion * velocidad * Time.deltaTime);
    }
}
