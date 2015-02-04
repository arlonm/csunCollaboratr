using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CollaboratR.Models
{

    public class AccountModel : BusinessObject
    {
        protected int accountId;
        protected String username;
        protected String email;
        protected DateTime dateCreated;
        protected Boolean active;
        protected Boolean locked;
        protected Boolean isContentAdmin;
        protected String salt;
        protected String password;

        public int AccountId
        {
            get { return accountId; }
            set { accountId = value; }
        }
        public String Username
        {
            get { return username; }
            set { username = value; }
        }
        public String Email
        {
            get { return email; }
            set { email = value; }
        }
        public DateTime DateCreated
        {
            get { return dateCreated; }
            set { dateCreated = value; }
        }
        public Boolean Active
        {
            get { return active; }
            set { active = value; }
        }
        public Boolean Locked
        {
            get { return locked; }
            set { locked = value; }
        }

        public String Salt
        {
            get { return salt; }
            set { salt = value; }
        }

        public String Password
        { 
            get { return password; }
            set { password = value; }
        }
    }
}