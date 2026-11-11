using UnityEngine;

/// <summary>Cualquier objeto con este interfaz puede ser activado con la tecla E.</summary>
public interface IInteractable
{
    void Interact(GameObject player);
}
