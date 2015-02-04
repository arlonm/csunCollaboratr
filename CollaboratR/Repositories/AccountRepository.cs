using CollaboratR.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

//Sprint 0: Account Repository initial implementation complete. Needs to be tested
namespace CollaboratR.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        //Instantiated connection string 
        private String connectionString;


        /// <param name="connectionStr">Connection string to the database storing your data</param>
        public AccountRepository(String connectionStr)
        {
            this.connectionString = connectionStr;
        }
        /// <summary>
        /// Get specific user account
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        public AccountModel GetAccount(int accountID)
        {
            SqlCommand command = new SqlCommand("AccountRead", new SqlConnection(connectionString));
            command.Parameters.AddWithValue("AccountId", accountID);
            DataSet dsResults  = DataTools.RunStoredProcedure(command);
            
            AccountModel acct = getAccountFromDataRow(dsResults.Tables[0].Rows[0]);

            return acct;
        }

        /// <summary>
        /// Gets all account from the database
        /// </summary>
        /// <returns>a list containing all of the accounts</returns>
        public IEnumerable<AccountModel> GetAllAccounts()
        {
            SqlCommand command = new SqlCommand("AccountReadAll", new SqlConnection(connectionString));
            DataSet dsResults = DataTools.RunStoredProcedure(command);

            List<AccountModel> accounts = new List<AccountModel>();
            foreach(DataRow row in dsResults.Tables[0].Rows)
            {
                accounts.Add(getAccountFromDataRow(row));
            }
            return accounts;
        }
        /// <summary>
        /// Update account info if there were changes
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public AccountModel UpdateAccount(AccountModel account)
        {
            //These are commands that are probably needed for this function.
            SqlCommand command = new SqlCommand("UpdateAccount", new SqlConnection(connectionString));

            command.Parameters.AddWithValue("@AccountId", account.AccountId);
            command.Parameters.AddWithValue("@Username", account.Username);
            command.Parameters.AddWithValue("@Email", account.Email);
            command.Parameters.AddWithValue("@Active", account.Active);
            command.Parameters.AddWithValue("@Locked", account.Locked);
            command.Parameters.AddWithValue("@Salt", account.Salt);
            command.Parameters.AddWithValue("@Password", account.Password);

            DataSet dsResult = DataTools.RunStoredProcedure(command);
            AccountModel acct = getAccountFromDataRow(dsResult.Tables[0].Rows[0]);

            return acct;
        }

        /// <summary>
        /// Get login information and validates against database
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        public AccountModel Login(LoginModel loginModel) //Successfully tested. runs fine
        {
            SqlCommand command = new SqlCommand("AccountLogin", new SqlConnection(this.connectionString));
            command.Parameters.AddWithValue("@LoginName", loginModel.LoginName);
            command.Parameters.AddWithValue("@Password", loginModel.Password);
            DataSet dsResult = DataTools.RunStoredProcedure(command);
            AccountModel acct = new AccountModel();
            try
            {
                acct = getAccountFromDataRow(dsResult.Tables[1].Rows[0]);
            }
            catch { }
            //check for message and same for success
            if (dsResult.Tables[0].Rows[0]["Message"] != null)
                acct.Message = Convert.ToString(dsResult.Tables[0].Rows[0]["Message"]);

            if (dsResult.Tables[0].Rows[0]["Success"] != null)
                acct.Success = Convert.ToBoolean(dsResult.Tables[0].Rows[0]["Success"]);

            return acct;
        }

        public AccountModel Register(RegistrationModel model)
        {
             SqlCommand command = new SqlCommand("AccountCreate", new SqlConnection(this.connectionString));
             command.Parameters.AddWithValue("@Username", model.Username);
             command.Parameters.AddWithValue("@Email", model.Email);
             command.Parameters.AddWithValue("@Password", model.Password);
             command.Parameters.AddWithValue("@Salt", model.Salt);
             command.Parameters.AddWithValue("@IpAddress", model.IpAddress ?? "");
             DataSet dsResult = DataTools.RunStoredProcedure(command);

             AccountModel acct = new AccountModel();
             if (dsResult.Tables[1] != null)
             {
                 if (dsResult.Tables[1].Rows.Count > 0)
                 {
                     acct = getAccountFromDataRow(dsResult.Tables[1].Rows[0]);
                 }
             }

             if (dsResult.Tables[0].Rows[0]["Message"] != null)
                 acct.Message = Convert.ToString(dsResult.Tables[0].Rows[0]["Message"]);

             if (dsResult.Tables[0].Rows[0]["Success"] != null)
                 acct.Success = Convert.ToBoolean(dsResult.Tables[0].Rows[0]["Success"]);

             return acct;
        }

       /// <summary>
       /// Get all account information per account by row
       /// </summary>
       /// <param name="row"></param>
       /// <returns>The account information</returns>
        private AccountModel getAccountFromDataRow(DataRow row)
        {
            AccountModel model = new AccountModel();
            //checking if account successfully logged in
            model.AccountId = Convert.ToInt32(row["Accountid"] ?? 0);

            if(row["DateCreated"] != null)
                model.DateCreated = DateTime.Parse(row["DateCreated"].ToString());

            model.Active = Convert.ToBoolean(row["Active"] ?? false);
            model.Email = Convert.ToString(row["Email"] ?? "");
            model.Locked = Convert.ToBoolean(row["Locked"] ?? false);
            model.Username = Convert.ToString(row["Username"] ?? "");

            return model;
        }
        public string GetSaltByLoginName(string loginName)
        {
            try
            {
                SqlCommand command = new SqlCommand("SaltReadByLoginName", new SqlConnection(this.connectionString));
                command.Parameters.AddWithValue("@LoginName", loginName);
                DataSet dsResults = DataTools.RunStoredProcedure(command);
                return dsResults.Tables[0].Rows[0]["Salt"].ToString();
            }
            catch
            {
                return String.Empty;
            }
        }
        /// <summary>
        /// Poll the database to see if a user's email exists.
        /// </summary>
        /// <param name="email">email to check</param>
        /// <returns></returns>
        public string GenerateEmailResetToken(string email)
        {
            SqlCommand cmd = new SqlCommand("GenerateEmailResetToken", new SqlConnection(this.connectionString));
            cmd.Parameters.AddWithValue("@email", email);
            DataSet dsResults = DataTools.RunStoredProcedure(cmd);
            return Convert.ToString(dsResults.Tables[0].Rows[0][0]);
        }
    }
}