using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Muestra el aviso "Te faltan X pociones" unos segundos y luego lo oculta.
/// Conectalo al evento On Not Ready del caldero con la funcion Mostrar().
/// </summary>
public class MensajeFaltan : MonoBehaviour
{
    [Tooltip("El panel completo (imagen + texto). Debe empezar DESACTIVADO.")]
    public GameObject panel;
    [Tooltip("El texto (UI > Legacy > Text) donde se escribe el mensaje.")]
    public Text texto;
    public float segundos = 3f;

    Coroutine actual;

    public void Mostrar()
    {
        if (panel == null) return;

        int faltan = GameProgress.TotalRequired - GameProgress.Total;
        if (texto != null)
        {
            if (faltan <= 1) texto.text = "¡Te falta 1 poción!";
            else texto.text = "Te faltan " + faltan + " pociones";
        }

        if (actual != null) StopCoroutine(actual);
        actual = StartCoroutine(Rutina());
    }

    IEnumerator Rutina()
    {
        panel.SetActive(true);
        yield return new WaitForSeconds(segundos);
        panel.SetActive(false);
        actual = null;
    }
}
