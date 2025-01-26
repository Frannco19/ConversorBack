using Data.Entities;
using Data.Models.DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IUserService
    {
        User CreateUser(UserRegistrationDTO registrationDto);

        User GetById(int userId);
        User ValidateUser(string username, string password);

        User IncrementConversionsUsed(int userId);

        int GetUserIdFromToken(string token);
        User UpdateUser(User user);

        bool CanConvert(int userId);
    }
}
