using UnityEngine;
using UnityEngine.AI;

public class EnemigoIA : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 3f;

    [Header("Combate")]
    [SerializeField] private float distanciaAtaque = 2f;
    [SerializeField] private float tiempoEntreAtaques = 2f;


    private NavMeshAgent agente;
    private Transform jugador;

    private EnemigoAnimacion animacion;

    private float contadorAtaque;



    private void Awake()
    {
        agente = GetComponent<NavMeshAgent>();
        animacion = GetComponent<EnemigoAnimacion>();
    }



    private void Start()
    {
        agente.speed = velocidad;

        BuscarJugador();
    }



    private void Update()
    {
        if (jugador == null)
            return;


        float distancia =
            Vector3.Distance(
                transform.position,
                jugador.position
            );



        // Jugador lejos

        if (distancia > distanciaAtaque)
        {
            Perseguir();

        }
        else
        {
            Atacar();
        }

    }



    private void Perseguir()
    {
        agente.isStopped = false;


        agente.SetDestination(
            jugador.position
        );


        // Animación caminar

        if (animacion != null)
        {
            animacion.Caminar(1);
        }
    }




    private void Atacar()
    {
        agente.isStopped = true;


        // mirar al jugador

        Vector3 direccion =
            jugador.position - transform.position;


        direccion.y = 0;


        if (direccion != Vector3.zero)
        {
            transform.rotation =
                Quaternion.LookRotation(direccion);
        }



        if (animacion != null)
        {
            animacion.Caminar(0);
        }



        contadorAtaque += Time.deltaTime;


        if (contadorAtaque >= tiempoEntreAtaques)
        {
            LanzarAtaque();

            contadorAtaque = 0;
        }
    }




    private void LanzarAtaque()
    {
        int ataque =
            Random.Range(1, 4);


        switch (ataque)
        {
            case 1:
                animacion.Ataque1();
                break;


            case 2:
                animacion.Ataque2();
                break;


            case 3:
                animacion.Ataque3();
                break;
        }
    }





    private void BuscarJugador()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");


        if (player != null)
        {
            jugador = player.transform;
        }
        else
        {
            Debug.LogError(
                "No existe Player con Tag Player"
            );
        }
    }
}