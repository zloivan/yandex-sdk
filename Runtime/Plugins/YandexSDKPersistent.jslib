mergeInto(LibraryManager.library, {
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