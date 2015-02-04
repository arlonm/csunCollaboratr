using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CollaboratR.Models
{
    public class RoomModel : BusinessObject
    {
        public String RoomName { get; set; }
        public String RoomDescription { get; set; }
        public String RoomGuid { get; set; }
        public String Password { get; set; }
        public ConcurrentDictionary<String, RoomUserModel> CurrentUsers { get; set; }
        public ConcurrentDictionary<String, RoomUserModel> BannedUsers { get; set; }
        public List<ModuleModel> Modules { get; set; }
        public int MaxModuleCount { get; set; }
        //used for checking if room is public(true) or private(false)
        public bool RoomPublic { get; set; }

        //Javascript work around.
        //Without this property you have to add an additional property
        //loop for the dictionary to get the length
        public int UserCount { get { return CurrentUsers.Count; } }
    }
}