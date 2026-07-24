using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// -------------------------------------------------------------
// Construye automáticamente las barras de vida (jugador y enemigo) al
// entrar a la escena de combate, sin necesidad de montar el Canvas a mano.
//
// Reutiliza BarraVidaUI: solo crea el Canvas + las Images y las configura.
// Se dispara solo cuando se carga BattleArena, así que también funciona al
// repetir una pelea.
// -------------------------------------------------------------
public static class BattleHUDBootstrap
{
    private const string BattleSceneName = "BattleArena";
    private const string HudObjectName = "BattleHUD (auto)";

    private static readonly Color ColorFondo = new Color(0f, 0f, 0f, 0.55f);
    private static readonly Color ColorVidaJugador = new Color(0.20f, 0.80f, 0.25f, 1f);
    private static readonly Color ColorVidaEnemigo = new Color(0.85f, 0.20f, 0.20f, 1f);

    private static Sprite spriteBlanco;

    // Sprite blanco 1x1: necesario para que un Image de tipo Filled se dibuje.
    private static Sprite SpriteBlanco
    {
        get
        {
            if (spriteBlanco == null)
            {
                Texture2D tex = Texture2D.whiteTexture;
                spriteBlanco = Sprite.Create(
                    tex,
                    new Rect(0f, 0f, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f)
                );
                spriteBlanco.name = "BarraVida_Blanco";
            }

            return spriteBlanco;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Register()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;

        // Cubre el caso en que BattleArena ya sea la escena activa al iniciar.
        if (SceneManager.GetActiveScene().name == BattleSceneName)
        {
            BuildHudIfNeeded();
        }
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == BattleSceneName)
        {
            BuildHudIfNeeded();
        }
    }

    private static void BuildHudIfNeeded()
    {
        // Si ya existe (o hay barras montadas a mano), no duplicar.
        if (GameObject.Find(HudObjectName) != null)
        {
            return;
        }

        if (Object.FindAnyObjectByType<BarraVidaUI>() != null)
        {
            return;
        }

        GameObject canvasObject = new GameObject(HudObjectName);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        // Barra del jugador: arriba a la izquierda.
        CrearBarra(
            canvasObject.transform,
            "BarraVida_Jugador",
            BarraVidaUI.Objetivo.Jugador,
            ColorVidaJugador,
            new Vector2(0f, 1f),
            new Vector2(30f, -30f),
            ocultarSinObjetivo: false
        );

        // Barra del enemigo: arriba a la derecha. Se oculta si no hay enemigo.
        CrearBarra(
            canvasObject.transform,
            "BarraVida_Enemigo",
            BarraVidaUI.Objetivo.Enemigo,
            ColorVidaEnemigo,
            new Vector2(1f, 1f),
            new Vector2(-470f, -30f),
            ocultarSinObjetivo: true
        );
    }

    private static void CrearBarra(
        Transform parent,
        string nombre,
        BarraVidaUI.Objetivo objetivo,
        Color colorVida,
        Vector2 anclaEsquina,
        Vector2 posicion,
        bool ocultarSinObjetivo)
    {
        const float ancho = 440f;
        const float alto = 34f;

        // Contenedor (fondo).
        GameObject fondoObject = new GameObject(nombre);
        fondoObject.transform.SetParent(parent, false);

        RectTransform fondoRect = fondoObject.AddComponent<RectTransform>();
        fondoRect.anchorMin = anclaEsquina;
        fondoRect.anchorMax = anclaEsquina;
        fondoRect.pivot = new Vector2(0f, 1f);
        fondoRect.sizeDelta = new Vector2(ancho, alto);
        fondoRect.anchoredPosition = posicion;

        Image fondoImg = fondoObject.AddComponent<Image>();
        fondoImg.color = ColorFondo;
        fondoImg.sprite = SpriteBlanco;

        // Relleno (la vida).
        GameObject rellenoObject = new GameObject("Relleno");
        rellenoObject.transform.SetParent(fondoObject.transform, false);

        RectTransform rellenoRect = rellenoObject.AddComponent<RectTransform>();
        rellenoRect.anchorMin = Vector2.zero;
        rellenoRect.anchorMax = Vector2.one;
        rellenoRect.pivot = new Vector2(0f, 0.5f);
        // Pequeño margen para que se vea el fondo como borde.
        rellenoRect.offsetMin = new Vector2(4f, 4f);
        rellenoRect.offsetMax = new Vector2(-4f, -4f);

        Image rellenoImg = rellenoObject.AddComponent<Image>();
        rellenoImg.color = colorVida;
        rellenoImg.type = Image.Type.Filled;
        rellenoImg.fillMethod = Image.FillMethod.Horizontal;
        rellenoImg.fillOrigin = (int)Image.OriginHorizontal.Left;
        rellenoImg.fillAmount = 1f;
        // Sprite blanco para que el Image de tipo Filled se dibuje.
        rellenoImg.sprite = SpriteBlanco;

        BarraVidaUI barra = rellenoObject.AddComponent<BarraVidaUI>();
        barra.Configure(objetivo, rellenoImg, ocultarSinObjetivo);
    }
}
