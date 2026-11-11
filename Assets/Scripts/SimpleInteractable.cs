using UnityEngine;
using UnityEngine.Events;

/// <summary>Objeto interactuable sencillo: palancas, puertas, carteles. Conecta acciones en el Inspector.</summary>
public class SimpleInteractable : MonoBehaviour, IInteractable
{
    public UnityEvent onInteract;

    public void Interact(GameObject player)
    {
        if (onInteract != null) onInteract.Invoke();
    }
}
