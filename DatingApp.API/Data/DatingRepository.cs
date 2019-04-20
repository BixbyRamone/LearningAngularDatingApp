using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        // public async Task<Photo> GetMainPhotoForUser(int userId)
        // {
        //     return await _context.Photos.Where(u => u.UserId== userId)
        //         .FirstOrDefaultAsync(p => p.IsMain);
        // }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users
                .Include(l => l.Likees)
                .Include(l => l.Likers)
                .Include(p => p.Photos)
                .FirstOrDefaultAsync(u => u.Id == id);

                return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users =  _context.Users                
                .Include(p => p.Photos)
                .OrderByDescending(u => u.LastActive).AsQueryable();

            users = users.Where(u => u.Id != userParams.UserId);

            users = users.Where(u => u.Gender == userParams.Gender);

            // var likes = _context.Likes.AsQueryable();

            

            // if (userParams.Likers)
            // {  
            //     likes = likes.Where(like => like.LikeeId == userParams.UserId);
            //     // Select liker column
            //     users = likes.Select(like => like.Liker);                      
            // }

            // if (userParams.Likees)
            // {
            //     likes = likes.Where(like => like.LikerId == userParams.UserId);
            //     // Select liker column
            //     users = likes.Select(like => like.Likee);                  
            // }

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
            }

            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;

                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        // private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
        // {
        //     var user = await _context.Users
        //         .Include(x => x.Likers)
        //         .Include(x => x.Likees)
        //         .FirstOrDefaultAsync(u => u.Id == id);

        //     if (likers)
        //     {
        //         return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
        //     }
        //     else
        //     {
        //         return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
        //     }
        // }

        // private async Task<IEnumerable<Like>> GetUserLikees(int id) {
        //     var user = await GetUser(id);
        //     var userLikees = user.Likees.Where(u => u.LikerId == id).ToList();
        //     return userLikees;
        // }
 
        // private async Task<IEnumerable<Like>> GetUserLikers(int id) {
        //    var user = await GetUser(id); 
        //    var userLikers = user.Likees.Where(u => u.LikeeId == id).ToList();
        //    return userLikers;
        // }

        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}