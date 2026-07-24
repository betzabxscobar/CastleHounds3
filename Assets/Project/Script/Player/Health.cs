using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSalud : MonoBehaviour
{
    [Header("Vida del jugador")]
    [SerializeField] private float vidaMaxima = 100f;

    [Header("Al morir")]
    [Tooltip("Escena de derrota que se carga cuando el jugador muere en combate.")]
    [SerializeField] private string nombreEscenaDerrota = "Perdiste";

    [Tooltip("Segundos antes de cargar la escena de derrota (para ver la animación).")]
    [SerializeField, Min(0f)] private float retrasoDerrota = 1.5f;

    private float vidaActual;
    private bool muerto;
    private bool cargandoDerrota;

    public float VidaActual => vidaActual;
    public float VidaMaxima => vidaMaxima;
    public bool Muerto => muerto;

    private void Start()
    {
        vidaActual = vidaMaxima;

        Debug.Log("Vida inicial: " + vidaActual);
    }

    public void RecibirDanio(float danio)
    {
        if (muerto)
            return;

        vidaActual -= danio;

        if (vidaActual < 0)
            vidaActual = 0;

        Debug.Log("Vida del jugador: " + vidaActual + " / " + vidaMaxima);

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    public void Curar(float cantidad)
    {
        if (muerto)
            return;

        vidaActual += cantidad;

        if (vidaActual > vidaMaxima)
            vidaActual = vidaMaxima;

        Debug.Log("Vida del jugador: " + vidaActual + " / " + vidaMaxima);
    }

    private void Morir()
    {
        muerto = true;

        Debug.Log("El jugador ha muerto");

        // Cargar la pantalla de derrota tras un pequeño retraso.
        if (retrasoDerrota <= 0f)
        {
            CargarEscenaDerrota();
        }
        else
        {
            Invoke(nameof(CargarEscenaDerrota), retrasoDerrota);
        }
    }


    private void CargarEscenaDerrota()
    {
        if (cargandoDerrota)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(nombreEscenaDerrota))
        {
            Debug.LogError("PlayerSalud: 'nombreEscenaDerrota' está vacío; no se puede cargar la derrota.");
            return;
        }

        cargandoDerrota = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(nombreEscenaDerrota);
    }
}