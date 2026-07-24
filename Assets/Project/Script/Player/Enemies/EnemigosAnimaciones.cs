using UnityEngine;

public class EnemigoAnimacion : MonoBehaviour
{
    private Animator animator;


    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            Debug.LogError("No se encontró Animator");
        }
    }


    public void Caminar(float velocidad)
    {
        animator.SetFloat("Speed", velocidad);
    }


    public void Ataque1()
    {
        animator.SetTrigger("Attack1");
    }


    public void Ataque2()
    {
        animator.SetTrigger("Attack2");
    }


    public void Ataque3()
    {
        animator.SetTrigger("Attack3");
    }


    public void RecibirGolpe()
    {
        animator.SetTrigger("Hit");
    }


    public void Aturdir()
    {
        animator.SetTrigger("Stun");
    }


    public void Morir()
    {
        animator.SetTrigger("Die");
    }
}