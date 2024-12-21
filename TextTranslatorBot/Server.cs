namespace AnonimusBot
{
    public class Server
    {
        public string Name { get; private set; }
        public int MaxUsers { get; private set; }
        public int OnlineUsers { get; private set; }
        public List<User> Users { get; private set; }
        public Server(string name, int maxUsers = 16) 
        {
            MaxUsers = maxUsers;
            Name = name;
            Users = new List<User>();
        }

        public async Task<bool> AddUser(User user)
        {
            if (OnlineUsers + 1 > Users.Count)
                return await Task.Run(() => false);
            Users.Add(user);
            OnlineUsers++;
            user.ServerConnectedName = Name;
            return await Task.Run(() => true);
        }
        public async Task<bool> RemoveUser(User user)
        {
            if (OnlineUsers - 1 < 0)
                return await Task.Run(() => false);
            try
            {
                Users.Remove(user);
            }
            catch (Exception)
            {
                return await Task.Run(() => false);
            }
            OnlineUsers--;
            user.ServerConnectedName = "None";
            return await Task.Run(() => true);
        }
        public async Task<bool> IsUserInServer(User user)
        {
            foreach (User serverUser in Users)
            {
                if (serverUser.Id == user.Id)
                    return await Task.Run(() => true);
            }
            return await Task.Run(() => false);
        }
        public override string ToString()
        {
            return $"{Name} [{OnlineUsers}/{MaxUsers}]";
        }
    }
}
