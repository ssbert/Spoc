var nv = nv ? nv : {};
nv.evtBus = (function () {
    var evtListDic = {};
    var callbackDic = {};
    var handleArray = [];
    var onceDic = {};
    function getHandle() {
        var handle;
        do {
            handle = Math.floor(Math.random() * 1000000);
        } while (handleArray.indexOf(handle) > 0)

        return handle;
    }

    var publicFunc = {
        addEvt: function (evtName, callback, once) {
            if (typeof(evtName) !== "string") {
                return;
            }
            if (typeof (callback) !== "function") {
                return;
            }
            if (once != true) {
                once = false;
            }
            if (!evtListDic[evtName]) {
                evtListDic[evtName] = [];
            }
            var evtList = evtListDic[evtName];
            evtList.push(callback);
            var handle = getHandle();
            callbackDic[handle] = {
                evtName: evtName,
                callback: callback
            };
            handleArray.push(handle);
            if (once) {
                onceDic[callback] = handle;
            }
            return handle;
        },
        removeEvt: function (handle) {
            var handleIndex = handleArray.indexOf(handle);
            if ( handleIndex < 0) {
                return;
            }
            handleArray.splice(handleIndex, 1);
            var obj = callbackDic[handle];
            delete callbackDic[handle];
            var callbackArray = evtListDic[obj.evtName];
            var callbackIndex = callbackArray.indexOf(obj.callback);
            if (callbackArray < 0) {
                return;
            }
            callbackArray.splice(callbackIndex, 1);
        },
        dispatchEvt: function (evtName, data) {
            var callbackArray = evtListDic[evtName];
            if (!callbackArray) {
                return;
            }
            
            for (var i = 0; i < callbackArray.length; i++) {
                var callback = callbackArray[i];
                if (callback) {
                    callback(data);
                } else {
                    callbackArray.splice(i, 1);
                    i -= 1;
                }

                var handle = onceDic[callback];
                if (handle) {
                    publicFunc.removeEvt(handle);
                }
            }
        }
    };

    return publicFunc;
})();