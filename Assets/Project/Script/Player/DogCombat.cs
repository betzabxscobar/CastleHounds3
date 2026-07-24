using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class DogCombat : MonoBehaviour
{
    [Header("Espada")]
    [Tooltip("Si se deja vacío, se busca un DogWeapon en los hijos.")]
    [SerializeField] private DogWeapon weapon;

    [Tooltip("Cuánto tiempo queda activo el hitbox de la espada por ataque.")]
    [SerializeField] private float hitboxDuration = 0.4f;


    private PlayerControls controls;

    private DogControl dogControl;

    private Coroutine swingRoutine;


    private void Awake()
    {
        dogControl = GetComponent<DogControl>();

        if (weapon == null)
        {
            weapon = GetComponentInChildren<DogWeapon>(true);
        }


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


        Golpear();


        Debug.Log("Ataque 1");

    }





    private void Attack02()
    {

        if (dogControl != null)
        {
            dogControl.Attack02();
        }


        Golpear();


        Debug.Log("Ataque 2");

    }




    // Activa el hitbox de la espada durante un instante para que el
    // golpe conecte con el enemigo (sin depender de eventos de animación).
    private void Golpear()
    {
        if (weapon == null)
        {
            return;
        }

        if (swingRoutine != null)
        {
            StopCoroutine(swingRoutine);
        }

        swingRoutine = StartCoroutine(SwingRoutine());
    }




    private IEnumerator SwingRoutine()
    {
        weapon.EnableHitbox();

        yield return new WaitForSeconds(hitboxDuration);

        weapon.DisableHitbox();

        swingRoutine = null;
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