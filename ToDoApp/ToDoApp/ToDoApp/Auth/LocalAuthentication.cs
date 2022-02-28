using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Models;
using Xamarin.Essentials;

namespace ToDoApp.Auth
{
    public class LocalAuthentication : IFirebaseAuthentication
    {
        private static UserModel _currentUser = null;

        public Task<bool> ForgetPassword(string email)
        {
            throw new NotImplementedException();
        }

        public string GetUserId()
        {
            return GetUserKey(_currentUser?.Email);
        }

        public string GetUsername()
        {
            return _currentUser?.DisplayName;
        }

        public bool IsLoggedIn()
        {
            if (_currentUser == null)
            {
                _currentUser = GetUserModel("current");
            }

            return _currentUser != null;
        }

        public Task<UserModel> LoginWithEmailAndPassword(string email, string password)
        {
            var user = GetUserModel(email);
            if (!user.Token.Equals(password))
            {
                user = null;
            }
            else
            {
                _currentUser = user;
                this.SaveUserModel("current", user);
            }

            return Task.FromResult(user);
        }

        public bool LogOut()
        {
            _currentUser = null;

            return this.SaveUserModel("current", _currentUser);
        }

        public Task<bool> RegisterWithEmailAndPassword(string username, string email, string password)
        {
            var result = true;
            var user = GetUserModel(email);
            if (user == null)
            {
                user = new UserModel()
                {
                    DisplayName = username,
                    Email = email,
                    Token = password,
                };
            }
            else if (user.Token.Equals(password))
            {
                user.DisplayName = username;
            }
            else
            {
                result = false;
            }

            if (result)
            {
                result = this.SaveUserModel(email, user);
            }

            return Task.FromResult(result);
        }

        private bool SaveUserModel(string userId, UserModel userModel)
        {
            var result = false;
            if (!string.IsNullOrEmpty(userId))
            {
                var userKey = GetUserKey(userId);
                Preferences.Set(userKey, JsonConvert.SerializeObject(userModel));
                result = true;
            }

            return result;
        }

        private UserModel GetUserModel(string userId)
        {
            var userKey = GetUserKey(userId);
            var userJson = Preferences.Get(userKey, string.Empty);
            return string.IsNullOrEmpty(userJson) ? null : JsonConvert.DeserializeObject<UserModel>(userJson);
        }

        private string GetUserKey(string userId)
        {
            return "user-" + userId;
        }
    }
}
