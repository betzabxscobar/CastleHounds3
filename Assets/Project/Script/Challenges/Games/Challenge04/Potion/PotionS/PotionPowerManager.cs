using UnityEngine;


public class PotionPowerManager : MonoBehaviour
{

    public static PotionPowerManager Instance;


    public ElementType currentPower;


    private bool powerEnabled;



    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }





    public void SetPower(ElementType power)
    {

        currentPower = power;


        Debug.Log(
            "Poder guardado: " + currentPower
        );

    }





    public ElementType GetPower()
    {

        return currentPower;

    }





    // Activar poder al entrar a combate

    public void EnablePower()
    {

        powerEnabled = true;


        Debug.Log(
            "Poder habilitado para combate"
        );

    }





    // Desactivar al volver al mundo

    public void DisablePower()
    {

        powerEnabled = false;


        currentPower = ElementType.None;


        Debug.Log(
            "Poder eliminado"
        );

    }





    // Saber si puede usarse

    public bool CanUsePower()
    {

        return powerEnabled &&
               currentPower != ElementType.None;

    }





    public bool HasPower()
    {

        return currentPower != ElementType.None;

    }

}