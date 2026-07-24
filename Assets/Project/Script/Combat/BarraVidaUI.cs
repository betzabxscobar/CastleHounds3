using UnityEngine;
using UnityEngine.UI;


// -------------------------------------------------------------
// Barra de vida para el jugador o el enemigo.
//
// Uso:
//   1. Crea una UI Image (Image Type = Filled, Fill Method = Horizontal)
//      dentro de un Canvas.
//   2. Ponle este componente.
//   3. Elige "objetivo" (Jugador o Enemigo) y arrastra la Image al campo
//      "relleno" (o se toma la Image de este mismo objeto).
//
// El enemigo se genera en tiempo de ejecución, así que la barra lo busca
// sola cuando aparece.
// -------------------------------------------------------------
public class BarraVidaUI : MonoBehaviour
{
    public enum Objetivo
    {
        Jugador,
        Enemigo
    }


    [Header("Configuración")]
    [SerializeField] private Objetivo objetivo = Objetivo.Jugador;

    [Tooltip("Image con Image Type = Filled. Si se deja vacío se usa la de este objeto.")]
    [SerializeField] private Image relleno;

    [Tooltip("Opcional: se oculta la barra si no hay objetivo (útil para el enemigo).")]
    [SerializeField] private bool ocultarSinObjetivo = false;

    [Header("Suavizado")]
    [SerializeField] private float velocidadSuavizado = 8f;


    private PlayerSalud saludJugador;
    private EnemigoSalud saludEnemigo;

    private float objetivoFill = 1f;


    private void Awake()
    {
        if (relleno == null)
        {
            relleno = GetComponent<Image>();
        }
    }


    private void Update()
    {
        if (relleno == null)
        {
            return;
        }

        bool hayObjetivo = ActualizarObjetivoFill();

        if (ocultarSinObjetivo)
        {
            relleno.enabled = hayObjetivo;
        }

        // Interpolar suavemente hacia el valor real.
        relleno.fillAmount = Mathf.MoveTowards(
            relleno.fillAmount,
            objetivoFill,
            velocidadSuavizado * Time.deltaTime
        );
    }


    // Devuelve true si encontró un objetivo vivo del que leer la vida.
    private bool ActualizarObjetivoFill()
    {
        if (objetivo == Objetivo.Jugador)
        {
            if (saludJugador == null)
            {
                saludJugador = FindAnyObjectByType<PlayerSalud>();
            }

            if (saludJugador == null)
            {
                return false;
            }

            objetivoFill = Calcular(saludJugador.VidaActual, saludJugador.VidaMaxima);
            return true;
        }
        else
        {
            // El enemigo puede no existir aún (se instancia al empezar la pelea)
            // o haber muerto (destruido). En ambos casos se vuelve a buscar.
            if (saludEnemigo == null)
            {
                saludEnemigo = FindAnyObjectByType<EnemigoSalud>();
            }

            if (saludEnemigo == null)
            {
                objetivoFill = 0f;
                return false;
            }

            objetivoFill = Calcular(saludEnemigo.VidaActual, saludEnemigo.VidaMaxima);
            return true;
        }
    }


    private static float Calcular(float actual, float maxima)
    {
        if (maxima <= 0f)
        {
            return 0f;
        }

        return Mathf.Clamp01(actual / maxima);
    }
}
