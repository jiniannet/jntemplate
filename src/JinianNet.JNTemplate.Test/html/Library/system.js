$(function(){
    $("#btnLogin").click(function(){
        var data = {
            "account"  : $("#Account").val(),
            "password" : $("#UserPassword").val()
        };
        
        $.ajax({
            url: "../Ajax/User/Login.aspx",
            type: "post",
            data: data,
            success: function (json) {
                if (json.Success) {
                    alert("登录成功！");
                    location.href='user/default.aspx';
                } else {
                    alert(json.Message);
                }
            }
        });
    });
    $("#btnSupportSearch").click(function(){
        var text = $("#SearchKey").val();
        if(!text){
            alert("请输入搜索词");
            return;
        }
        
        location.href='/Search.aspx?key='+escape(text)+"&product="+$("#SearchProduct").val();
    });
    
});