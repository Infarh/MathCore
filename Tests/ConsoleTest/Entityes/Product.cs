using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleTest.Entityes
{
    class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }

    class Cart
    {
        public int Id { get; set; }
        public User User { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

    class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Role> Roles { get; set; } = new List<Role>();
        public Cart Cart { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }

    class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }

    class Order
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public DateTime Time { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public User User { get; set; }
    }
}
