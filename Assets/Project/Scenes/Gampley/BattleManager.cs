using UnityEngine;


public class BattleManager : MonoBehaviour
{

    public static BattleManager Instance;


    [Header("Punto donde aparece el enemigo")]
    public Transform enemySpawn;



    private GameObject currentEnemy;



    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }





    private void Start()
    {
        SpawnEnemy();
    }





    void SpawnEnemy()
    {

        if (PotionDataManager.Instance == null)
        {
            Debug.LogError(
                "No existe PotionDataManager"
            );

            return;
        }



        RecipeData recipe =
        PotionDataManager.Instance.GetRecipe();



        if (recipe == null)
        {
            Debug.LogError(
                "No existe receta seleccionada"
            );

            return;
        }



        if (recipe.enemyPrefab == null)
        {
            Debug.LogError(
                "La receta no tiene enemigo asignado"
            );

            return;
        }




        currentEnemy = Instantiate(
            recipe.enemyPrefab,
            enemySpawn.position,
            enemySpawn.rotation
        );



        Debug.Log(
            "Enemigo generado: "
            + currentEnemy.name
        );

    }

}