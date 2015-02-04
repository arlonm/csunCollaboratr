
    /*
        Chat JSON Object Description
        {
            "log": ["oldest chat", "newer chat", "newest chat"]
        }
    */
    //ModuleTypeId = 1
    var createChatModule = function (module, data)
    {
        var chatObj = JSON.parse(data.ModuleContent);
        module.append($('<div class="chat-space"></div>'));
        for (var i = 0 ; i < chatObj.log.length; i++) {
            module.children('.chat-space').append('<a class="list-group-item">' + chatObj.log[i] + '</a>');
        }
        module.append($('<div class="chat-input-wrapper"><input type="text" class="chat-input"/> <button class="btn btn-primary btn-chat-send">Send</div>'));
        module.children('.btn-chat-send').click(function () {
            
        });
    }
    var updateChatModule = function (module, data) {
        var chatObj = JSON.parse(data.ModuleContent);
        module.children('.chat-space').append('<a class="list-group-item">' + chatObj.log[chatObj.length-1] + '</a>');
    };

    /*
    WhiteBoard JSON Object Description
        {
            "lines":[
                {
                    "x1":5,
                    "y1":10,
                    "x2":20,
                    "y2":30,
                    "color":"#RRGGBB"
                    "thickness":"5px"
                },
                {
                    "x1":5,
                    "y1":10,
                    "x2":20,
                    "y2":30,
                    "color":"#RRGGBB"
                    "thickness":"5px"
                }
            ]
        }
    */
    var renderWhiteBoardModule = function (module, data) {

    }
