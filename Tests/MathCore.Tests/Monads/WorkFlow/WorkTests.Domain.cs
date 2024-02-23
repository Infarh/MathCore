#nullable enable
using System.Collections;
using System.Diagnostics.CodeAnalysis;

using MathCore.Monads.WorkFlow;

using NN = MathCore.Annotations.NotNullAttribute;
using CN = MathCore.Annotations.CanBeNullAttribute;
// ReSharper disable MemberCanBePrivate.Local

namespace MathCore.Tests.Monads.WorkFlow;

public partial class WorkTests
{
    #region Domain

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

            public static implicit operator Item(string Message) => new(Message);
        }
        private readonly List<Item> _Items = new();

        public void Add(string Message) => _Items.Add(Message);
        public void Add(string Message, Exception Error) => _Items.Add(new(Message, Error));

        public IEnumerator<Item> GetEnumerator() => _Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_Items).GetEnumerator();
    }

    private sealed class User
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public int LoginCount { get; set; }

        public bool LoggedIn { get; set; }
    }

    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalse")]
    private sealed class UserManager
    {
        private readonly List<User> _Users;

        public UserManager(IEnumerable<User> Users) => _Users = Users as List<User> ?? Users.ToList();

        public void Create(User user, string password)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));
            if (password is null) throw new ArgumentNullException(nameof(password));

            if (string.IsNullOrWhiteSpace(user.UserName))
                throw new ArgumentException("Отсутствует имя пользователя", nameof(user));

            if (_Users.Contains(user) || _Users.Any(u => u.UserName == user.UserName))
                throw new InvalidOperationException($"Ошибка создания пользователя: пользователь {user.UserName} уже существует");

            user.Id       = _Users.Count == 0 ? 1 : _Users.Max(u => u.Id) + 1;
            user.Password = $"{password}user:{user.UserName}".GetHashCode().ToString();
            _Users.Add(user);
        }

        public User? GetUserByName(string UserName) => _Users.FirstOrDefault(u => u.UserName == UserName);
        public User? GetUserById(int Id) => _Users.FirstOrDefault(u => u.Id == Id);

        public bool CheckPassword(User user, string password)
        {
            if (user is null) throw new ArgumentNullException(nameof(user));
            if (password is null) throw new ArgumentNullException(nameof(password));

            var db_user = GetUserById(user.Id);
            if (db_user is null) throw new InvalidOperationException($"Пользователь с id:{user.Id} не найден");
            return user.Password == $"{password}user:{user.UserName}".GetHashCode().ToString();
        }

        public bool Login(string UserName, string Password)
        {
            if (UserName is null) throw new ArgumentNullException(nameof(UserName));
            if (Password is null) throw new ArgumentNullException(nameof(Password));

            var user = GetUserByName(UserName);
            if (user is null || user.LoggedIn || !CheckPassword(user, Password)) return false;
            user.LoggedIn = true;
            user.LoginCount++;
            return true;
        }

        public void Logout(string UserName)
        {
            if (UserName is null) throw new ArgumentNullException(nameof(UserName));

            var user = GetUserByName(UserName);
            if (user is null) return;
            user.LoggedIn = false;
        }
    }

    private class AccountController
    {
        private readonly UserManager _UserManager;
        private readonly Logger _Logger;

        public AccountController(UserManager UserManager, Logger Logger)
        {
            _UserManager = UserManager ?? throw new ArgumentNullException(nameof(UserManager));
            _Logger      = Logger ?? throw new ArgumentNullException(nameof(Logger));
        }

        public Result Register(string UserName, string Password) =>
            Work.With(_UserManager)
               .Invoke(m => m.Create(new() { UserName = UserName }, Password))
               .InvokeIfSuccess(() => _Logger.Add($"{UserName} registered"))
               .InvokeIfSuccess(() => _Logger.Add($"{UserName} registered"))
               .InvokeIfFailure(e => _Logger.Add($"{UserName} not registered with reason {e.Message}", e))
               .GetIfSuccess<Result>(() => new SuccessResult($"User {UserName} registered"))
               .GetIfFailure(e => new FailureResult($"Registration operation faulted with message {e.Message}"));

        public Result Login(string UserName, string Password) => Work
           .With(_UserManager)
           .Invoke(m => m.Login(UserName, Password))
           .InvokeIfSuccess(() => _Logger.Add($"{UserName} login"))
           .InvokeIfFailure(e => _Logger.Add($"{UserName} error login with reason {e.Message}", e))
           .GetIfSuccess<Result>(() => new SuccessResult("Login operation complete"))
           .GetIfFailure(e => new FailureResult($"Registration operation faulted with message {e.Message}"));
    }

    private abstract class Result
    {
        public string Message { get; }
        public Result(string Message) => this.Message = Message;
    }

    private sealed class SuccessResult : Result { public SuccessResult(string Message) : base(Message) { } }
    private sealed class FailureResult : Result { public FailureResult(string Message) : base(Message) { } }

    #endregion

    [TestMethod]
    public void Work_of_AccountController_RegisterUser()
    {
        const string test_user_name     = "TestUser";
        const string test_user_password = "password";

        var users = new List<User>
        {
            new()
            {
                UserName = "Admin",
                Password = ("Password" + "user:Admin").GetHashCode().ToString()
            },
        };

        var user_manager = new UserManager(users);
        var logger       = new Logger();

        var controller = new AccountController(user_manager, logger);

        var registration_result = controller.Register(test_user_name, test_user_password);

        CollectionAssert.That.Collection(users).Contains(user => user.UserName == test_user_name);
        Assert.That.Value(registration_result).IsNotNull();
        Assert.That.Value(registration_result).Is<SuccessResult>();

        var test_user    = user_manager.GetUserByName(test_user_name);
        var login_result = controller.Login(test_user_name, test_user_password);
        Assert.That.Value(login_result).Is<SuccessResult>();
        Assert.That.Value(test_user)
           .AsNotNull()
           .Where(user => user.LoggedIn).IsTrue();
    }
}