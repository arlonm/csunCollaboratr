using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CollaboratR.Models;

namespace CollaboratR.ViewModels
{
    public class RoomViewModel
    {
        public RoomModel Room { get; set; }
        public RoomUserModel JoiningUser { get; set; }
    }
}