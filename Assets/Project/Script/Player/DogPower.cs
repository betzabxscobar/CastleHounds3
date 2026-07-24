using UnityEngine;
using UnityEngine.InputSystem;

public class DogPower : MonoBehaviour
{
    private PlayerControls controls;

    private DogControl dogControl;

    private bool canUsePower;



    private void Awake()
    {
        dogControl = GetComponent<DogControl>();

        controls = new PlayerControls();

        controls.Player.Power.performed += ctx =>
        {
            UsePower();
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



    private void Start()
    {
        canUsePower =
            UnityEngine.SceneManagement.SceneManager
            .GetActiveScene().name == "BattleArena";
    }



    void UsePower()
    {
        Debug.Log("Intentando usar poder...");


        if (!canUsePower)
        {
            Debug.Log("Poder bloqueado: no estamos en Combats");
            return;
        }



        if (PotionPowerManager.Instance == null)
        {
            Debug.Log("No existe PotionPowerManager");
            return;
        }



        if (!PotionPowerManager.Instance.HasPower())
        {
            Debug.Log("No hay poder guardado");
            return;
        }




        ElementType power =
            PotionPowerManager.Instance.GetPower();



        Debug.Log(
            "PODER USADO: " + power
        );



        dogControl.Power();



        switch (power)
        {

            case ElementType.Fuego:

                Debug.Log("Ejecutando poder FUEGO");
                break;



            case ElementType.Hielo:

                Debug.Log("Ejecutando poder HIELO");
                break;



            case ElementType.Veneno:

                Debug.Log("Ejecutando poder VENENO");
                break;



            case ElementType.Toxicidad:

                Debug.Log("Ejecutando poder TOXICIDAD");
                break;



            case ElementType.Roca:

                Debug.Log("Ejecutando poder ROCA");
                break;



            case ElementType.Rayo:

                Debug.Log("Ejecutando poder RAYO");
                break;



            case ElementType.Viento:

                Debug.Log("Ejecutando poder VIENTO");
                break;


            default:

                Debug.Log("Poder desconocido");

                break;

        }
    }
}