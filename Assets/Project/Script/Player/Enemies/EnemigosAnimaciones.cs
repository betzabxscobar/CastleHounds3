using UnityEngine;

public class EnemigoAnimacion : MonoBehaviour
{
    private Animator animator;


    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
        {
            animator = GetComponentInParent<Animator>();
        }

        if (animator == null)
        {
            Debug.LogError("No se encontró Animator en " + name +
                ". Asigna un Animator al enemigo o a uno de sus hijos.");
        }
    }


    public void Caminar(float velocidad)
    {
        if (animator == null) return;
        animator.SetFloat("Speed", velocidad);
    }


    public void Ataque1()
    {
        if (animator == null) return;
        animator.SetTrigger("Attack1");
    }


    public void Ataque2()
    {
        if (animator == null) return;
        animator.SetTrigger("Attack2");
    }


    public void Ataque3()
    {
        if (animator == null) return;
        animator.SetTrigger("Attack3");
    }


    public void RecibirGolpe()
    {
        if (animator == null) return;
        animator.SetTrigger("Hit");
    }


    public void Aturdir()
    {
        if (animator == null) return;
        animator.SetTrigger("Stun");
    }


    public void Morir()
    {
        if (animator == null) return;
        animator.SetTrigger("Die");
    }
}