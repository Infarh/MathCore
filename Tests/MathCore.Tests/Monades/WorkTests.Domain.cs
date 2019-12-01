using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using MathCore.Monades;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NN = MathCore.Annotations.NotNullAttribute;
using CN = MathCore.Annotations.CanBeNullAttribute;


namespace MathCore.Tests.Monades
{
    public partial class WorkTests
    {
        private sealed class Logger : IEnumerable<Logger.Item>
        {
            public class Item
            {
                public readonly DateTime Time;
                public readonly string Message;
                public readonly Exception Error;
                public bool Success => Error is null;
                public Item(string Message) => (Time, this.Message) = (DateTime.Now, Message);
                public Item(string Message, Exception Error) => (Time, this.Message, this.Error) = (DateTime.Now, Message, Error);

                public static implicit operator Item(string Message) => new Item(Message);
            }
            private readonly List<Item> _Items = new List<Item>();

            public void Add(string Message) => _Items.Add(Message);
            public void Add(string Message, Exception Error) => _Items.Add(new Item(Message, Error));

            public IEnumerator<Item> GetEnumerator() { return _Items.GetEnumerator(); }

            IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable) _Items).GetEnumerator(); }
        }

        private sealed class User
        {
            public int Id { get; set; }

            [NN] public string UserName { get; set; }

            [NN] public string Password { get; set; }

            public int LoginCount { get; set; }

            public bool LoggedIn { get; set; }
        }

        [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
        private sealed class UserManager
        {
            [NN] private readonly List<User> _Users;

            public UserManager([NN] IEnumerable<User> Users) => _Users = Users as List<User> ?? Users.ToList();

            public void Create([NN] User user, [NN] string password)
            {
                if (user is null) throw new ArgumentNullException(nameof(user));
                if (password is null) throw new ArgumentNullException(nameof(password));

                if (string.IsNullOrWhiteSpace(user.UserName))
                    throw new ArgumentException("Отсутствует имя пользователя", nameof(user));

                if (_Users.Contains(user) || _Users.Any(u => u.UserName == user.UserName))
                    throw new InvalidOperationException($"Ошибка создания пользователя: пользователь {user.UserName} уже существует");

                user.Id = _Users.Count == 0 ? 1 : _Users.Max(u => u.Id) + 1;
                user.Password = $"{password}user:{user.UserName}".GetHashCode().ToString();
                _Users.Add(user);
            }

            [CN] public User GetUserByName([NN] string UserName) => _Users.FirstOrDefault(u => u.UserName == UserName);
            [CN] public User GetUserById(int Id) => _Users.FirstOrDefault(u => u.Id == Id);

            public bool CheckPassword([NN] User user, [NN] string password)
            {
                if (user is null) throw new ArgumentNullException(nameof(user));
                if (password is null) throw new ArgumentNullException(nameof(password));

                var db_user = GetUserById(user.Id);
                if (db_user is null) throw new InvalidOperationException($"Пользователь с id:{user.Id} не найден");
                return user.Password == $"{password}user:{user.UserName}".GetHashCode().ToString();
            }

            public bool Login([NN] string UserName, [NN] string Password)
            {
                if (UserName is null) throw new ArgumentNullException(nameof(UserName));
                if (Password is null) throw new ArgumentNullException(nameof(Password));

                var user = GetUserByName(UserName);
                if(user is null || user.LoggedIn || !CheckPassword(user, Password)) return false;
                user.LoggedIn = true;
                user.LoginCount++;
                return true;
            }

            public void Logout([NN] string UserName)
            {
                if (UserName is null) throw new ArgumentNullException(nameof(UserName));

                var user = GetUserByName(UserName);
                if(user is null) return;
                user.LoggedIn = false;
            }
        }

        [TestMethod]
        public void Work_of_AccountController_RegisterUser()
        {
            const string test_user_name = "TestUser";
            const string test_user_password = "password";

            var users = new List<User>
            {
                new User
                {
                    UserName = "Admin",
                    Password = ("Password" + "user:Admin").GetHashCode().ToString()
                },
            };

            #region AccountController

            var user_manager = new UserManager(users);
            var logger = new Logger();

            void Register(string UserName, string Password) => Work
                .With(user_manager)
                .Do(m => m.Create(new User { UserName = UserName }, Password))
                .IfSuccess(() => logger.Add($"{UserName} registered"))
                .IfFailure(e => logger.Add($"{UserName} not registered with reason {e.Message}", e))
                .Execute();

            void Login(string UserName, string Password) => Work
               .With(user_manager)
               .Do(m => m.Login(test_user_name, test_user_password))
               .IfSuccess(() => logger.Add($"{UserName} login"))
               .IfFailure(e => logger.Add($"{UserName} error login with reason {e.Message}", e))
               .Execute();

            #endregion
           
            Register(test_user_name, test_user_password);
            Assert.IsTrue(users.Contains(user => user.UserName == test_user_name));

            var test_user = user_manager.GetUserByName(test_user_name);
            Assert.That.Value(test_user).IsNotNull();

            Login(test_user_name, test_user_password);
            Assert.IsTrue(test_user.LoggedIn);
        }
    }
}