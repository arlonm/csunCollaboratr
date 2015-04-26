using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;

namespace CollaboratR.WhiteboardHub
{
    [HubName("whiteboardHub")]
    public class WhiteboardHub : Hub
    {
        public void JoinGroup( string groupName)
        {

             Groups.Add(Context.ConnectionId, groupName);
            

        }

        public void SendDraw(string drawObject, string sessionId, string groupName,string name)
        {
            Clients.Group(groupName).HandleDraw(drawObject, sessionId, name);
        }


        
    }
}