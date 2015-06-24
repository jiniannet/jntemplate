(function(){
    var quest = null;
    function init(){
        quest = {};
        var aTmp;
        var queryString = new String(window.location.search);
        queryString = queryString.substr(1, queryString.length); 
        var aPairs = queryString.split("&");
        for (var i = 0; i < aPairs.length; i++) {
            aTmp = aPairs[i].split("=");
            quest[aTmp[0].toLowerCase()] = aTmp[1];
        }
    }
    
    window.getRequest = function (key) {
        if (!quest) {
            init();
        }
        key = key.toLowerCase();
        return quest[key] || "";
    };
})(window);

function autoSetData(dataJson,list) {
    var attr= null;
    for(var i=0;i<list.length;i++){
        if (list[i]) {
            attr = list[i].getAttribute('bind-field');
            if(!attr){
                continue;
            }
            switch(list[i].tagName.toLowerCase()){
                case "input":
                case "textarea":
                case "select":
                    $(list[i]).val(dataJson[attr]);
                    break;
                case "img":
                    list[i].src = dataJson[attr];
                    break;
                default:
                    $(list[i]).html(dataJson[attr]);
                    break;
            }
        }
    }
}


function getPostData(dom) {
    if (!dom) {
        dom = document.body;
    }
    var dataJson = new Object();
    var inputs = dom.getElementsByTagName("input");
    var values = {};
    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].name) {
            switch (inputs[i].type) {
                case "radio":
                    if(inputs[i].value && inputs[i].value!=='on'){
                        if (inputs[i].checked) {
                            dataJson[inputs[i].name] = inputs[i].value;
                        }
                    }else{
                        dataJson[inputs[i].name] = inputs[i].checked ? true : false;
                    }
                    break;
                case "checkbox":
                    if (inputs[i].checked) {
                        if(inputs[i].value && inputs[i].value!=='on'){
                            if (!dataJson[inputs[i].name]) {
                                dataJson[inputs[i].name] = inputs[i].value;
                            } else {
                                dataJson[inputs[i].name] += "," + inputs[i].value;
                            }
                        }else{
                            dataJson[inputs[i].name] = true;
                        }
                    }
                    break;
                default:
                    dataJson[inputs[i].name] = inputs[i].value;
                    break;
            }
        }
    }
    var selects = dom.getElementsByTagName("select");
    for (var i = 0; i < selects.length; i++) {
        if (selects[i].name) {
            dataJson[selects[i].name] = selects[i].value;
        }
    }

    var textareas = dom.getElementsByTagName("textarea");
    for (var i = 0; i < textareas.length; i++) {
        if (textareas[i].name) {
            dataJson[textareas[i].name] = textareas[i].value;
        }
    }
    return dataJson;
}
