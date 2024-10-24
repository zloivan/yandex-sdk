// using System;
// using System.Runtime.InteropServices;
// using UnityEngine;
//
// namespace ColorPainting
// {
//     public class YandexSdk : MonoBehaviour
//     {
//         public static YandexSdk Instance;
//
//         public Action OnInterstitialWindowOpened = () => { };
//         public Action OnInterstitialWindowClosed = () => { };
//         public Action<string> OnInterstitialError = (error) => { };
//
//         public Action OnRewardWindowOpened = () => { };
//         public Action OnRewardWindowClosed = () => { };
//         public Action OnRewarded = () => { };
//         public Action<string> OnRewardError = (error) => { };
//
//         public Action<string> OnLanguageResponce = (language) => { };
//
//         public event Action OnTabFocused = () => { };
//         public event Action OnTabUnfocused = () => { };
//
//         private void Awake()
//         {
//             if (Instance == null)
//             { 
//                 Instance = this;
//             }
//             else
//             {
//                 Destroy(this);
//             }
//
//             DontDestroyOnLoad(this);
//         }
//
//         [DllImport("__Internal")]
//         private static extern void ShowFullScreenAdv();
//
//         public void ShowInterstitial()
//         {
// #if !UNITY_EDITOR
//
//              ShowFullScreenAdv();
// #endif
//         }
//
//         [DllImport("__Internal")]
//         private static extern void ShowRewardedAdv();
//         public void ShowRewardedVideo()
//         {
// #if !UNITY_EDITOR
//
//               ShowRewardedAdv();
// #endif
//         }
//
//         [DllImport("__Internal")]
//         private static extern void AddFocusListener();
//         public void AppFocusListener()
//         {
// #if !UNITY_EDITOR
//
//                AddFocusListener();
// #endif
//         }
//
//         [DllImport("__Internal")]
//         private static extern void CheckLanguage();
//         public void CheckWebLanguage()
//         {
// #if !UNITY_EDITOR
//
//                CheckLanguage();
// #endif
//         }
//
//         private void InterstitialWindowOpenedEvent()
//         {
//             OnInterstitialWindowOpened.Invoke();
//         }
//
//         private void InterstitialWindowClosedEvent()
//         {
//             OnInterstitialWindowClosed.Invoke();
//         }
//
//         private void InterstitialShowFailedEvent(string error)
//         {
//             OnInterstitialError.Invoke(error);
//         }
//
//         private void RewardWindowOpenedEvent()
//         {
//             OnRewardWindowOpened.Invoke();
//         }
//
//         private void RewardWindowClosedEvent()
//         {
//             OnRewardWindowClosed.Invoke();
//         }
//
//         private void RewardedAdEvent()
//         {
//             OnRewarded.Invoke();
//         // }
//
//         private void RewardedShowFailedEvent(string error)
//         {
//             OnRewardError.Invoke(error);
//         }
//
//         private void OnTabFocusedReact()
//         {
//             OnTabFocused.Invoke();
//         }
//
//         private void OnTabUnfocusedReact()
//         {
//             OnTabUnfocused.Invoke();
//         }
//
//         private void LanguageResponce(string language)
//         {
//             OnLanguageResponce.Invoke(language);
//         }
//     }
// }
