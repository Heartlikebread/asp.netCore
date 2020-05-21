"use strict";
//建立singalR連線
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " ： " + msg;
    //建立一個li標籤
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    

       //抓取輸入標籤
    var showUser = document.getElementById("userInput").value;
    //將li特效的點點做隱藏
    li.style.listStyle = "none";
    //加入間距上20px
    li.style.marginTop = "20px"; 
    //判斷若是自己
    if (showUser == user) {        
        ////若是自己則顯示在右邊 
        li.style.textAlign = "right";
    }
    else {
        //若是其他人則顯示為左邊
        li.style.textAlign = "left";
    }
    //抓取介面ul顯示範圍加入子元件li
    document.getElementById("messagesList").appendChild(li);
});

connection.start().then(function () {
    //若有資料進來禁止同時輸入
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    if (user.length == 0 || message.length == 0) {
        alert("請填入使用者與訊息");
        return;
    }
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});