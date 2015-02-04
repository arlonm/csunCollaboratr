using CollaboratR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CollaboratR.ViewModels
{
    public class AccountViewModel : BusinessObject
    {
        protected int accountId;
        protected String username;
        protected String email;
        protected DateTime dateCreated;
        protected Boolean active;
        protected Boolean locked;
        protected Boolean isContentAdmin;

        public AccountViewModel(AccountModel m)
        {
            this.accountId = m.AccountId;
            this.username = m.Username;
            this.email = m.Email;
            this.active = m.Active;
            this.locked = m.Locked;
            this.success = m.Success;
            this.message = m.Message;
        }

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

    }
}