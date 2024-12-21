using Microsoft.Data.Sqlite;

namespace AnonimusBot
{
    public class Database
    {
        private string connectionString;
        private SqliteConnection connection;
        private SqliteCommand command;
        private string verifedUsers;
        private string usersOnRegistration;

        public Server DefaultServer { get; set; }
        public Database(string connectionString, Server defaultServer)
        {
            verifedUsers = "users";
            usersOnRegistration = "registration";
            this.connectionString = connectionString;

            connection = new SqliteConnection(connectionString);
            connection.Open();

            command = new SqliteCommand() { Connection = this.connection };
            DefaultServer = defaultServer;
        }
        public async Task AddToRegistration(long id)
        {
            command = new SqliteCommand($"INSERT INTO {usersOnRegistration} (id) VALUES (@id)", connection);
            command.Parameters.Add(new SqliteParameter("@id", id));

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
            command = new SqliteCommand($"DELETE FROM {usersOnRegistration} WHERE id = @id", connection);
            command.Parameters.Add(new SqliteParameter("@id", id));

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
            command = new SqliteCommand($"INSERT INTO {verifedUsers} (id, name, server) VALUES (@userId, @userName, @userServerConnectedName)", connection);
            command.Parameters.Add(new SqliteParameter("@userId", user.Id));
            command.Parameters.Add(new SqliteParameter("@userName", user.Name));
            command.Parameters.Add(new SqliteParameter("@userServerConnectedName", user.ServerConnectedName));

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
            command = new SqliteCommand($"SELECT COUNT(id) FROM {verifedUsers} WHERE id = @id", connection);
            command.Parameters.Add(new SqliteParameter("@id", id));

            long updId = (long)await command.ExecuteScalarAsync();

            return updId != 0;
        }
        public async Task<bool> IsUserIdInRegistration(long id)
        {
            command = new SqliteCommand($"SELECT COUNT(*) FROM {usersOnRegistration} WHERE id = @id", connection);
            command.Parameters.Add(new SqliteParameter("@id", id));

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
            command = new SqliteCommand($"SELECT * FROM {verifedUsers} WHERE id = @id", connection);
            command.Parameters.Add(new SqliteParameter("@id", id));

            SqliteDataReader reader = await command.ExecuteReaderAsync();

            reader.Read();

            User user = new User((long)reader.GetValue(0), (string)reader.GetValue(1), (string)reader.GetValue(2));

            reader.Close();

            return await Task.Run(() => user);
        }
        public async Task<List<User>> GetUsersAsync()
        {
            List<User> users = new List<User>();

            command = new SqliteCommand($"SELECT * FROM {verifedUsers}", connection);

            SqliteDataReader reader = await command.ExecuteReaderAsync();

            while (reader.Read())
            {
                User user = new User((long)reader.GetValue(0), (string)reader.GetValue(1));
                users.Add(user);
            }

            reader.Close();

            return await Task.Run(() => users);
        }
        public async Task<List<User>> GetUsersAsync(string serverName)
        {
            List<User> users = new List<User>();

            command = new SqliteCommand($"SELECT * FROM {verifedUsers} WHERE server = @serverName", connection);
            command.Parameters.Add(new SqliteParameter("@serverName", serverName));

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
            command = new SqliteCommand($"SELECT server FROM {verifedUsers} WHERE id = @id", connection);
            command.Parameters.Add(new SqliteParameter("@id", id));

            SqliteDataReader reader = await command.ExecuteReaderAsync();

            reader.Read();

            string server = "None";

            try
            {
                server = (string)reader.GetValue(0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            

            reader.Close();

            return await Task.Run(() => server);
        }
        public async Task SetServerAsync(long id, string serverName)
        {
            command = new SqliteCommand($"UPDATE {verifedUsers} SET server = @serverName WHERE id = @id", connection);
            command.Parameters.Add(new SqliteParameter("@serverName", serverName));
            command.Parameters.Add(new SqliteParameter("@id", id));

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
        public async Task SetUsername(long id, string newUsername)
        {
            command = new SqliteCommand($"UPDATE {verifedUsers} SET name = @newUsername WHERE id = @id", connection);
            command.Parameters.Add(new SqliteParameter("@newUsername", newUsername));
            command.Parameters.Add(new SqliteParameter("@id", id));

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
        public async Task<User?> GetUserAsync(string nickname, string serverName)
        {
            command = new SqliteCommand($"SELECT * FROM {verifedUsers} WHERE name = @username AND server = @serverName", connection);
            command.Parameters.Add(new SqliteParameter("@username", nickname));
            command.Parameters.Add(new SqliteParameter("@serverName", serverName));

            SqliteDataReader reader = await command.ExecuteReaderAsync();

            reader.Read();

            User? user;
            try
            {
                user = new User((long)reader.GetValue(0), (string)reader.GetValue(1), (string)reader.GetValue(2));
            }
            catch (Exception e)
            {
                user = null;

                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            reader.Close();

            return await Task.Run(() => user);
        }

        ~Database()
        {
            connection.Dispose();
            command.Dispose();
        }
    }
}
