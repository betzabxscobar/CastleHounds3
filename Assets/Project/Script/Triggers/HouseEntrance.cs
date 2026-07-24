using UnityEngine;
using UnityEngine.SceneManagement;


public class HouseEntrance : MonoBehaviour
{

    [Header("Receta de esta casa")]
    public RecipeData houseRecipe;


    [Header("Escena del minijuego")]
    public string potionScene = "PotionScene";



    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {

            EnterHouse();

        }

    }




    void EnterHouse()
    {

        if (houseRecipe == null)
        {

            Debug.LogError(
                "La casa no tiene receta asignada"
            );

            return;

        }



        if (PotionDataManager.Instance == null)
        {

            Debug.LogError(
                "No existe PotionDataManager"
            );

            return;

        }



        // Guardar receta de la casa
        PotionDataManager.Instance.SetRecipe(
            houseRecipe
        );



        Debug.Log(
            "Receta enviada: "
            + houseRecipe.recipeName
        );



        // Ir al minijuego
        SceneManager.LoadScene(
            potionScene
        );

    }

}