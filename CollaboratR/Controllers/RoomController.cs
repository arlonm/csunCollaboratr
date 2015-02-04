using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CollaboratR.Models;
using CollaboratR.Hubs;
using CollaboratR.Repositories;
using System.Configuration;
using CollaboratR.ViewModels;

namespace CollaboratR.Controllers
{
    public class RoomController : Controller
    {
        //
        // GET: /Room/
        AccountRepository accountRepository;

        public RoomController()
        {
            accountRepository = new AccountRepository(ConfigurationManager.ConnectionStrings["defaultConnection"].ToString());
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            return View();
        }
        //roomGuid=b4e8dbfe-9938-4d79-97e4-bf91e84183a9&userGuid=a0e827d6-f317-432a-9f4b-52e4d1e006bf&adminKey=TTWlDvU4DzFRLGJjzR0orCzVsbiE4wV75%2FTrVd1XFEPi9IC5XAGvrU60n3O3HkKvICsPhftKa2owKScBTbpT%2BRgfgNmcblArJgRp0EORxMixOzWyR49ImIgWiTLZrWCUYKZbl51pE7Hb5F724JsmnmReTxGFobKI1uhQheUdUXg%3D
        [HttpPost]
        public ActionResult Join(String RoomGuid = "", String UserGuid = "", String AdminKey = "")
        {
            
            if(String.IsNullOrEmpty(RoomGuid) || String.IsNullOrEmpty(UserGuid) || String.IsNullOrEmpty(AdminKey))
            {
                return Join(RoomGuid, UserGuid);
            }
            return Join(RoomGuid, UserGuid);
            //TODO: Add admin key logic.
            //return View();
        }

        [HttpGet]
        public ActionResult Join(String RoomId = "", String Username = "")
        {
            if (Hubs.CollaborationHub.CollaborationRooms == null || !Hubs.CollaborationHub.CollaborationRooms.ContainsKey(RoomId))
            {
                return RedirectToAction("Index", "Error", new { msg = "Room doesn't exist"});
            }
            //TODO implement this logic.
            /*
                User polls to join a room.
                He is given a random GUID from the Server
                When server re-directs him his Guid is stored before he 
                gets a chance to maniuplate it before being added.
             *  This guid will uniquely id him for the purpose of 
             *  room permissions, this will entirely avoid the use of SQL
             
             */
            RoomViewModel roomViewModel = new RoomViewModel();
            roomViewModel.Room = Hubs.CollaborationHub.CollaborationRooms[RoomId];
            roomViewModel.JoiningUser = new RoomUserModel();
            roomViewModel.JoiningUser.UserGuid = Guid.NewGuid().ToString();
            //Identify user information 
            if (Request.IsAuthenticated)
            {
                //Grab an account model from them
                AccountModel acct = accountRepository.GetAccount(Convert.ToInt32(Utility.GetRequestUserid(User)));
                //Using that account model create a new Room USer model, which only has the core identification information

                //Set the necessary properties
                roomViewModel.JoiningUser.Username = acct.Username;
                //...
                //Add user to his specific room

                roomViewModel.Room.CurrentUsers.TryAdd(roomViewModel.JoiningUser.UserGuid, roomViewModel.JoiningUser);
                return View(roomViewModel);
            }
            else if(!String.IsNullOrEmpty(Username))
            {
                //Set the necessary properties
                roomViewModel.JoiningUser.Username = Username;
                roomViewModel.Room.CurrentUsers.TryAdd(roomViewModel.JoiningUser.UserGuid, roomViewModel.JoiningUser);
                return View(roomViewModel);
            }
            else
            {
                //Set the necessary properties
                roomViewModel.JoiningUser.Username = "Anonymous" + (new Random().Next() % 99999);
                roomViewModel.Room.CurrentUsers.TryAdd(roomViewModel.JoiningUser.UserGuid, roomViewModel.JoiningUser);
                return View(roomViewModel);
            }
        }

        public ActionResult Browse()
        {
            return View();
        }
	}

}