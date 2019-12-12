using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace Server
{
    public class ChatHub : Hub
    {
        public static List<Users> users = new List<Users>();
        public void Hello()
        {
            Clients.All.hello();
        }
        public void SendMessage(string message, string send_connectionID)
        //I have defined 2 parameters. These are the parameters to be sent here from the client software
        {
            var sender_connectionID = Context.ConnectionId;
            Users user = users.Where(x => x.connectionID.Equals(sender_connectionID)).FirstOrDefault();
            if (user != null)
            {
                Clients.Client(send_connectionID).SendMessage(message, user.username);
            }
        }
        public override Task OnConnected()
        {
            var connectionID = Context.ConnectionId;
            string userName = Context.QueryString["username"];
            if (string.IsNullOrEmpty(userName))
            {
                userName = Context.Headers["username"];
            }
            Users user = new Users()
            {
                username = userName,
                connectionID = connectionID
            };
            users.Add(user); //add the connection user to the list
            string json = JsonConvert.SerializeObject(users); //send to client
            Clients.All.getUserList(json);
            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            var connectionID = Context.ConnectionId;
            Users user = users.Where(x => x.connectionID.Equals(connectionID)).FirstOrDefault();
            if (user != null)
            {
                users.Remove(user); //in the case of connection termination we removed the user from the list
                string json = JsonConvert.SerializeObject(users); //send to client
                Clients.All.getUserList(json);
            }
            return base.OnDisconnected(stopCalled);
        }
        public class Users
        {
            public string username { get; set; }
            public string connectionID { get; set; }
        }
    }
}