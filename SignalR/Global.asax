<%@ Import Namespace="System.Web.Routing" %>

<Script language="C#" runat="server">
     public void Application_OnStart() {
         // Application start-up code goes here.
         RouteTable.Routes.MapHubs();
     }
     public void Application_BeginRequest() {
         // Application code for each request could go here.
     }
     public void Application_OnEnd() {
         // Application clean-up code goes here.
     }
</script>

