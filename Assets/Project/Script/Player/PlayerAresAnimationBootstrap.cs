using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class PlayerAresAnimationBootstrap : MonoBehaviour
{
    private const string AresObjectName = "Player(Ares)";
    private const string ControllerResourcePath = "Player/AresAnimatorController";

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
        GameObject ares = GameObject.Find(AresObjectName);
        if (ares == null)
        {
            return;
        }

        Animator animator = ares.GetComponent<Animator>();
        if (animator == null)
        {
            animator = ares.AddComponent<Animator>();
        }

        if (animator.runtimeAnimatorController == null)
        {
            RuntimeAnimatorController controller = Resources.Load<RuntimeAnimatorController>(ControllerResourcePath);
            if (controller == null)
            {
                Debug.LogError($"PlayerAresAnimationBootstrap: no se pudo cargar '{ControllerResourcePath}' desde Resources.");
            }
            else
            {
                animator.runtimeAnimatorController = controller;
            }
        }

        AresAnimatorController aresAnimator = ares.GetComponent<AresAnimatorController>();
        if (aresAnimator == null)
        {
            aresAnimator = ares.AddComponent<AresAnimatorController>();
        }

        aresAnimator.Configure(animator);
    }
}
