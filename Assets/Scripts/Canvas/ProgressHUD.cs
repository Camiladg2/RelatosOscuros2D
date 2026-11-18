using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressHUD : MonoBehaviour
{
    [Serializable]
    public class Slot
    {
        [Tooltip("La Image del icono (con el dibujo de esa botella).")]
        public Image icon;
        [Tooltip("El MISMO ID que tiene la botella en su Sphere Collectible.")]
        public string potionId = "";
    }

    [Tooltip("Solo las botellas de ESTE mundo (por ejemplo 3). No hace falta poner las 9.")]
    public Slot[] slots = new Slot[3];

    [Header("Colores")]
    public Color collectedColor = Color.white;
    public Color missingColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);

    [Header("Texto opcional (ej: 2 / 3)")]
    public Text counterText;
    [Tooltip("Marcado: el contador cuenta solo las botellas de este mundo. Desmarcado: cuenta las 9 del juego.")]
    public bool contarSoloEstosSlots = true;

    private void OnEnable()
    {
        GameProgress.OnCollectedChanged += OnChanged;
        Refresh();
    }

    private void OnDisable()
    {
        GameProgress.OnCollectedChanged -= OnChanged;
    }

    private void OnChanged(int total)
    {
        Refresh();
    }

    public void Refresh()
    {
        int conseguidas = 0;
        int validas = 0;

        if (slots != null)
        {
            foreach (Slot s in slots)
            {
                if (s == null || s.icon == null) continue;
                validas++;

                bool have = !string.IsNullOrEmpty(s.potionId) && GameProgress.IsCollected(s.potionId);
                if (have) conseguidas++;
                s.icon.color = have ? collectedColor : missingColor;
            }
        }

        if (counterText != null)
        {
            if (contarSoloEstosSlots)
                counterText.text = conseguidas + " / " + validas;
            else
                counterText.text = GameProgress.Total + " / " + GameProgress.TotalRequired;
        }
    }

    [ContextMenu("Borrar progreso (solo para pruebas)")]
    private void ResetForTesting()
    {
        GameProgress.ResetAll();
        Refresh();
    }
}