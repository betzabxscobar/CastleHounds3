using UnityEngine;

public class PlayerSalud : MonoBehaviour
{
    [Header("Vida del jugador")]
    [SerializeField] private float vidaMaxima = 100f;

    private float vidaActual;
    private bool muerto;

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

        // Aquí luego pondremos:
        // - Animación de muerte
        // - Pantalla de derrota
        // - Regresar al mapa
    }
}