using UnityEngine;

public class EnemigoSalud : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private float vidaMaxima = 90f;

    [Header("Al morir")]
    [SerializeField] private float retrasoDestruir = 2f;

    private float vidaActual;
    private bool muerto;

    private EnemigoAnimacion animacion;

    // Getters para la barra de vida.
    public float VidaActual => vidaActual;
    public float VidaMaxima => vidaMaxima;
    public bool Muerto => muerto;


    private void Awake()
    {
        animacion = GetComponent<EnemigoAnimacion>();
    }


    private void Start()
    {
        vidaActual = vidaMaxima;

        Debug.Log(
            "Vida enemigo: " + vidaActual
        );
    }



    public void RecibirDanio(float daño)
    {
        if (muerto)
            return;


        vidaActual = Mathf.Max(0f, vidaActual - daño);


        Debug.Log(
            "Enemigo recibió " + daño +
            " daño. Vida: " + vidaActual
        );


        if (animacion != null && vidaActual > 0f)
        {
            animacion.RecibirGolpe();
        }


        if (vidaActual <= 0)
        {
            Morir();
        }
    }



    private void Morir()
    {
        muerto = true;


        Debug.Log(
            "Enemigo muerto"
        );


        if (animacion != null)
        {
            animacion.Morir();
        }


        // Desactivar IA y colisiones para que ya no ataque ni estorbe.
        EnemigoIA ia = GetComponent<EnemigoIA>();
        if (ia != null)
        {
            ia.enabled = false;
        }

        UnityEngine.AI.NavMeshAgent agente = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agente != null)
        {
            agente.enabled = false;
        }


        // Destruir tras la animación de muerte.
        Destroy(gameObject, retrasoDestruir);
    }
}