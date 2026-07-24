using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class DemoCameraBootstrap : MonoBehaviour
{
    private const string PlayerObjectName = "Player";
    private const string ExplorationCameraName = "CamaraExploracion";

    private static readonly string[] UnusedCameraNames =
    {
        "CamaraEntrada",
        "CamaraVistaCastillo",
        "CamaraPerro"
    };

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RegisterSceneHandler()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;
        TryInstall(SceneManager.GetActiveScene());
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryInstall(scene);
    }

    private static void TryInstall(Scene scene)
    {
        GameObject explorationCameraObject = GameObject.Find(ExplorationCameraName);
        if (explorationCameraObject == null)
        {
            Debug.LogError($"DemoCameraBootstrap: no se encontro {ExplorationCameraName}.");
            return;
        }

        GameObject player = GameObject.Find(PlayerObjectName);
        if (player == null)
        {
            Debug.LogError($"DemoCameraBootstrap: no se encontro {PlayerObjectName} para apuntar la camara.");
            return;
        }

        CinemachineCamera explorationCamera = explorationCameraObject.GetComponent<CinemachineCamera>();
        if (explorationCamera == null)
        {
            Debug.LogError($"DemoCameraBootstrap: {ExplorationCameraName} no tiene CinemachineCamera.");
            return;
        }

        explorationCamera.Target = new CameraTarget
        {
            TrackingTarget = player.transform,
            LookAtTarget = player.transform
        };

        explorationCamera.Priority = new PrioritySettings { Enabled = true, Value = 100 };

        foreach (string unusedCameraName in UnusedCameraNames)
        {
            GameObject unusedCamera = GameObject.Find(unusedCameraName);
            if (unusedCamera == null)
            {
                continue;
            }

            CinemachineVirtualCameraBase unusedVcam = unusedCamera.GetComponent<CinemachineVirtualCameraBase>();
            if (unusedVcam != null)
            {
                unusedVcam.Priority = new PrioritySettings { Enabled = true, Value = -100 };
            }

            unusedCamera.SetActive(false);
        }

        Debug.Log($"DemoCameraBootstrap: {ExplorationCameraName} apuntando a {PlayerObjectName}.");
    }
}
