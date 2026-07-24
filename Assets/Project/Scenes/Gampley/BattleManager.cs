using UnityEngine;


public class BattleManager : MonoBehaviour
{

    public static BattleManager Instance;


    [Header("Punto donde aparece el enemigo")]
    public Transform enemySpawn;



    [Header("Enemigo por defecto")]
    [Tooltip("Se usa cuando la receta no trae enemigo o no hay receta seleccionada. " +
             "Garantiza que SIEMPRE aparezca un enemigo en la BattleArena.")]
    public GameObject defaultEnemyPrefab;



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

        // Intentar usar el enemigo de la receta seleccionada.
        GameObject prefabAUsar = null;

        if (PotionDataManager.Instance != null)
        {
            RecipeData recipe = PotionDataManager.Instance.GetRecipe();

            if (recipe != null && recipe.enemyPrefab != null)
            {
                prefabAUsar = recipe.enemyPrefab;
            }
        }



        // Si la receta no trajo enemigo (o no hay receta), usar el por defecto.
        // Así SIEMPRE aparece un enemigo en la BattleArena.
        if (prefabAUsar == null)
        {
            prefabAUsar = defaultEnemyPrefab;

            Debug.LogWarning(
                "BattleManager: la receta no tenía enemigo asignado. "
                + "Usando el enemigo por defecto."
            );
        }



        if (prefabAUsar == null)
        {
            Debug.LogError(
                "BattleManager: no hay enemigo que generar. "
                + "Asigna un 'Default Enemy Prefab' en el inspector."
            );

            return;
        }



        // Posición de aparición: el punto asignado o, si falta, la del propio manager.
        Vector3 spawnPos = enemySpawn != null ? enemySpawn.position : transform.position;
        Quaternion spawnRot = enemySpawn != null ? enemySpawn.rotation : transform.rotation;



        currentEnemy = Instantiate(
            prefabAUsar,
            spawnPos,
            spawnRot
        );



        Debug.Log(
            "Enemigo generado: "
            + currentEnemy.name
        );

    }

}