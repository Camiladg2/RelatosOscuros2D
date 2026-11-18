using UnityEngine;

public class AbrirCreditosAlIniciar : MonoBehaviour
{
    public static bool mostrarCreditosAlCargar = false;

    [Header("Arrastra aquí tu Panel de Créditos")]
    public GameObject panelCreditos;

    private void Start()
    {
        if (mostrarCreditosAlCargar)
        {
            mostrarCreditosAlCargar = false; // Reinicia la bandera para futuras partidas
            if (panelCreditos != null)
            {
                panelCreditos.SetActive(true);
            }
        }
    }
}