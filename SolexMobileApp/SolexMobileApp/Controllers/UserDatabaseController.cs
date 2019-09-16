using SQLite;
using Xamarin.Forms;
using SolexMobile.Data;
using SolexMobileApp.Models;

namespace SolexMobileApp.Controllers
{
    public class UserDatabaseController
    {
        static readonly object locker = new object();

        SQLiteConnection database;

        public UserDatabaseController()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            database.CreateTable<User>();
        }

        public User GetLastUserActive()
        {
            lock (locker)
            {
                if(database.Table<User>().Count() == 0)
                {
                    return null;
                }
                else
                {
                    return database.Table<User>().OrderByDescending(x => x.Updated_at).First();
                }
            }
        }

        public User GetUserByUsername(string username)
        {
            lock (locker)
            {
                return database.Table<User>().FirstOrDefault(x => x.Username == username);
            }
        }

        public bool CheckCredentialsUserLocal(string username, string password)
        {
            lock (locker)
            {
                User user = new User();
                user = database.Table<User>().FirstOrDefault(x => x.Username == username && x.Password == password);
                if (user != null)
                    return true;
                else
                    return false;
            }
        }

        public int SaveUser(User user)
        {
            lock (locker)
            {
                if(user.Id > 0)
                {
                    database.Update(user);
                    return user.Id;
                }
                else
                {
                    return database.Insert(user);
                }
            }
        }

        public int LoginUser(bool IsLogin = false)
        {
            lock (locker)
            {
                var user = GetLastUserActive();
                user.IsLogin = IsLogin;
                return database.Update(user);
            }
        }

        public int DeleteUser(int id)
        {
            lock (locker)
            {
                return database.Delete<User>(id);
            }
        }
    }
}
