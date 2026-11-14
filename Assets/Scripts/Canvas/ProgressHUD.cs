using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contador de botellas en pantalla: un icono por cada botella.
/// Cada icono esta ligado al ID de UNA botella concreta. Cuando se recoge esa botella,
/// su icono se ilumina; mientras no, se ve oscuro.
/// </summary>
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

    [Tooltip("Un elemento por botella (9 en total).")]
    public Slot[] slots = new Slot[9];

    [Header("Colores")]
    public Color collectedColor = Color.white;
    public Color missingColor = new Color(0.25f, 0.25f, 0.25f, 0.8f);

    [Header("Texto opcional (ej: 4 / 9)")]
    public Text counterText;

    void OnEnable()
    {
        GameProgress.OnCollectedChanged += OnChanged;
        Refresh();
    }

    void OnDisable()
    {
        GameProgress.OnCollectedChanged -= OnChanged;
    }

    void OnChanged(int total)
    {
        Refresh();
    }

    public void Refresh()
    {
        if (slots != null)
        {
            foreach (Slot s in slots)
            {
                if (s == null || s.icon == null) continue;
                bool have = !string.IsNullOrEmpty(s.potionId) && GameProgress.IsCollected(s.potionId);
                s.icon.color = have ? collectedColor : missingColor;
            }
        }

        if (counterText != null)
            counterText.text = GameProgress.Total + " / " + GameProgress.TotalRequired;
    }

    [ContextMenu("Borrar progreso (solo para pruebas)")]
    void ResetForTesting()
    {
        GameProgress.ResetAll();
        Refresh();
    }
}