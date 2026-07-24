using UnityEngine;

public class EnemigoSalud : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private float vidaMaxima = 90f;

    private float vidaActual;
    private bool muerto;


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


        vidaActual -= daño;


        Debug.Log(
            "Enemigo recibió " + daño +
            " daño. Vida: " + vidaActual
        );



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


        // Después aquí llamaremos:
        // animación Die
        // destruir enemigo
    }
}