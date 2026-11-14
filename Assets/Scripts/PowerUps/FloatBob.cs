using UnityEngine;

/// Hace que el objeto suba y baje suavemente, como si levitara.
public class FloatBob : MonoBehaviour
{
    public float amplitude = 0.15f; // cuanto sube y baja
    public float speed = 2f;        // velocidad

    Vector3 startPos;
    float offset;

    void Start()
    {
        startPos = transform.localPosition;
        offset = Random.value * 6.28f; // para que no suban todas a la vez
    }

    void Update()
    {
        Vector3 p = startPos;
        p.y += Mathf.Sin(Time.time * speed + offset) * amplitude;
        transform.localPosition = p;
    }
}