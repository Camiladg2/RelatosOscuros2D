using System.Collections;
using UnityEngine;

/// <summary>
/// Muestra un objeto (por ejemplo un texto de la interfaz) unos segundos y lo oculta.
/// Se conecta a un UnityEvent con el metodo Mostrar().
/// </summary>
public class MensajeTemporal : MonoBehaviour
{
    [Tooltip("El objeto que se muestra (un texto del Canvas). Debe empezar DESACTIVADO.")]
    public GameObject mensaje;
    public float segundos = 2.5f;

    Coroutine actual;

    public void Mostrar()
    {
        if (mensaje == null) return;
        if (actual != null) StopCoroutine(actual);
        actual = StartCoroutine(Rutina());
    }

    IEnumerator Rutina()
    {
        mensaje.SetActive(true);
        yield return new WaitForSeconds(segundos);
        mensaje.SetActive(false);
        actual = null;
    }
}
