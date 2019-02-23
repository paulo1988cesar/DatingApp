using System;
using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        public Seed(UserManager<User> userMaganer, RoleManager<Role> roleManager)
        {
            _userManager = userMaganer;
            _roleManager = roleManager;
        }    

        public void SeedUsers()
        {
            var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);

            var roles = new List<Role>
            {
                new Role { Name = "Member"},
                new Role { Name = "Admin"},
                new Role { Name = "Moderator"},
                new Role { Name = "VIP"}
            };

            if(_roleManager.Roles == null)
            {
                foreach (var item in roles)
                {
                    _roleManager.CreateAsync(item).Wait();
                }
            }

            AddAdminUser();
            
            if(!_userManager.Users.Any())
            {
                foreach (var user in users)
                {
                    _userManager.CreateAsync(user, "password").Wait();
                    _userManager.AddToRoleAsync(user, "Member").Wait();
                }
            }
            else
            {
                var lstUsuarios = _userManager.Users.Select(c => c.UserName);
                var lstUsers = users.Select(c => c.UserName);

                var usersToCreate = users.Where(p => !lstUsuarios.Any(p2 => p2 == p.UserName)).ToList();

                if(usersToCreate.Count > 0)
                {    
                    foreach (var user in users)
                    {
                        _userManager.CreateAsync(user, "password").Wait();
                        _userManager.AddToRoleAsync(user, "Member").Wait();
                    }
                }                                          
            }

            AddRoleUser();
        }

        private void AddAdminUser()
        {
            var userExistente = _userManager.FindByNameAsync("Admin").Result;

            if (userExistente == null)
            {
                var adminUser = new User
                {
                    UserName = "Admin",
                    KnownAs = "Usuário administrador"                        
                };

                IdentityResult result = _userManager.CreateAsync(adminUser, "password").Result;

                if(result.Succeeded)
                {
                    var admin = _userManager.FindByNameAsync("Admin").Result;
                    _userManager.AddToRolesAsync(admin, new []{"Member", "Moderator"}).Wait();
                }
            }
        }
    
        private void AddRoleUser()
        {
           List<User> users = _userManager.Users.Where(c => !c.UserName.Equals("Admin")).ToList();

           foreach (var user in users)
           {
               if(!_userManager.IsInRoleAsync(user, "Member").Result)
               {
                   _userManager.AddToRoleAsync(user, "Member").Wait();
               }
           }
                           
        }
    }
}