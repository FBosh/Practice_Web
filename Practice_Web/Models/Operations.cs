using Practice_Web.DAL;
using Practice_Web.Models.Entity;
using System;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Practice_Web.Models
{
    public class Operations
    {
        public Operations()
        {
            //Default constructor
        }

        internal void HandleRegister(UserContext context, User user)
        {
            user.Password = CryptographyLib.Encrypt.EncMD5(user.Password);
            //user.ConfirmPassword = MD5Lib.ClassMD5.EncMD5(user.ConfirmPassword);
            context.Users.Add(user);
            context.SaveChanges();
        }

        internal User GetUser(UserContext context, int id) => context.Users.Find(id);

        internal void HandleChangePassword(UserContext context, User user)
        {
            string originalPassword = GetUser(new UserContext(), user.ID).Password;
            string newPassword = CryptographyLib.Encrypt.EncMD5(user.Password);
            if (string.Equals(originalPassword, newPassword)) return;

            user.Password = newPassword;
            context.Entry(user).State = System.Data.Entity.EntityState.Modified;
            context.SaveChanges();
        }

        internal bool IsLoginSuccess(UserContext context, User user)
        {
            //HttpContext.Current.Session[""] = "";
            User resultUser = user != null ? GetUser(context, user.Account) : null;
            return resultUser != null && string.Equals(resultUser.Password, user.Password) && user.IsEnabled;
        }

        internal User GetUser(UserContext context, string account) => context.Users.SingleOrDefault(user => user.Account == account);

        internal bool IsAccountExist(UserContext context, string account) => GetUser(context, account) != null;

        internal void HandleLoggedSession(bool isLogged, UserContext context, User user)
        {
            var session = HttpContext.Current.Session;

            if (!isLogged || user == null) session.Remove(Constants.SESSION_LOGGED_USER);
            else session[Constants.SESSION_LOGGED_USER] = GetUser(context, user.Account);
        }
    }
}
