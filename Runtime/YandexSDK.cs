using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UnityEngine;
using Yandex.Handlers;
using Yandex.Helpers;

namespace Yandex
{
    public static class YandexSDKInitializer
    {
        private static ILogger _logger = new YandexSDKLogger();
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoad()
        {
            // Check if YandexSDK GameObject exists
            if (GameObject.Find(YandexSDK.GameObjectName) == null)
            {
                // Create it if it doesn't exist
                var sdkObject = new GameObject(YandexSDK.GameObjectName);
                sdkObject.AddComponent<YandexSDK>();
                _logger.Log("YANDEX_SDK_INITIALIZED","YandexSDK GameObject was not found. Created a new one.");
            }
            else
            {
                _logger.Log("YANDEX_SDK_INITIALIZED","YandexSDK GameObject already exists.");
            }
        }
    }
    public class YandexSDK : MonoBehaviour
    {
        public static YandexSDK Instance { get; private set; }

        public const string GameObjectName = "YandexSDK";
        public string Language { get; private set; }
        public event Action<bool> InterstitialShown;
        public event Action<string> InterstitialFailed;
        public event Action RewardedAdOpened;
        public event Action RewardedAdRewarded;
        public event Action RewardedAdClosed;
        public event Action<string> RewardedAdError;
        public event Action SDKInitialized;
        public event Action<bool> CanReview;
        public event Action<bool> ReviewDone;
        public event Action<string> LeaderboardLoaded;
        public event Action<string> LanguageLoaded;

        public bool IsInitialized { get; private set; }
        
        private ILogger _logger = new YandexSDKLogger();

        [DllImport("__Internal")]
        private static extern void SetLeaderboardScore(string leaderboardName, int score);

        [DllImport("__Internal")]
        private static extern void GetLeaderboardEntries(string leaderboardName);

        [DllImport("__Internal")]
        private static extern void CheckCanReview();

        [DllImport("__Internal")]
        private static extern void RequestReview();

        [DllImport("__Internal")]
        private static extern void ShowInterstitial();

        [DllImport("__Internal")]
        private static extern void ShowRewarded();

        [DllImport("__Internal")]
        private static extern void GameReady();

        [DllImport("__Internal")]
        private static extern void GameplayStart();

        [DllImport("__Internal")]
        private static extern void GameplayStop();

        [DllImport("__Internal")]
        private static extern void GetLanguage();

        [DllImport("__Internal")]
        private static extern void IsSDKInitialized();

        [DllImport("__Internal")]
        private static extern void InitSDK();

        
        public YandexStorage Storage { get; private set; }
        private void Awake()
        {
            // Set the GameObject name
            gameObject.name = GameObjectName;
            
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                _logger.LogWarning("YANDEX_SDK", "Another instance of YandexSDK already exists. Destroying this one.");
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            Initialize();
        }

        private void Update()
        {
            Storage?.Update();
        }

        /// <summary>
        /// Init also called from HTML so this is just in case
        /// </summary>
        public void Initialize()
        {
            _logger.Log("YANDEX_SDK", "Initializing Yandex SDK");
#if !UNITY_EDITOR && UNITY_WEBGL
            InitSDK();
#endif

            Storage = new YandexStorage();
            Storage.Initialize();
        }

        public void UpdateLeaderboardScore(string leaderboardName, int score)
        {
            _logger.Log("YANDEX_SDK", $"Updating leaderboard score: {score}");
#if !UNITY_EDITOR && UNITY_WEBGL
            SetLeaderboardScore(leaderboardName, score);
#endif
        }

        public void RequestLeaderboardEntries(string leaderboardName)
        {
            _logger.Log("YANDEX_SDK", $"Requesting leaderboard entries for {leaderboardName}");
#if !UNITY_EDITOR && UNITY_WEBGL
            GetLeaderboardEntries(leaderboardName);
#endif
        }

        public void RequestSDKInitializationCheck()
        {
            _logger.Log("YANDEX_SDK", "Checking if SDK is initialized");
#if !UNITY_EDITOR && UNITY_WEBGL
            IsSDKInitialized();
#endif
        }

        public void RequestUserLanguage()
        {
            _logger.Log("YANDEX_SDK", "Requesting user language");
#if !UNITY_EDITOR && UNITY_WEBGL
            GetLanguage();
#endif
        }

        public void OnGameReady()
        {
            _logger.Log("YANDEX_SDK", "Game is ready");
#if !UNITY_EDITOR && UNITY_WEBGL
            GameReady();
#endif
        }

        public void OnGameplayStart()
        {
            _logger.Log("YANDEX_SDK", "Gameplay started");
#if !UNITY_EDITOR && UNITY_WEBGL
            GameplayStart();
#endif
        }

        public void OnGameplayStop()
        {
            _logger.Log("YANDEX_SDK", "Gameplay stopped");
#if !UNITY_EDITOR && UNITY_WEBGL
            GameplayStop();
#endif
        }

        public void ShowInterstitialAd()
        {
            _logger.Log("YANDEX_SDK", "Showing interstitial ad");
#if !UNITY_EDITOR && UNITY_WEBGL
            ShowInterstitial();
#endif
        }

        public void ShowRewardedAd()
        {
            _logger.Log("YANDEX_SDK", "Showing rewarded ad");
#if !UNITY_EDITOR && UNITY_WEBGL
            ShowRewarded();
#endif
        }

        public void CheckCanPlayerReview()
        {
            _logger.Log("YANDEX_SDK", "Checking if player can review");
#if !UNITY_EDITOR && UNITY_WEBGL
            CheckCanReview();
#endif
        }

        public void RequestPlayerReview()
        {
            _logger.Log("YANDEX_SDK", "Requesting player review");
#if !UNITY_EDITOR && UNITY_WEBGL
            RequestReview();
#endif
        }

        #region Implicit calls
        [UnityEngine.Scripting.Preserve]
        [UsedImplicitly]
        private void OnCanReview(string canReviewString)
        {
            _logger.Log("YANDEX_SDK_RESPONSE", $"Can review: {canReviewString}");
            
            var canReview = canReviewString.ToLower() == "true";
            CanReview?.Invoke(canReview);
        }
        
        [UnityEngine.Scripting.Preserve]
        [UsedImplicitly]
        private void OnReviewDone(string reviewDoneString)
        {
            _logger.Log("YANDEX_SDK_RESPONSE", $"Review done: {reviewDoneString}");
            var reviewDone = reviewDoneString.ToLower() == "true";
            ReviewDone?.Invoke(reviewDone);
        }
        
        [UnityEngine.Scripting.Preserve]
        [UsedImplicitly]
        private void OnInterstitialShown(string wasShownString)
        {
            _logger.Log("YANDEX_SDK_RESPONSE", $"Interstitial shown: {wasShownString}");
            var wasShown = wasShownString.ToLower() == "true";
            InterstitialShown?.Invoke(wasShown);
        }
        
        [UnityEngine.Scripting.Preserve]
        [UsedImplicitly]
        private void OnInterstitialFailed(string error)
        {
            _logger.Log("YANDEX_SDK_RESPONSE", $"Interstitial failed: {error}");
            InterstitialFailed?.Invoke(error);
        }
        
        [UnityEngine.Scripting.Preserve]
        [UsedImplicitly]
        private void OnRewardedAdOpened()
        {
            _logger.Log("YANDEX_SDK_RESPONSE", "Rewarded ad opened");
            RewardedAdOpened?.Invoke();
        }
        
        [UnityEngine.Scripting.Preserve]
        [UsedImplicitly]
        private void OnRewardedAdRewarded()
        {
            _logger.Log("YANDEX_SDK_RESPONSE", "Rewarded ad rewarded");
            RewardedAdRewarded?.Invoke();
        }
        
        [UnityEngine.Scripting.Preserve]
        [UsedImplicitly]
        private void OnRewardedAdClosed()
        {
            _logger.Log("YANDEX_SDK_RESPONSE", "Rewarded ad closed");
            RewardedAdClosed?.Invoke();
        }
        
        [UnityEngine.Scripting.Preserve]
        [UsedImplicitly]
        private void OnRewardedAdError(string error)
        {
            _logger.Log("YANDEX_SDK_RESPONSE", $"Rewarded ad error: {error}");
            RewardedAdError?.Invoke(error);
        }
        
        [UnityEngine.Scripting.Preserve]
        [UsedImplicitly]
        private void OnSDKInitialized()
        {
            _logger.Log("YANDEX_SDK_RESPONSE", "SDK initialized callback received from JavaScript");
            IsInitialized = true;
            SDKInitialized?.Invoke();
        }
        
        [UnityEngine.Scripting.Preserve]
        [UsedImplicitly]
        private void OnLeaderboardLoaded(string leaderboardData)
        {
            _logger.Log("YANDEX_SDK_RESPONSE", $"Leaderboard loaded: {leaderboardData}");
            LeaderboardLoaded?.Invoke(leaderboardData);
        }
        
        [UnityEngine.Scripting.Preserve]
        [UsedImplicitly]
        public void OnInitCheckResponse(string response)
        {
            _logger.Log("YANDEX_SDK_RESPONSE", $"SDK initialized: {response}");
            if (IsInitialized || response != "1") return;

            IsInitialized = true;
            SDKInitialized?.Invoke();
        }
        
        [UnityEngine.Scripting.Preserve]
        [UsedImplicitly]
        private void OnLanguageRequestResponse(string language)
        {
            _logger.Log("YANDEX_SDK_RESPONSE", $"Language loaded: {language}");
            LanguageLoaded?.Invoke(language);
        }

        #endregion
    }
}