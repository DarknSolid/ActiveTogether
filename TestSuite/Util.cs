using EntityLib.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using ModelLib.DTOs.DogPark;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static EntityLib.Entities.Enums;

namespace TestSuite
{
    public class Util
    {
        public static DogParkCreateDTO GenerateDogPark()
        {
            Random random = new Random();
            var lat = random.Next(55, 57) + random.NextDouble();
            var lon = random.Next(11, 14) + random.NextDouble();
            return new DogParkCreateDTO
            {
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Curabitur sagittis eros porta lectus sagittis iaculis. Aliquam erat volutpat. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Vestibulum eget mattis velit. Etiam vulputate metus et mauris sodales, eget vulputate nulla laoreet. Nullam consequat convallis lorem a bibendum. Interdum et malesuada fames ac ante ipsum primis in faucibus.",
                Name = "Parken",
                Point = new NpgsqlPoint(x:lon, y:lat),
                Facilities = new List<DogParkFacilityType>()
            };
        }

        public static List<ApplicationUser> CreateUsers(int amount)
        {
            var users = new List<ApplicationUser>();
            for (int i = 0; i < amount; i++)
            {
                var email = $"TestUser{i + 1}@hotmail.com";
                users.Add(new ApplicationUser
                {
                    FirstName = "Test",
                    LastName = $"User {i + 1}",
                    Email = email,
                    EmailConfirmed = true,
                    UserName = email,
                    PasswordHash = $"asdf{i}"

                });
            }

            return users;
        }

        public static ApplicationUser CreateDeveloperUser(string email, string password, IPasswordHasher<ApplicationUser> passwordHasher)
        {
            var user = new ApplicationUser
            {
                FirstName = "developer",
                LastName = "user",
                UserName = email,
                NormalizedUserName = email.ToUpper(),
                NormalizedEmail = email.ToUpper(),
                Email = email,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            user.PasswordHash = passwordHasher.HashPassword(user, password);
            return user;

        }

        public static List<string> CreateDogBreeds()
        {
            return new List<string>()
            {
                "Crusty",
                "Labrador",
                "Golden Retriever",
                "Rotweiler",
            };
        }
    }
}
