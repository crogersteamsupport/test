$(document).ready(function () {
    var pusher = new Pusher('0cc6bf2df4f20b16ba4d', { authEndpoint: '../Services/ChatService.asmx/Auth' });
    var privateChannel = pusher.subscribe('presence-test');
    
    privateChannel.bind('pusher:subscription_succeeded', function () {alert('test')
        var me = privateChannel.members.me;
        var userId = me.id;
        var userInfo = me.info;
        alert(me)
    });

    privateChannel.bind('pusher:subscription_error', function (status) {
        alert(status)

    });
    


    $("#message").submit(function (e) {
        e.preventDefault();
        var message = $('#messageinput').val();
        var triggered = channel.trigger(eventName, message);

    });
  
});
