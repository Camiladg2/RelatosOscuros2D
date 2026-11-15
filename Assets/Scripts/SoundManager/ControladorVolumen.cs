using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ControladorVolumen : MonoBehaviour
{
    public AudioMixer audioMixer; // Arrastra aquí tu AudioMixerPrincipal
    public Slider sliderMusica;   // Arrastra aquí el Slider de la música
    public Slider sliderSfx;      // Arrastra aquí el Slider de efectos

    private void Start()
    {
        // Carga el volumen guardado previamente o lo deja al máximo (1)
        if (sliderMusica != null)
        {
            sliderMusica.value = PlayerPrefs.GetFloat("VolumenMusica", 0.75f);
            CambiarVolumenMusica(sliderMusica.value);
        }

        if (sliderSfx != null)
        {
            sliderSfx.value = PlayerPrefs.GetFloat("VolumenSfx", 0.75f);
            CambiarVolumenSfx(sliderSfx.value);
        }
    }

    public void CambiarVolumenMusica(float valor)
    {
        // Convierte el valor lineal del slider (0 a 1) a decibelios logarítmicos (-80 a 0)
        float volumenDb = Mathf.Log10(Mathf.Clamp(valor, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("MusicaVolumen", volumenDb);
        PlayerPrefs.SetFloat("VolumenMusica", valor); // Guarda la preferencia
    }

    public void CambiarVolumenSfx(float valor)
    {
        float volumenDb = Mathf.Log10(Mathf.Clamp(valor, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("SfxVolumen", volumenDb);
        PlayerPrefs.SetFloat("VolumenSfx", valor);
    }
}