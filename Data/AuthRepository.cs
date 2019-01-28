using System.Threading.Tasks;
using DatingApp.API.Models;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using DatingApp.API.Data.Interfaces;

namespace DatingApp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _context;
        public AuthRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<User> Login(string username, string password)
        {
            var _user = await _context.Users.Include(p=> p.Photos).Where(c=> c.UserName.Equals(username)).FirstOrDefaultAsync();

            if(_user == null)
                return null;

            if(!VerifyHash(password, _user.PasswordHash, _user.PasswordSalt))
                return null;

            return _user;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash = null;
            byte[] passwordSalt = null;

            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<bool> UserExists(string username)
        {
            var _userExists = await _context.Users.Where(c=> c.UserName.Equals(username)).FirstOrDefaultAsync();

            if(_userExists == null)
                return false;

            return true;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
               passwordSalt = hmac.Key;
               passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            };  
        }

        private bool VerifyHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
               var computerHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

               for (int i = 0; i < computerHash.Length; i++)
               {
                   if(computerHash[i] !=passwordHash [i]) return false;
               } 

               return true;
            };
        }

    }
}