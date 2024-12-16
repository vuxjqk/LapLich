using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LapLich.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }

        public User(int userID, string userName, string password, string role)
        {
            UserID = userID;
            UserName = userName;
            Password = password;
            Role = role;
        }

        public override string ToString()
        {
            return $"UserID: {UserID}, UserName: {UserName}, Password: {Password}, Role: {Role}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is User))
                return false;

            User otherUser = (User)obj;
            return UserID == otherUser.UserID;
        }

        public override int GetHashCode()
        {
            return UserID.GetHashCode();
        }
    }
}