using UnityEngine;
using System.Reflection;
using System.Linq;
using Yandex;
using Yandex.Helpers;

public class YandexSDKDebugTool : MonoBehaviour
{
    private bool isDebugWindowOpen = false;
    private Vector2 scrollPosition;
    private GUIStyle buttonStyle;
    private GUIStyle windowStyle;
    private Rect debugButtonRect;
    private Rect windowRect;

    private ILogger _logger = new YandexSDKLogger();
    private void Awake()
    {
        // Ensure this object persists across scene loads
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        // Set up rectangles for UI elements
        debugButtonRect = new Rect(Screen.width - (300 + 10), Screen.height - (200 + 10), 300, 200);
        windowRect = new Rect((Screen.width - 300) / 2, (Screen.height - 100) / 2, 300, Screen.height - 100);
    }

    private void InitializeGUIStyles()
    {
        if (buttonStyle == null)
        {
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.normal.textColor = Color.white;
            buttonStyle.hover.textColor = Color.yellow;
        }

        if (windowStyle == null)
        {
            windowStyle = new GUIStyle(GUI.skin.window);
            windowStyle.normal.textColor = Color.white;
        }
    }

    private void OnGUI()
    {
        // Initialize styles before using them
        InitializeGUIStyles();

        // Draw debug button
        if (GUI.Button(debugButtonRect, "Yandex Debug", buttonStyle))
        {
            ToggleDebugWindow();
        }

        // Draw debug window if open
        if (isDebugWindowOpen)
        {
            windowRect = GUI.Window(0, windowRect, DrawDebugWindow, "Yandex SDK Debug", windowStyle);
        }
    }

    private void ToggleDebugWindow()
    {
        isDebugWindowOpen = !isDebugWindowOpen;
    }

    private void DrawDebugWindow(int windowID)
    {
        GUILayout.BeginVertical();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        if (Yandex.YandexSDK.Instance == null)
        {
            GUILayout.Label("YandexSDK instance not found.");
            return;
        }

        GUILayout.Label($"SDK Initialized: {Yandex.YandexSDK.Instance.IsInitialized}");

        var methods = typeof(Yandex.YandexSDK)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => !m.IsSpecialName).ToArray();

        foreach (var method in methods)
        {
            if (GUILayout.Button(method.Name, buttonStyle))
            {
                InvokeMethod(method);
            }
        }
        
        if (GUILayout.Button("Storage Save 404 to test", buttonStyle))
        {
            YandexSDK.Instance.Storage.SetInt("test",404);
            YandexSDK.Instance.Storage.Save();
        }
        if (GUILayout.Button("Load saved value", buttonStyle))
        {
            var value = YandexSDK.Instance.Storage.GetInt("test");
            Debug.Log(value);
        }
        

        GUILayout.EndScrollView();

        if (GUILayout.Button("Close", buttonStyle))
        {
            isDebugWindowOpen = false;
        }

        GUILayout.EndVertical();

        GUI.DragWindow();
    }

    private void InvokeMethod(MethodInfo method)
    {
        _logger.Log("YANDEX_SDK_DEBUG_TOOL", $"Invoking method: {method.Name}");
        if (Yandex.YandexSDK.Instance == null)
        {
            _logger.LogError("YANDEX_SDK_DEBUG_TOOL","YandexSDK instance is null. Cannot invoke method.");
            return;
        }

        if (method.GetParameters().Length == 0)
        {
            method.Invoke(Yandex.YandexSDK.Instance, null);
            _logger.Log("YANDEX_SDK_DEBUG_TOOL", $"Invoked method: {method.Name}");
        }
        else
        {
            _logger.Log("YANDEX_SDK_DEBUG_TOOL",$"Method {method.Name} requires parameters and cannot be invoked from debug window.");
        }
    }
}