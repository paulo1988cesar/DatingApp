using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Data.Interfaces;
using DatingApp.API.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DatingApp.API.Helpers;
using System;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;
        }

        public void Add<T>(T enity) where T : class
        {
            _context.Add(enity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u=> u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

            return photo;
        }

        public async Task<User> GetUser(int id)
        {
           var users = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);

           return users;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users = _context.Users.Include(p => p.Photos).OrderByDescending(p=> p.LastActive).AsQueryable();

            users = users.Where(c => c.Id != userParams.UserID);
            
            users = users.Where(c => c.Gender == userParams.Gender);

            if(userParams.MinAge != 18  || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge -1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(m => m.DateOfBirth >= minDob && m.DateOfBirth <= maxDob);
            }                     

            if(!String.IsNullOrEmpty(userParams.OrderBy))
            {
                    switch (userParams.OrderBy)
                    {
                        case  "created":
                            users = users.OrderByDescending(u => u.Created);
                            break;
                        default:
                            users = users.OrderByDescending(u => u.LastActive);
                            break;
                    }
            }

           return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> QtdPhotosUser(int id) => await _context.Photos.CountAsync(p => p.UserId == id);
    }
}