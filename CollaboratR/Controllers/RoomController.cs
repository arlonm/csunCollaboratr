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
        public ActionResult Browse()
        {
            return View();
        }

        //this recieves the form data from create.cshtml
        [HttpPost]
        public ActionResult Join(String RoomGuid = "", String UserGuid = "", String AdminKey = "")
        {

            if (String.IsNullOrEmpty(RoomGuid) || String.IsNullOrEmpty(UserGuid) || String.IsNullOrEmpty(AdminKey))
            {
                return RedirectToAction("Index", "Error", new { msg = "Error at Join Post - a var is nullorempty" });
                //TODO: Add admin key logic.
            }
            else return Join(RoomGuid, UserGuid);

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
            roomViewModel.JoiningUser.UserGuid = Guid.NewGuid().ToString();//this creates a new guid every time it loads for even the same user
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

	}

}