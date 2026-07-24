using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BattleManager : MonoBehaviour
{

    public static BattleManager Instance;


    [Header("Punto donde aparece el enemigo")]
    public Transform enemySpawn;

    [Tooltip("Altura extra al generar el enemigo, para que no aparezca hundido en el piso.")]
    public float spawnYOffset = 1f;



    [Header("Enemigo por defecto")]
    [Tooltip("Se usa cuando la receta no trae enemigo o no hay receta seleccionada. " +
             "Garantiza que SIEMPRE aparezca un enemigo en la BattleArena.")]
    public GameObject defaultEnemyPrefab;



    [Header("Regreso al ganar")]
    [Tooltip("Escena a la que se vuelve tras ganar la pelea (el overworld con las casas).")]
    public string returnScene = "Demo 1";

    [Tooltip("Segundos de espera tras la muerte del enemigo antes de volver, para que " +
             "se vea su animación de muerte.")]
    [Min(0f)]
    public float returnDelay = 2.5f;



    private GameObject currentEnemy;

    // ID del reto de la casa que lanzó esta pelea. Se toma de la receta y se
    // marca como completado SOLO al ganar. Vacío = no marca ningún reto.
    private string retoCasaId;

    private bool victoriaProcesada;



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

            if (recipe != null)
            {
                // Guardar el reto de la casa para marcarlo al ganar la pelea.
                retoCasaId = recipe.challengeId;

                if (recipe.enemyPrefab != null)
                {
                    prefabAUsar = recipe.enemyPrefab;
                }
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

        // Subir el enemigo para que no aparezca hundido en el piso.
        spawnPos.y += spawnYOffset;



        currentEnemy = Instantiate(
            prefabAUsar,
            spawnPos,
            spawnRot
        );



        Debug.Log(
            "Enemigo generado: "
            + currentEnemy.name
        );


        // Escuchar la muerte del enemigo para procesar la victoria.
        EnemigoSalud saludEnemigo = currentEnemy.GetComponent<EnemigoSalud>();
        if (saludEnemigo != null)
        {
            saludEnemigo.OnMuerto += HandleVictoria;
        }
        else
        {
            Debug.LogWarning(
                "BattleManager: el enemigo no tiene EnemigoSalud; "
                + "no se podrá detectar la victoria."
            );
        }

    }




    // Se llama al morir el enemigo generado: marca el reto de la casa (si lo
    // hay) y vuelve al overworld. Protegido contra llamadas repetidas.
    private void HandleVictoria(EnemigoSalud enemigo)
    {
        if (victoriaProcesada)
        {
            return;
        }

        victoriaProcesada = true;

        if (enemigo != null)
        {
            enemigo.OnMuerto -= HandleVictoria;
        }


        MarcarRetoCompletado();


        StartCoroutine(VolverAlOverworld());
    }




    // Marca ÚNICAMENTE el reto asociado a la casa que lanzó esta pelea.
    // Es idempotente: repetir la pelea no completa retos de más.
    private void MarcarRetoCompletado()
    {
        if (string.IsNullOrWhiteSpace(retoCasaId))
        {
            // Enemigo por defecto / receta sin reto: no se marca nada.
            return;
        }

        ChallengeProgressManager progreso = ChallengeProgressManager.Instance;
        if (progreso == null)
        {
            Debug.LogError(
                "BattleManager: no existe ChallengeProgressManager; "
                + "no se pudo marcar el reto '" + retoCasaId + "'."
            );
            return;
        }

        if (!progreso.IsKnownChallengeId(retoCasaId))
        {
            Debug.LogError(
                "BattleManager: la receta trae un challengeId inválido '"
                + retoCasaId + "'. Revisa el RecipeData de la casa."
            );
            return;
        }

        progreso.CompleteChallenge(retoCasaId);

        Debug.Log(
            "BattleManager: reto de la casa marcado como completado: "
            + retoCasaId
        );
    }




    private IEnumerator VolverAlOverworld()
    {
        if (returnDelay > 0f)
        {
            yield return new WaitForSeconds(returnDelay);
        }

        Time.timeScale = 1f;

        if (string.IsNullOrWhiteSpace(returnScene))
        {
            Debug.LogError(
                "BattleManager: 'returnScene' está vacío; no se puede volver al overworld."
            );
            yield break;
        }

        SceneManager.LoadScene(returnScene);
    }

}