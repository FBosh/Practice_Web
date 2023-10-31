using Practice_Web.Models.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Practice_Web.Models
{
    public class UserOperation : IDataOperation<User>
    {
        private readonly string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["UserContext"].ConnectionString;
        private readonly string[] propertyNames = Enum.GetNames(typeof(User.UserProperty));

        private const string str_CREATE = "Create";
        private const string str_GETALL = "GetAll";
        private const string str_UPDATE = "Update";
        private const string str_DELETE = "Delete";

        public int Create(User user)
        {
            var commandString = GetCommandString(str_CREATE, propertyNames);
            if (string.IsNullOrEmpty(connectionString) || user == null || user.HasNullInProperties || string.IsNullOrEmpty(commandString))
            {
                return 0;
            }

            var propertyValues = user.GetProperties();

            using (var connection = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(commandString, connection))
                {
                    for (var i = 0; i < propertyValues.Length; i++)
                    {
                        if (Equals(propertyNames[i], "ID")) continue;

                        cmd.Parameters.Add(new SqlParameter($"@{propertyNames[i]}", propertyValues[i]));
                    }

                    connection.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<User> GetAll()
        {
            var commandString = GetCommandString(str_GETALL, propertyNames);
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(commandString))
            {
                yield return null;
            }

            using (var connection = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(commandString, connection))
                {
                    connection.Open();
                    var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleResult);
                    if (reader == null) yield return null;

                    while (reader.Read()) yield return new User(GetProperties(reader));
                }
            }
        }

        public User Get(int id) => id > 0 ? HandleGet(id) : null;

        public User Get(string account) => account != null ? HandleGet(account) : null;

        public int Update(User user)
        {
            var commandString = GetCommandString(str_UPDATE, propertyNames);
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(commandString) || user == null)
            {
                return 0;
            }

            var propertyValues = user.GetProperties();

            using (var connection = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(commandString, connection))
                {
                    for (int i = 0; i < propertyNames.Length; i++)
                    {
                        cmd.Parameters.Add(new SqlParameter($"@{propertyNames[i]}", propertyValues[i]));
                    }

                    connection.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public int Delete(User user)
        {
            var commandString = GetCommandString(str_DELETE, propertyNames);
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(commandString) || user == null || !user.IsValid)
            {
                return 0;
            }

            using (var connection = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(commandString, connection))
                {
                    cmd.Parameters.Add(new SqlParameter("@ID", user.ID));

                    connection.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public int Delete(int id)
        {
            var commandString = GetCommandString(str_DELETE, propertyNames);
            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(commandString) || id <= 0)
            {
                return 0;
            }

            using (var connection = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(commandString, connection))
                {
                    cmd.Parameters.Add(new SqlParameter("@ID", id));

                    connection.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        private object[] GetProperties(SqlDataReader reader)
        {
            if (reader == null) return new User().GetProperties();

            var properties = new object[propertyNames.Length];
            for (int i = 0; i < properties.Length; i++) properties[i] = reader.GetValue(reader.GetOrdinal(propertyNames[i]));

            return properties;
        }

        private string GetCommandString(string whichCRUD, object[] propertyNames)
        {
            var strResult = "";

            if (propertyNames == null) return null;

            switch (whichCRUD)
            {
                case str_CREATE:
                    var hasMarkAt = false;
                    strResult = "INSERT INTO Users (";

                    for (int i = 0; i < propertyNames.Length; i++)
                    {
                        if (Equals(propertyNames[i], "ID")) continue;

                        strResult +=
                            (hasMarkAt ? "@" : "")
                            + propertyNames[i]
                            + (i != propertyNames.Length - 1 ? ", " : ")");

                        if (i == propertyNames.Length - 1 && !hasMarkAt)
                        {
                            hasMarkAt = true;
                            i = 0;
                            strResult += " VALUES (";
                        }
                    }

                    break;

                case str_UPDATE:
                    strResult = "UPDATE Users SET ";

                    for (int i = 0; i < propertyNames.Length; i++)
                    {
                        if (Equals(propertyNames[i], "ID")) continue;

                        strResult +=
                            $"{propertyNames[i]} = @{propertyNames[i]}"
                            + (i != propertyNames.Length - 1 ? ", " : $" WHERE {propertyNames[0]} = @{propertyNames[0]}");
                    }

                    break;

                case str_GETALL:
                    strResult = "SELECT * FROM Users";
                    break;

                case str_DELETE:
                    strResult = "DELETE FROM Users WHERE ID = @ID";
                    break;
            }

            return strResult;
        }

        private User HandleGet(object p)
        {
            if (string.IsNullOrEmpty(connectionString) || (!(p is int) && !(p is string))) return null;

            var commandString = "SELECT * FROM Users WHERE " + (p is int ? "ID = @ID" : "Account = @Account");

            using (var connection = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(commandString, connection))
                {
                    cmd.Parameters.Add(new SqlParameter(p is int ? "@ID" : "@Account", p));

                    connection.Open();
                    var reader = cmd.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleResult);
                    return reader != null && reader.Read() ? new User(GetProperties(reader)) : null;
                }
            }
        }
    }
}
