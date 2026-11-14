using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform objetivo;
    public float velocidadCamara = 0.1f;
    public Vector3 desplazamiento = new Vector3(0f, 0.3f, -10f);

    [Header("Limites (para no salir del escenario)")]
    public bool usarLimites = true;
    public float limiteIzquierdo = -10f;
    public float limiteDerecho = 40f;
    public float limiteAbajo = -5f;
    public float limiteArriba = 10f;

    private void LateUpdate()
    {
        if (objetivo == null) return;

        Vector3 posicionDeseada = objetivo.position + desplazamiento;
        Vector3 posicionSuavizada = Vector3.Lerp(transform.position, posicionDeseada, velocidadCamara);

        if (usarLimites)
        {
            posicionSuavizada.x = Mathf.Clamp(posicionSuavizada.x, limiteIzquierdo, limiteDerecho);
            posicionSuavizada.y = Mathf.Clamp(posicionSuavizada.y, limiteAbajo, limiteArriba);
        }

        transform.position = posicionSuavizada;
    }
}