using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace AnonimusBot
{
    public class Database
    {
        private string connectionString;
        private SqliteConnection connection;
        private SqliteCommand command;
        private string verifedUsers;
        private string usersOnRegistration;
        public Database(string connectionString)
        {
            verifedUsers = "users";
            usersOnRegistration = "registration";
            this.connectionString = connectionString;

            connection = new SqliteConnection(connectionString);
            connection.Open();

            command = new SqliteCommand() { Connection = this.connection };
        }
        public async Task AddToRegistration(long id)
        {
            command.CommandText = $"INSERT INTO {usersOnRegistration} (id) VALUES ({id})";
            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        public async Task RemoveFromRegistration(long id)
        {
            command.CommandText = $"DELETE FROM {usersOnRegistration} WHERE id = {id}";
            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        public async Task AddToVerifedUser(User user)
        {
            command.CommandText = $"INSERT INTO {verifedUsers} (id, name) VALUES ({user.Id}, \"{user.Name}\")";
            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
        public async Task<bool> IsUserIdInVerifed(long id)
        {
            try
            {
                command.CommandText = $"SELECT id FROM {verifedUsers} WHERE id = {id}";
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            
            long updId = 0;
            try
            {
                updId = (long)await command.ExecuteScalarAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            return updId != 0;
        }
        public async Task<bool> IsUserIdInRegistration(long id)
        {
            command.CommandText = $"SELECT COUNT(*) FROM {usersOnRegistration} WHERE id = {id}";
            long count = 0;
            try
            {
                count = (long)await command.ExecuteScalarAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            if (count == 0)
                return await Task.Run(() => false);
            return await Task.Run(() => true);
        }
        public async Task<User> GetVerifedUserById(long id)
        {
            command.CommandText = $"SELECT * FROM {verifedUsers} WHERE id = {id}";

            SqliteDataReader reader = await command.ExecuteReaderAsync();

            reader.Read();

            User user = new User((long)reader.GetValue(0), (string)reader.GetValue(1));

            reader.Close();

            return await Task.Run(() => user);
        }
        public async Task<List<User>> GetUsersAsync()
        {
            List<User> users = new List<User>();

            command.CommandText = $"SELECT * FROM {verifedUsers}";

            SqliteDataReader reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                User user = new User((long)reader.GetValue(0), (string)reader.GetValue(1));
                users.Add(user);
            }

            reader.Close();

            return await Task.Run(() => users);
        }
        public async Task<string> GetServerAsync(long id)
        {
            command.CommandText = $"SELECT server FROM {verifedUsers} WHERE id = {id}";

            SqliteDataReader reader = await command.ExecuteReaderAsync();

            reader.Read();

            string server = (string)reader.GetValue(0);

            reader.Close();

            return await Task.Run(() => server);
        }
        public async Task SetServerAsync(long id, string serverName)
        {
            command.CommandText = $"UPDATE {verifedUsers} SET server = {serverName} WHERE id = {id}";

            try
            {
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        ~Database()
        {
            connection.Dispose();
            command.Dispose();
        }
    }
}
