using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class Challenge04SceneFlowController : MonoBehaviour
{
    private const string PotionSceneName = "MiniJuegoPocion";
    private const string DemoSceneName = "Demo";
    private const string PlayerName = "Player_Dog_Model";
    private const int ReturnFrameCount = 5;
    private const float GroundProbeHeight = 30f;
    private const float GroundProbeDistance = 80f;

    private static Challenge04SceneFlowController runner;
    private static bool pendingReturn;
    private static string pendingChallengeId;
    private static ChallengeResult pendingResult = ChallengeResult.None;
    private static Vector3 pendingReturnPosition;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RegisterSceneHandlers()
    {
        EnsureRunner();

        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    public static void EnterPotionScene(string challengeId)
    {
        if (!Application.CanStreamedLevelBeLoaded(PotionSceneName))
        {
            Debug.LogError($"Challenge04SceneFlowController: la escena '{PotionSceneName}' no esta en Build Settings.");
            return;
        }

        EnsureRunner();

        GameObject player = GameObject.Find(PlayerName);
        if (player == null)
        {
            Debug.LogError($"Challenge04SceneFlowController: no se encontro {PlayerName} antes de entrar a {PotionSceneName}.");
        }

        pendingChallengeId = challengeId;
        pendingReturnPosition = player != null ? player.transform.position : Vector3.zero;

        Time.timeScale = 1f;
        SceneManager.LoadScene(PotionSceneName);
    }

    public static void CompleteAndReturn(ChallengeResult result)
    {
        EnsureRunner();

        pendingReturn = true;
        pendingResult = result;

        Time.timeScale = 1f;
        SceneManager.LoadScene(DemoSceneName);
    }

    private static void EnsureRunner()
    {
        if (runner != null)
        {
            return;
        }

        GameObject runnerObject = new GameObject(nameof(Challenge04SceneFlowController));
        runner = runnerObject.AddComponent<Challenge04SceneFlowController>();
        DontDestroyOnLoad(runnerObject);
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != DemoSceneName || !pendingReturn)
        {
            return;
        }

        pendingReturn = false;
        ChallengeResult result = pendingResult;
        string challengeId = pendingChallengeId;
        pendingResult = ChallengeResult.None;
        pendingChallengeId = null;

        if (result == ChallengeResult.Won && !string.IsNullOrWhiteSpace(challengeId))
        {
            ChallengeProgressManager progressManager = ChallengeProgressManager.Instance;
            if (progressManager != null)
            {
                progressManager.CompleteChallenge(challengeId);
            }
            else
            {
                Debug.LogError("Challenge04SceneFlowController: falta ChallengeProgressManager al volver a Demo.");
            }
        }

        EnsureRunner();
        runner.StartCoroutine(ApplyReturnAfterDemoStartup());
    }

    private static IEnumerator ApplyReturnAfterDemoStartup()
    {
        for (int frame = 0; frame < ReturnFrameCount; frame++)
        {
            yield return null;
            SkipDemoIntroIfPresent();

            yield return new WaitForEndOfFrame();
            TeleportPlayerToReturnPosition();
        }
    }

    private static void SkipDemoIntroIfPresent()
    {
        CinematicIntroController introController = Object.FindAnyObjectByType<CinematicIntroController>();
        if (introController != null)
        {
            introController.SkipIntro();
        }
    }

    private static void TeleportPlayerToReturnPosition()
    {
        GameObject player = GameObject.Find(PlayerName);
        if (player == null)
        {
            Debug.LogError($"Challenge04SceneFlowController: no se encontro {PlayerName} para teletransportar al volver a {DemoSceneName}.");
            return;
        }

        Time.timeScale = 1f;

        CharacterController characterController = player.GetComponent<CharacterController>();
        bool restoreCharacterController = characterController != null && characterController.enabled;
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        Vector3 groundedReturnPosition = ResolveGroundedReturnPosition(pendingReturnPosition, characterController);

        Rigidbody rigidbody = player.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            rigidbody.position = groundedReturnPosition;
        }

        player.transform.position = groundedReturnPosition;

        if (characterController != null)
        {
            characterController.enabled = restoreCharacterController;
        }

        Debug.Log($"Challenge04SceneFlowController: jugador teletransportado a {groundedReturnPosition} al volver de {PotionSceneName}.");
    }

    private static Vector3 ResolveGroundedReturnPosition(Vector3 desiredPosition, CharacterController characterController)
    {
        Vector3 probeOrigin = desiredPosition + Vector3.up * GroundProbeHeight;
        if (!Physics.Raycast(probeOrigin, Vector3.down, out RaycastHit hit, GroundProbeDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            return desiredPosition;
        }

        float groundedY = hit.point.y;
        if (characterController != null)
        {
            groundedY += characterController.height * 0.5f - characterController.center.y;
        }

        return new Vector3(desiredPosition.x, groundedY, desiredPosition.z);
    }
}
