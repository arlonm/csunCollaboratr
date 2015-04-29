using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Web;
using CollaboratR.Models;

namespace CollaboratR.Hubs
{
    public class CollaborationHub : Hub
    {
        public static ConcurrentDictionary<string, RoomModel> CollaborationRooms = new ConcurrentDictionary<string, RoomModel>();

        //A public chat detail collection
        //So make a model


        public CollaborationHub()
        {

            //Create a couple of demo rooms if nothing exists in context collection
            if (CollaborationRooms.Count == 0)
            {
                /*ModuleModel testMod1 = new ModuleModel();
                testMod1.ModuleId = 1;
                testMod1.ModuleTypeId = Constants.MODULE_TYPE_CHAT;
                testMod1.ModuleWidth = 200;
                testMod1.ModuleHeight = 200;
                testMod1.ModuleX = 100;
                testMod1.ModuleY = 100;
                testMod1.ModuleContent = ""+
                "{"+
                    "   \"log\":[\"This is some test chat\", \"And this should come second\", \"Look at all of this chat\"]" +    
                "}";*/

                ModuleModel testMod2 = new ModuleModel();
                testMod2.ModuleId = 2;
                testMod2.ModuleTypeId = Constants.MODULE_TYPE_CHAT;
                testMod2.ModuleWidth = 300;
                testMod2.ModuleHeight = 300;
                testMod2.ModuleX = 100;
                testMod2.ModuleY = 100;
                testMod2.ModuleContent = "" +
"{" +
"   \"log\":[\"Welcome to the chat room\"]" +
"}";
                RoomModel room1 = new RoomModel();
                room1.BannedUsers = new ConcurrentDictionary<String, RoomUserModel>();
                room1.CurrentUsers = new ConcurrentDictionary<String, RoomUserModel>();
                room1.RoomName = "General Chat";
                room1.RoomGuid = Guid.NewGuid().ToString();
                room1.RoomDescription = "Welcome to CollaboratR, from SignalR!";

                room1.Modules = new List<ModuleModel>();
                room1.Modules.Add(testMod2);

                RoomModel room2 = new RoomModel();
                room2.BannedUsers = new ConcurrentDictionary<String, RoomUserModel>();
                room2.CurrentUsers = new ConcurrentDictionary<String, RoomUserModel>();
                room2.RoomName = "Private chat";
                room2.RoomGuid = Guid.NewGuid().ToString();
                room2.RoomDescription = "This is private chat";
                room2.Password = "MyPassword";

                RoomModel room3 = new RoomModel();
                room3.BannedUsers = new ConcurrentDictionary<String, RoomUserModel>();
                room3.CurrentUsers = new ConcurrentDictionary<String, RoomUserModel>();
                room3.RoomName = "Current News";
                room3.RoomGuid = Guid.NewGuid().ToString();
                room3.RoomDescription = "Come discuss current news and events!";
                

                if (!CollaborationRooms.ContainsKey(room1.RoomGuid))
                {
                    CollaborationRooms.TryAdd(room1.RoomGuid, room1);
                }
                if (!CollaborationRooms.ContainsKey(room2.RoomGuid))
                {
                    CollaborationRooms.TryAdd(room2.RoomGuid, room2);
                }
                if (!CollaborationRooms.ContainsKey(room3.RoomGuid))
                {
                    CollaborationRooms.TryAdd(room3.RoomGuid, room3);
                }

            }
            ///////////////////////////////////////////////////
            ///////////////////////////////////////////////////
        }
        /// <summary>
        /// Creates room based on current users input on room creation
        /// </summary>
        /// <param name="roomName"></param>
        /// <param name="desc"></param>
        /// <param name="passW"></param>
        public void CreateRoom(String roomName, String desc, String passW)
        {    
            RoomModel room = new RoomModel();
            //create user guid
            RoomUserModel user = new RoomUserModel();
            user.UserGuid = Guid.NewGuid().ToString();
            //create random byte for admin key
            Random rnd = new Random();
            byte[] b = new byte[128];
            //convert byte to string for easier handling in javascript
            String convKey = Convert.ToBase64String(b);
            rnd.NextBytes(b);
            //where are the banned and current users being used?
            room.BannedUsers = new ConcurrentDictionary<string, RoomUserModel>();
            room.CurrentUsers = new ConcurrentDictionary<string, RoomUserModel>();
            room.RoomName = roomName;
            room.RoomGuid = Guid.NewGuid().ToString();
            //need to create loop that checks if created roomguid already exists.
            while (CollaborationRooms.ContainsKey(room.RoomGuid))
            {
                room.RoomGuid = Guid.NewGuid().ToString();
            }            
            //check room description
            if (!(String.IsNullOrEmpty(desc)))
            {
                room.RoomDescription = desc;
            }
            else
            {
                room.RoomDescription = "";
            }
            //check permissions to see if room is private or public
            if (String.IsNullOrEmpty(passW))
            {
                room.RoomPublic = true;
            }
            else
            {
                room.Password = passW;
                room.RoomPublic = false;
            }
            //do we need another check before adding the room?
            CollaborationRooms.TryAdd(room.RoomGuid, room);
            //broadcast room guid, user guid, admin key
           
            Clients.Caller.broadcastCreateRoomResult(room.RoomGuid, user.UserGuid, convKey);
        }
        /// <summary>
        /// Allows current user to join room
        /// </summary>
        /// <param name="roomGuid"></param>
        /// <param name="user"></param>
        public void JoinRoom(String roomGuid, RoomUserModel user)
        {
            if(CollaborationRooms.ContainsKey(roomGuid))
            {
                RoomModel room = CollaborationRooms[roomGuid];
                //Check if users exist or if they are banned
                if (!room.BannedUsers.ContainsKey(user.UserGuid))
                {
                    room.CurrentUsers.TryAdd(user.UserGuid, user);
                    Groups.Add(Context.ConnectionId, roomGuid);
                    Clients.Group(roomGuid, Context.ConnectionId).addUserToList(user);
                }

            }
        }
        public Task LeaveRoom(string roomId)
        {
            return Groups.Remove(Context.ConnectionId, roomId.ToString());
        }

        public void AddRoom(string userOwner)
        {
            string guid = Guid.NewGuid().ToString().Replace("-","");
            if (CollaborationRooms[guid] == null)
            {
                RoomModel room = new RoomModel();
            }
        }

        public void ListRooms()
        {
            var topRooms =
                from room in CollaborationRooms
                orderby room.Value.CurrentUsers.Count
                select room;
            
            Clients.Caller.broadcastRooms(topRooms);
        }

        //copied this alone from johns updated code - want to test
        public void ListTopRooms()
        {
            var topTenRooms =
                (from room in CollaborationRooms
                 orderby room.Value.CurrentUsers.Count descending
                 select room).Take(10);

            Clients.Caller.broadcastRooms(topTenRooms);
        }

        public void UpdateModule(String roomGuid, String userGuid, ModuleModel model)
        {
            if(CollaborationRooms.ContainsKey(roomGuid)){
                for(int i = 0; i < CollaborationRooms[roomGuid].Modules.Count; i++)
                {
                    if (model.ModuleId == CollaborationRooms[roomGuid].Modules[i].ModuleId)
                    {
                        CollaborationRooms[roomGuid].Modules[i] = model;
                        Clients.Group(roomGuid,Context.ConnectionId).updateModule(model);
                        //Clients.All.updateModule(model);
                        break;
                    }
                }
            }
        }
        public void UpdateModuleContent(String roomGuid, String userGuid, int moduleId, int moduleTypeId, String content)
        {
            //TODO(Tyler): check for module permissions as well
            if (String.IsNullOrEmpty(content)) { return; }
            if (!CollaborationRooms.ContainsKey(roomGuid)) { return; }
            RoomModel room = CollaborationRooms[roomGuid];
            try
            {
                var module =
                    (from mod in room.Modules
                     where mod.ModuleId == moduleId
                     select mod).ElementAt(0);
                module.ModuleContent = content;
                //Clients.Group(roomGuid).updateModuleContent(module);
            }
            catch
            {
                return;
            }
        }

        //this is called thru js on join.cshtml load
        public  void LoadRoomUserList(string roomGuid)
        {
            //if our context collection has this room
            if (CollaborationRooms.ContainsKey(roomGuid))
            {
                RoomModel model = CollaborationRooms[roomGuid];
                Clients.Caller.broadcastUserlist(model.CurrentUsers.Values.ToArray());
            }
        }
        public  void LoadRoomInformation(string roomGuid)
        {
            if (CollaborationRooms.ContainsKey(roomGuid))
            {
                RoomModel model = CollaborationRooms[roomGuid];
                Clients.Caller.displayRoom(model);
            }
        }

        public void sendChat(string userName, string message, string callerId, string roomIdentifier)
        {
            //using all except
            /*List<string> excludelist = new List<string>();
            excludelist.Add(callerId);
            Clients.AllExcept(excludelist.ToArray()).recieveMessage(userName, message);*/

            //using groups
            List<string> excludelist = new List<string>();
            excludelist.Add(callerId);
            Clients.Group(roomIdentifier, excludelist.ToArray()).recieveMessage(userName, message);

            //also maybe save messages-to-groupname, maybe for a history mechanism
        }
        
        public void SendDraw(string drawObject, string sessionId, string groupName, string name)
        {
            Clients.Group(groupName).HandleDraw(drawObject, sessionId, name);
        }         

    }
}