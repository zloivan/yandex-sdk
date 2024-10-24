mergeInto(LibraryManager.library, {

CheckLanguage : function (){
        gameInstance.SendMessage('YandexSdk', 'LanguageResponce', ysdk.environment.i18n.lang);
},

AddFocusListener : function (){
document.addEventListener("visibilitychange", function() {
    if (document.visibilityState === 'visible') {
        console.log('Tap has focus');
        gameInstance.SendMessage('YandexSdk', 'OnTabFocusedReact');
    } else {
        console.log('Tap lost focus');
        gameInstance.SendMessage('YandexSdk', 'OnTabUnfocusedReact');
    }
  })
},

ShowFullScreenAdv : function (){

  ysdk.adv.showFullscreenAdv({
    callbacks: {
        onOpen: function() {
          gameInstance.SendMessage('YandexSdk', 'InterstitialWindowOpenedEvent');
          console.log('Opened AD:');
        },
        onClose: function(wasShown) {
          console.log('Closed AD:');
          gameInstance.SendMessage('YandexSdk', 'InterstitialWindowClosedEvent');
        },
        onError: function(error) {
          gameInstance.SendMessage('YandexSdk', 'InterstitialShowFailedEvent', error);
          console.log('Error AD:', error);
        }
    }
  })
},

ShowRewardedAdv : function (){

  ysdk.adv.showRewardedVideo({
      callbacks: {
          onOpen: function() {
            gameInstance.SendMessage('YandexSdk', 'RewardWindowOpenedEvent');
            console.log('Opened Rewarded AD.');
          },
          onRewarded: function() {
            gameInstance.SendMessage('YandexSdk', 'RewardedAdEvent');
            console.log('Rewarded!');
          },
          onClose: function() {
            gameInstance.SendMessage('YandexSdk', 'RewardWindowClosedEvent');
            console.log('Video ad closed.');
          }, 
          onError: function(error) {
            gameInstance.SendMessage('YandexSdk', 'RewardedShowFailedEvent', error);
            console.log('Error while open video ad:', error);
          }
      }
  })
},

});