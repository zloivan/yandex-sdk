var player = null;

mergeInto(LibraryManager.library, {
    InitSDK: function() {
        console.log("InitSDK called from Unity");

        function initializePlayer() {
            window.ysdk.getPlayer()
                .then(function(_player) {
                    player = _player;
                    console.log("Player initialized");
                })
                .catch(function(err) {
                    console.error("Error initializing the Player object:", err);
                });
        }

        if (window.ysdk) {
            console.log("Yandex SDK already initialized");
            initializePlayer();
            gameInstance.SendMessage('YandexSDK', 'OnSDKInitialized');
        } else {
            console.log("Waiting for Yandex SDK to initialize");
            var initializationCheck = setInterval(function() {
                if (window.ysdk) {
                    console.log("Yandex SDK initialized");
                    clearInterval(initializationCheck);
                    initializePlayer();
                    gameInstance.SendMessage('YandexSDK', 'OnSDKInitialized');
                }
            }, 100);
        }
    },

    GameReady: function() {
        console.log("GameReady called from Unity");
        if (window.ysdk && window.ysdk.features && window.ysdk.features.LoadingAPI) {
            window.ysdk.features.LoadingAPI.ready();
        } else {
            console.error("Yandex SDK not initialized or LoadingAPI not available");
        }
    },

    GameplayStart: function() {
        console.log("GameplayStart called from Unity");
        if (window.ysdk && window.ysdk.features && window.ysdk.features.GameplayAPI) {
            window.ysdk.features.GameplayAPI.start();
        } else {
            console.error("Yandex SDK not initialized or GameplayAPI not available");
        }
    },

    GameplayStop: function() {
        console.log("GameplayStop called from Unity");
        if (window.ysdk && window.ysdk.features && window.ysdk.features.GameplayAPI) {
            window.ysdk.features.GameplayAPI.stop();
        } else {
            console.error("Yandex SDK not initialized or GameplayAPI not available");
        }
    },

    ShowInterstitial: function() {
        console.log("ShowInterstitial called from Unity");
        if (window.ysdk && window.ysdk.adv && window.ysdk.adv.showFullscreenAdv) {
            window.ysdk.adv.showFullscreenAdv({
                callbacks: {
                    onClose: function(wasShown) {
                        console.log("Interstitial closed, wasShown:", wasShown);
                        gameInstance.SendMessage('YandexSDK', 'OnInterstitialShown', wasShown ? "true" : "false");
                    },
                    onError: function(error) {
                        console.error("Interstitial error:", error);
                        gameInstance.SendMessage('YandexSDK', 'OnInterstitialFailed', error);
                    }
                }
            });
        } else {
            console.error("Yandex SDK not initialized or adv.showFullscreenAdv not available");
            gameInstance.SendMessage('YandexSDK', 'OnInterstitialFailed', "SDK not initialized");
        }
    },

    ShowRewarded: function() {
        console.log("ShowRewarded called from Unity");
        if (window.ysdk && window.ysdk.adv && window.ysdk.adv.showRewardedVideo) {
            window.ysdk.adv.showRewardedVideo({
                callbacks: {
                    onOpen: function() {
                        console.log("Rewarded video opened");
                        gameInstance.SendMessage('YandexSDK', 'OnRewardedAdOpened');
                    },
                    onRewarded: function() {
                        console.log("Rewarded video rewarded");
                        gameInstance.SendMessage('YandexSDK', 'OnRewardedAdRewarded');
                    },
                    onClose: function() {
                        console.log("Rewarded video closed");
                        gameInstance.SendMessage('YandexSDK', 'OnRewardedAdClosed');
                    },
                    onError: function(error) {
                        console.error("Rewarded video error:", error);
                        gameInstance.SendMessage('YandexSDK', 'OnRewardedAdError', error);
                    }
                }
            });
        } else {
            console.error("Yandex SDK not initialized or adv.showRewardedVideo not available");
            gameInstance.SendMessage('YandexSDK', 'OnRewardedAdError', "SDK not initialized");
        }
    },

    CheckCanReview: function() {
        console.log("CheckCanReview called from Unity");
        if (window.ysdk && window.ysdk.feedback && window.ysdk.feedback.canReview) {
            window.ysdk.feedback.canReview()
                .then(function(result) {
                    var canReview = result.value ? 'true' : 'false';
                    gameInstance.SendMessage('YandexSDK', 'OnCanReview', canReview);
                    if (!result.value) {
                        console.log("Can't request review:", result.reason);
                    }
                })
                .catch(function(error) {
                    console.error("Error in canReview:", error);
                });
        } else {
            console.error("Yandex SDK not initialized or feedback.canReview not available");
        }
    },

    RequestReview: function() {
        console.log("RequestReview called from Unity");
        if (window.ysdk && window.ysdk.feedback && window.ysdk.feedback.requestReview) {
            window.ysdk.feedback.requestReview()
                .then(function(result) {
                    var feedbackSent = result.feedbackSent ? 'true' : 'false';
                    gameInstance.SendMessage('YandexSDK', 'OnReviewDone', feedbackSent);
                })
                .catch(function(error) {
                    console.error("Error in requestReview:", error);
                });
        } else {
            console.error("Yandex SDK not initialized or feedback.requestReview not available");
        }
    },

    SetLeaderboardScore: function(nameLB, newScore) {
        console.log("SetLeaderboardScore called from Unity");

        if (!window.ysdk || !window.ysdk.getLeaderboards) {
            console.error("Yandex SDK not initialized or getLeaderboards not available");
            return;
        }

        if (!player) {
            console.error("Player is not initialized. Ensure InitSDK has been called and completed successfully.");
            return;
        }

        if (player.getMode() === 'lite') {
            console.log("Player not authorized");
            return;
        }

        var boardName = UTF8ToString(nameLB);

        window.ysdk.getLeaderboards()
            .then(function(lb) {
                return lb.getLeaderboardPlayerEntry(boardName);
            })
            .then(function(response) {
                if (!response || response.score < newScore) {
                    console.log("Saving new score");
                    return window.ysdk.getLeaderboards()
                        .then(function(lb) {
                            return lb.setLeaderboardScore(boardName, newScore);
                        });
                } else {
                    console.log("New score is not higher than existing score");
                }
            })
            .catch(function(err) {
                console.error("Error working with leaderboard:", err);
            });
    },

    GetLanguage: function() {
        console.log("GetLanguage called from Unity");
        if (window.ysdk && window.ysdk.environment && window.ysdk.environment.i18n && window.ysdk.environment.i18n.lang) {
            var lang = window.ysdk.environment.i18n.lang;
            gameInstance.SendMessage('YandexSDK', 'OnLanguageRequestResponse', lang);
        } else {
            console.error("Yandex SDK not initialized or language not available");
        }
    },

    GetLeaderboardEntries: function(leaderboardName) {
        console.log("GetLeaderboardEntries called from Unity");

        if (!window.ysdk || !window.ysdk.getLeaderboards) {
            console.error("Yandex SDK not initialized or getLeaderboards not available");
            return;
        }

        var boardName = UTF8ToString(leaderboardName);

        window.ysdk.getLeaderboards()
            .then(function(lb) {
                return lb.getLeaderboardEntries(boardName, { quantityTop: 10, includeUser: true });
            })
            .then(function(result) {
                var leaderboardData = JSON.stringify(result);
                gameInstance.SendMessage('YandexSDK', 'OnLeaderboardLoaded', leaderboardData);
            })
            .catch(function(error) {
                console.error("Error getting leaderboard entries:", error);
            });
    },

    IsSDKInitialized: function() {
        console.log("IsSDKInitialized called from Unity");
        var isInitialized = window.ysdk ? "1" : "0";
        gameInstance.SendMessage('YandexSDK', 'OnInitCheckResponse', isInitialized);
    },
     SaveDataInternal: function(data) {
            console.log("YandexStorage: SaveData called");
            try {
                if (!player) {
                    console.error("YandexStorage: Player not initialized");
                    return;
                }
    
                var jsonData = UTF8ToString(data);
                player.setData({
                    prefs: jsonData
                }).then(() => {
                    console.log("YandexStorage: Data saved successfully");
                    gameInstance.SendMessage('YandexSDK', 'OnDataSaved', 'true');
                }).catch(error => {
                    console.error("YandexStorage: Save failed:", error);
                    gameInstance.SendMessage('YandexSDK', 'OnDataSaved', 'false');
                });
            } catch (error) {
                console.error("YandexStorage: Error saving data:", error);
                gameInstance.SendMessage('YandexSDK', 'OnDataSaved', 'false');
            }
        },
    
        LoadDataInternal: function() {
            console.log("YandexStorage: LoadData called");
            try {
                if (!player) {
                    console.error("YandexStorage: Player not initialized");
                    return;
                }
    
                player.getData(['prefs'])
                    .then(data => {
                        if (data.prefs) {
                            console.log("YandexStorage: Data loaded successfully");
                            var bufferSize = lengthBytesUTF8(data.prefs) + 1;
                            var buffer = _malloc(bufferSize);
                            stringToUTF8(data.prefs, buffer, bufferSize);
                            gameInstance.SendMessage('YandexSDK', 'OnDataLoaded', buffer);
                            _free(buffer);
                        } else {
                            console.log("YandexStorage: No data found");
                            gameInstance.SendMessage('YandexSDK', 'OnDataLoaded', "{}");
                        }
                    })
                    .catch(error => {
                        console.error("YandexStorage: Load failed:", error);
                        gameInstance.SendMessage('YandexSDK', 'OnDataLoaded', "{}");
                    });
            } catch (error) {
                console.error("YandexStorage: Error loading data:", error);
                gameInstance.SendMessage('YandexSDK', 'OnDataLoaded', "{}");
            }
        },
    
        DeleteAllInternal: function() {
            console.log("YandexStorage: DeleteAll called");
            try {
                if (!player) {
                    console.error("YandexStorage: Player not initialized");
                    return;
                }
    
                player.setData({
                    prefs: "{}"
                }).then(() => {
                    console.log("YandexStorage: All data deleted successfully");
                    gameInstance.SendMessage('YandexSDK', 'OnDataDeleted', 'true');
                }).catch(error => {
                    console.error("YandexStorage: Delete all failed:", error);
                    gameInstance.SendMessage('YandexSDK', 'OnDataDeleted', 'false');
                });
            } catch (error) {
                console.error("YandexStorage: Error deleting data:", error);
                gameInstance.SendMessage('YandexSDK', 'OnDataDeleted', 'false');
            }
        }
});
