using UnityEngine;
using UnityEngine.InputSystem;


public class DogCombat : MonoBehaviour
{

    private PlayerControls controls;

    private DogControl dogControl;


    private void Awake()
    {
        dogControl = GetComponent<DogControl>();


        controls = new PlayerControls();



        // Ataque 1 Q

        controls.Player.Attack01.performed += ctx =>
        {
            Attack01();
        };



        // Ataque 2 E

        controls.Player.Attack02.performed += ctx =>
        {
            Attack02();
        };



        // Defensa F

        controls.Player.Defend.performed += ctx =>
        {
            StartDefend();
        };


        controls.Player.Defend.canceled += ctx =>
        {
            StopDefend();
        };


    }




    private void OnEnable()
    {
        controls.Enable();
    }



    private void OnDisable()
    {
        controls.Disable();
    }





    private void Attack01()
    {

        if (dogControl != null)
        {
            dogControl.Attack01();
        }


        Debug.Log("Ataque 1");

    }





    private void Attack02()
    {

        if (dogControl != null)
        {
            dogControl.Attack02();
        }


        Debug.Log("Ataque 2");

    }





    private void StartDefend()
    {

        if (dogControl != null)
        {
            dogControl.Defend(true);
        }


        Debug.Log("Defendiendo");

    }





    private void StopDefend()
    {

        if (dogControl != null)
        {
            dogControl.Defend(false);
        }


    }

}