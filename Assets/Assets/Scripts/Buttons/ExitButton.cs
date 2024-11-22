#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void SalirDelJuego()
    {
#if UNITY_EDITOR
        // Detener la reproducción en el editor
        EditorApplication.isPlaying = false;
#else
        // Salir de la aplicación
        Application.Quit();
#endif
    }
}
