using UnityEngine;

public class DogControl : MonoBehaviour
{
    private Animator animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }



    public void SetSpeed(float speed)
    {
        animator.SetFloat(
            "Speed",
            speed
        );
    }



    public void Attack01()
    {
        animator.SetTrigger(
            "Attack01"
        );
    }



    public void Attack02()
    {
        animator.SetTrigger(
            "Attack02"
        );
    }



    public void Power()
    {
        animator.SetTrigger(
            "Power"
        );
    }



    public void Defend(bool value)
    {
        animator.SetBool(
            "Defend",
            value
        );
    }



    public void GetHit()
    {
        animator.SetTrigger(
            "GetHit"
        );
    }



    public void Dizzy()
    {
        animator.SetTrigger(
            "Dizzy"
        );
    }



    public void Die()
    {
        animator.SetTrigger(
            "Die"
        );
    }
}