mergeInto(LibraryManager.library, {

  SetLeaderboardScores: function (nameLB, newScore)
  {
    if(player!= null)
    {
        if (player.getMode() === 'lite') 
        {
                console.log('Player NOT Autorized');
        }
        else
        {
            console.log('Player Autorized');

            var boardName = UTF8ToString(nameLB);

            if(lb != null)
            {
                lb.getLeaderboardPlayerEntry(boardName).then(function(responce)
                {
                    if(responce.score < newScore)
                    {
                        console.log('Save New Score');

                        lb.setLeaderboardScore(boardName, newScore);
                    }
                })
            }
        }
    }
  },

CheckCanSendFeedback : function (){

  ysdk.feedback.canReview()
        .then(function(response) {
            if (response.value) {
                gameInstance.SendMessage('SettingPopup(Clone)', 'ShowFeedbackButton');
            } else {
                console.log(response.reason)
            }
  })
},

SendFeedback : function (){

    ysdk.feedback.requestReview();
},

AddFocusListener : function (){
document.addEventListener("visibilitychange", function() {
    if (document.visibilityState === 'visible') {
        console.log('Tap has focus');
        gameInstance.SendMessage('SoundManager', 'OnTabFocusedReact');
    } else {
        console.log('Tap lost focus');
        gameInstance.SendMessage('SoundManager', 'OnTabUnfocusedReact');
    }
  })
},

ShowFullScreenAdv : function (){

  ysdk.adv.showFullscreenAdv({
    callbacks: {
        onOpen: function() {
          gameInstance.SendMessage('AdManager', 'InterstitialAdOpenedEvent');
          console.log('Opened AD:');
        },
        onClose: function(wasShown) {
          console.log('Closed AD:');
          gameInstance.SendMessage('AdManager', 'InterstitialAdClosedEvent');
        },
        onError: function(error) {
          gameInstance.SendMessage('AdManager', 'InterstitialAdShowFailedEvent', error);
          console.log('Error AD:', error);
        }
    }
  })
},

ShowRewardedAdv : function (){

  ysdk.adv.showRewardedVideo({
      callbacks: {
          onOpen: function() {
            gameInstance.SendMessage('AdManager', 'RewardedVideoAdOpenedEvent');
            console.log('Opened Rewarded AD.');
          },
          onRewarded: function() {
            gameInstance.SendMessage('AdManager', 'RewardedVideoAdRewardedEvent');
            console.log('Rewarded!');
          },
          onClose: function() {
            gameInstance.SendMessage('AdManager', 'RewardedVideoAdClosedEvent');
            console.log('Video ad closed.');
          }, 
          onError: function(e) {
            gameInstance.SendMessage('AdManager', 'RewardedVideoAdShowFailedEvent', e);
            console.log('Error while open video ad:', e);
          }
      }
  })
},

});