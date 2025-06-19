using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Desktop.Repository
{
    public class UserRepository
    {
        private static readonly ApiClient apiClient = new ApiClient("http://45.144.64.179");

        public static async Task<bool> RegisterUserAsync(UserModel user)
        {
            try
            {
                await apiClient.PostAsync<UserModel>("/api/users", user);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static async Task<UserModel?> AuthorizeUserAsync(string mail, string password)
        {
            try
            {
                var users = await apiClient.GetAsync<List<UserModel>>("/api/users");
                return users.FirstOrDefault(use => use.Email == mail && use.Password == password);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<UserModel?> GetUserByNameAsync(string name)
        {
            try
            {
                var users = await apiClient.GetAsync<List<UserModel>>("/api/users");
                return users.FirstOrDefault(user => user.Name == name);
            }
            catch
            {
                return null;
            }
        }

        public static UserModel GetGuestUser()
        {
            return new UserModel
            {
                Id = Guid.NewGuid(),
                Name = "Guest",
                Email = "guest@example.com",
                Password = "guestpassword"
            };
        }
    }
}
