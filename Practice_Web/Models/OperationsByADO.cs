using Practice_Web.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Practice_Web.Models
{
    public class OperationsByADO
    {
        private readonly UserOperation userOperation = new UserOperation();

        public OperationsByADO()
        {
            //Constructor
        }

        internal int HandleRegister(User user)
        {
            if (user == null || user.HasNullInProperties) return 0;

            user.Password = CryptographyLib.Encrypt.EncMD5(user.Password);
            return userOperation.Create(user);
        }

        internal int HandleChangePassword(User user)
        {
            if (user == null || !user.IsValid) return 0;

            var user_DB = GetUser(user.Account);
            if (user_DB == null) return 0;

            var originalPassword = user_DB.Password;
            var newPassword = CryptographyLib.Encrypt.EncMD5(user.Password);
            if (originalPassword == newPassword) return -1;

            user.Password = newPassword;
            return userOperation.Update(user);
        }

        internal bool IsLoginSuccess(User user)
        {
            var resultUser = user != null ? GetUser(user.Account) : null;
            return resultUser != null && string.Equals(CryptographyLib.Encrypt.EncMD5(user.Password), resultUser.Password) && resultUser.IsEnabled;
        }

        internal User GetUser(string account) => account != null ? userOperation.Get(account) : null;

        internal bool IsAccountExist(string account) => account != null && GetUser(account) != null;

        internal void HandleLoggedSession(bool isLogged, User user)
        {
            var session = HttpContext.Current.Session;

            if (!isLogged || user == null) session.Remove(Constants.SESSION_LOGGED_USER);
            else session[Constants.SESSION_LOGGED_USER] = GetUser(user.Account);
        }

        internal int DeleteUser(int id) => userOperation.Delete(id);
    }
}
