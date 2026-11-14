using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Carga una escena por nombre. Se conecta a un UnityEvent (por ejemplo el del caldero).
/// La escena debe estar en File > Build Profiles > Scene List.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError("[SceneLoader] La escena '" + sceneName + "' no esta en Build Profiles o el nombre esta mal escrito.", this);
            return;
        }
        SceneManager.LoadScene(sceneName);
    }
}
