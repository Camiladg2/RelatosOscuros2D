using UnityEngine;

/// <summary>
/// Neblina que se desplaza despacio y cuya transparencia "respira". Ponlo en un objeto con Sprite Renderer
/// (con Draw Mode: Tiled) que sea hijo de la camara. Sirve para la neblina de fondo.
/// </summary>
public class FogDrift : MonoBehaviour
{
    [Header("Movimiento")]
    [Tooltip("Unidades por segundo. Negativo = hacia la izquierda.")]
    public float speed = 0.3f;
    [Tooltip("Ancho de UNA repeticion del dibujo en el mundo: ancho del PNG en pixeles / Pixels Per Unit.")]
    public float tileWidth = 10f;

    [Header("Transparencia que respira")]
    public bool pulse = true;
    [Range(0f, 1f)] public float alphaMin = 0.25f;
    [Range(0f, 1f)] public float alphaMax = 0.55f;
    public float pulseSpeed = 0.4f;

    private Vector3 startLocal;
    private float offset;
    private SpriteRenderer sr;

    private void Awake()
    {
        startLocal = transform.localPosition;
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        offset += speed * Time.deltaTime;
        // Al avanzar un ancho completo se vuelve al inicio: no se nota el salto porque el dibujo se repite.
        if (tileWidth > 0.01f && Mathf.Abs(offset) >= tileWidth) offset -= Mathf.Sign(offset) * tileWidth;
        transform.localPosition = new Vector3(startLocal.x + offset, startLocal.y, startLocal.z);

        if (pulse && sr != null)
        {
            float t = 0.5f + 0.5f * Mathf.Sin(Time.time * pulseSpeed * 6.2832f);
            Color c = sr.color;
            c.a = Mathf.Lerp(alphaMin, alphaMax, t);
            sr.color = c;
        }
    }
}
