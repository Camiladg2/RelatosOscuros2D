using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class IndicadorInteraccion : MonoBehaviour
{
    public GameObject indicador;

    [Header("Movimiento suave (opcional)")]
    public float subeBaja = 0.08f;
    public float velocidad = 2.5f;

    private Vector3 posBase;
    private int dentro;

    private void Reset()
    {
        Collider2D c = GetComponent<Collider2D>();
        if (c != null) c.isTrigger = true;
    }

    private void Start()
    {
        if (indicador != null)
        {
            posBase = indicador.transform.localPosition;
            indicador.SetActive(false);
        }
    }

    private void Update()
    {
        if (indicador == null || !indicador.activeSelf || subeBaja <= 0f) return;
        indicador.transform.localPosition = posBase + new Vector3(0f, Mathf.Sin(Time.time * velocidad) * subeBaja, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController2D>() == null) return;
        dentro++;
        if (indicador != null) indicador.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerController2D>() == null) return;
        dentro = Mathf.Max(0, dentro - 1);
        if (dentro == 0 && indicador != null) indicador.SetActive(false);
    }

    private void OnDisable()
    {
        dentro = 0;
        if (indicador != null) indicador.SetActive(false);
    }
}
