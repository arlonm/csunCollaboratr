using CollaboratR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CollaboratR.Repositories
{
    /// <summary>
    /// Defines the interface between the controll and it's data source. Ideally we should be able to replace the backend with
    /// any data sourcing code and we'd be good to go so long as it implements this interface.
    /// </summary>
    public interface IAccountRepository
    {
        AccountModel GetAccount(int accountID);

        IEnumerable<AccountModel> GetAllAccounts();

        AccountModel UpdateAccount(AccountModel account);

        AccountModel Login(LoginModel loginModel);

        AccountModel Register(RegistrationModel model);

        String GetSaltByLoginName(String loginName);

        string GenerateEmailResetToken(String email);

    }
}