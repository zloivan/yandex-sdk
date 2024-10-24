using UnityEngine;
using Yandex.Helpers;

public class YandexGameLoadedSignal : MonoBehaviour
{
    private ILogger _logger = new YandexSDKLogger();
    private void Start()
    {
        _logger.Log("YANDEX_GAME_LOADED_SIGNAL", "Game loaded signal received");
        
        Yandex.YandexSDK.Instance.OnGameReady();
    }
}
