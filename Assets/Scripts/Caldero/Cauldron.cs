using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// El caldero final. Se activa con la tecla E cuando la protagonista esta cerca.
/// Si ya tiene las 9 botellas empieza el ritual; si no, avisa que faltan.
/// </summary>
public class Cauldron : MonoBehaviour, IInteractable
{
    [Tooltip("Objeto que se enciende cuando ya tienes las 9 botellas (un brillo, particulas...).")]
    public GameObject readyGlow;
    [Tooltip("Se ejecuta si presionas E y todavia faltan botellas (por ejemplo, mostrar un mensaje).")]
    public UnityEvent onNotReady;
    [Tooltip("Se ejecuta si presionas E con las 9 botellas (por ejemplo, cargar la escena final).")]
    public UnityEvent onRitualStart;

    private void OnEnable()
    {
        GameProgress.OnCollectedChanged += OnProgressChanged;
        Refresh();
    }

    private void OnDisable()
    {
        GameProgress.OnCollectedChanged -= OnProgressChanged;
    }

    private void OnProgressChanged(int total)
    {
        Refresh();
    }

    private void Refresh()
    {
        if (readyGlow != null) readyGlow.SetActive(GameProgress.AllCollected);
    }

    public void Interact(GameObject player)
    {
        if (GameProgress.AllCollected)
        {
            if (onRitualStart != null) onRitualStart.Invoke();
        }
        else
        {
            if (onNotReady != null) onNotReady.Invoke();
        }
    }
}