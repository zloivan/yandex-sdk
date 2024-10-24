mergeInto(LibraryManager.library, {

CheckCanSendFeedback : function (){

  ysdk.feedback.canReview()
        .then(function(response) {
            if (response.value) {
                gameInstance.SendMessage('ComingSoonPage', 'ShowFeedbackButton');
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
        gameInstance.SendMessage('SoundControl(Clone)', 'OnTabFocusedReact');
    } else {
        console.log('Tap lost focus');
        gameInstance.SendMessage('SoundControl(Clone)', 'OnTabUnfocusedReact');
    }
  })
},

ShowFullScreenAdv : function (){

  ysdk.adv.showFullscreenAdv({
    callbacks: {
        onOpen: function() {
          gameInstance.SendMessage('AdControl', 'InterstitialAdShowSucceededEvent');
          console.log('Opened AD:');
        },
        onClose: function(wasShown) {
          console.log('Closed AD:');
          gameInstance.SendMessage('AdControl', 'InterstitialAdClosedEvent');
        },
        onError: function(error) {
          gameInstance.SendMessage('AdControl', 'InterstitialAdShowFailedEvent', error);
          console.log('Error AD:', error);
        }
    }
  })
},

ShowRewardedAdv : function (){

  ysdk.adv.showRewardedVideo({
      callbacks: {
          onOpen: function() {
            gameInstance.SendMessage('AdControl', 'RewardedVideoAdOpenedEvent');
            console.log('Opened Rewarded AD.');
          },
          onRewarded: function() {
            gameInstance.SendMessage('AdControl', 'RewardedVideoAdRewardedEvent');
            console.log('Rewarded!');
          },
          onClose: function() {
            gameInstance.SendMessage('AdControl', 'RewardedVideoAdClosedEvent');
            console.log('Video ad closed.');
          }, 
          onError: function(e) {
            gameInstance.SendMessage('AdControl', 'RewardedVideoAdShowFailedEvent', e);
            console.log('Error while open video ad:', e);
          }
      }
  })
},

PurchaseItem : function (itemID){

  var idItem = UTF8ToString(itemID);

  if(payments != null)
  {
    payments.purchase({ id: idItem}).then(function(purchase){
          console.log('Purchased item', idItem);
          gameInstance.SendMessage('IAPControll', 'OnPurchaseCompletedHandler', idItem);
      }).catch(function(err){
          console.log('Purchased failed', idItem);
          gameInstance.SendMessage('IAPControll', 'OnPurchaseFailedHandler', idItem);
      })
  }
  else
  {
    console.log('Payments not ready');
  }
},

GetPurchases : function (){
    if(payments != null)
    {
      payments.getPurchases().then(function(purchases) {
        purchases.forEach(function(purchase) {
          console.log('Item restored', purchase.productID);
          gameInstance.SendMessage('IAPControll', 'OnPurchaseCompletedHandler', purchase.productID);
        })

          gameInstance.SendMessage('IAPControll', 'OnRestoreProcessComplete');
      })
      .catch(function(err){
          gameInstance.SendMessage('IAPControll', 'OnRestoreFailedHandler');
      })
    }
    else
    {
      console.log('Payments not ready');
    }
},

GetItemPrices : function (){

    if(payments != null)
    {
      payments.getCatalog().then(function(products) {products.forEach(function(product) {
              console.log('Finded product', product.id);
              gameInstance.SendMessage('IAPControll', 'ProductsPricesReciver', product.id+'~'+product.price);   
        })})
      .catch(function(err){
          console.log('Prices failed to return', err);
      })
    }
    else
    {
      console.log('Payments not ready');
    }
},

});