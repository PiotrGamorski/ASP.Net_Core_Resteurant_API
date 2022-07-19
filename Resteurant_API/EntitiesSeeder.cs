using Resteurant_API.DataContext;
using Resteurant_API.Entities;
using Resteurant_API.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Resteurant_API
{
    public class EntitiesSeeder : ISeeder
    {
        private readonly ResteurantDbContext _dbContext;

        public EntitiesSeeder(ResteurantDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                if (!_dbContext.Resteurants.Any())
                {
                    var resteurants = GetResteurants();
                    _dbContext.Resteurants.AddRange(resteurants);
                    _dbContext.SaveChanges();
                }

                if (!_dbContext.Roles.Any())
                {
                    var roles = GetRoles();
                    _dbContext.Roles.AddRange(roles);
                    _dbContext.SaveChanges();
                }
            }
        }

        private IEnumerable<Resteurant> GetResteurants()
        {
            return new List<Resteurant>()
            {
                new Resteurant()
                {
                    Name = "KFC",
                    Category = "Fast Food",
                    Description =
                        "KFC (short for Kentucky Fried Chicken) is an American fast food resteurant chain headquartered",
                    ContactEmail = "contact@kfc.com",
                    HasDelivery = true,
                    Dishes = new List<Dish>()
                    { 
                        new Dish()
                        { 
                            Name = "Nashville Hot Chicken",
                            Price = 10.30M,
                        },
                        new Dish()
                        { 
                            Name = "Chicken Nuggets",
                            Price = 5.30M,
                        }
                    },
                    Adress = new Address()
                    { 
                        City = "Kraków",
                        Street = "Długa 5",
                        PostalCode = "30-001"
                    }
                },
                new Resteurant()
                {
                    Name = "Republic of Burger",
                    Category = "Fast Food",
                    Description =
                        "Americam cuisine",
                    ContactEmail = "@rob.com",
                    HasDelivery = true,
                    Dishes = new List<Dish>()
                    {
                        new Dish()
                        {
                            Name = "Burger Double",
                            Price = 34.90M,
                        },
                        new Dish()
                        {
                            Name = "Burger Mad Max",
                            Price = 40.90M,
                        }
                    },
                    Adress = new Address()
                    {
                        City = "Pabianice",
                        Street = "Grobelna 8",
                        PostalCode = "95-200"
                    }
                },
            };
        }
        private IEnumerable<Role> GetRoles()
        {
            return new List<Role>()
            {
                new Role() { Name = "User"},
                new Role() { Name = "Manager"},
                new Role() { Name = "Admin"}
            };
        }
    }
}
