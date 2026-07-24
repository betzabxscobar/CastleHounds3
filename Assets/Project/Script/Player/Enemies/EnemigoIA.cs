using UnityEngine;
using UnityEngine.AI;

public class EnemigoIA : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidad = 3f;

    [Header("Combate")]
    [SerializeField] private float distanciaAtaque = 2f;
    [SerializeField] private float tiempoEntreAtaques = 2f;
    [SerializeField] private float daño = 10f;
    [Tooltip("Retraso para aplicar el daño y que coincida con la animación de ataque.")]
    [SerializeField] private float retrasoImpacto = 0.5f;


    private NavMeshAgent agente;
    private Transform jugador;

    private EnemigoAnimacion animacion;

    private PlayerSalud saludJugador;
    private DogControl controlJugador;

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


        if (animacion != null)
        {
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


        // Aplicar el daño al jugador tras un pequeño retraso, para que
        // coincida con el momento del golpe en la animación.
        StartCoroutine(AplicarDañoJugador());
    }




    private System.Collections.IEnumerator AplicarDañoJugador()
    {
        yield return new WaitForSeconds(retrasoImpacto);


        if (jugador == null || saludJugador == null)
        {
            yield break;
        }


        // Solo conecta si el jugador sigue en rango de ataque.
        float distancia =
            Vector3.Distance(transform.position, jugador.position);


        if (distancia <= distanciaAtaque + 0.5f)
        {
            saludJugador.RecibirDanio(daño);


            if (controlJugador != null)
            {
                controlJugador.GetHit();
            }
        }
    }





    private void BuscarJugador()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");


        if (player != null)
        {
            jugador = player.transform;
            saludJugador = player.GetComponent<PlayerSalud>();
            controlJugador = player.GetComponent<DogControl>();

            IgnorarColisionConJugador(player);
        }
        else
        {
            Debug.LogError(
                "No existe Player con Tag Player"
            );
        }
    }



    // Evita que el enemigo EMPUJE físicamente al jugador. El NavMeshAgent
    // avanza hacia el jugador y su CapsuleCollider lo desplazaría al chocar.
    // Ignorando la colisión entre los cuerpos del enemigo y del jugador, el
    // enemigo solo aplica daño (por corrutina) sin moverlo. No afecta a las
    // hitbox de ataque (esas usan triggers/overlap aparte).
    private void IgnorarColisionConJugador(GameObject player)
    {
        Collider[] colsEnemigo = GetComponentsInChildren<Collider>();
        Collider[] colsJugador = player.GetComponentsInChildren<Collider>();


        foreach (Collider ce in colsEnemigo)
        {
            // No tocar los triggers (hitbox de ataque), solo cuerpos sólidos.
            if (ce.isTrigger)
                continue;


            foreach (Collider cj in colsJugador)
            {
                if (cj.isTrigger)
                    continue;


                Physics.IgnoreCollision(ce, cj, true);
            }
        }
    }
}