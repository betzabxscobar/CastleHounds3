using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class Challenge04PotionSceneBootstrap : MonoBehaviour
{
    private const string PotionSceneName = "MiniJuegoPocion";
    private const string ExitButtonName = "BtnSalir";

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
        if (scene.name != PotionSceneName)
        {
            return;
        }

        EnsureInputSystemEventSystem();
        EnsureExitButton();
    }

    private static void EnsureInputSystemEventSystem()
    {
        EventSystem eventSystem = Object.FindAnyObjectByType<EventSystem>();
        if (eventSystem == null)
        {
            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystem = eventSystemObject.AddComponent<EventSystem>();
        }

        StandaloneInputModule standaloneInputModule = eventSystem.GetComponent<StandaloneInputModule>();
        if (standaloneInputModule != null)
        {
            standaloneInputModule.enabled = false;
        }

        if (eventSystem.GetComponent<InputSystemUIInputModule>() == null)
        {
            eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
        }
    }

    private static void EnsureExitButton()
    {
        Canvas canvas = Object.FindAnyObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("Challenge04PotionSceneBootstrap: no hay Canvas para crear el boton de salida.");
            return;
        }

        Transform existing = canvas.transform.Find(ExitButtonName);
        if (existing != null)
        {
            return;
        }

        GameObject buttonObject = new GameObject(ExitButtonName);
        buttonObject.transform.SetParent(canvas.transform, false);

        RectTransform rectTransform = buttonObject.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1f, 1f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.pivot = new Vector2(1f, 1f);
        rectTransform.anchoredPosition = new Vector2(-24f, -24f);
        rectTransform.sizeDelta = new Vector2(160f, 48f);

        Image background = buttonObject.AddComponent<Image>();
        background.color = new Color(0f, 0f, 0f, 0.75f);

        Button button = buttonObject.AddComponent<Button>();

        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(buttonObject.transform, false);
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = "Salir";
        text.fontSize = 22f;
        text.alignment = TextAlignmentOptions.Center;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(HandleExitClicked);
    }

    private static void HandleExitClicked()
    {
        Challenge04SceneFlowController.CompleteAndReturn(ChallengeResult.Cancelled);
    }
}
