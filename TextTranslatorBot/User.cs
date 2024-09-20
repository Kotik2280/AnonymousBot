using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonimusBot
{
    public class User
    {
        public string Name { get; set; }
        public long Id { get; private set; }
        public string ServerConnectedName { get; set; }
        public User(long id, string name, string serverConnectedName = "None")
        {
            Name = name;
            Id = id;
            ServerConnectedName = serverConnectedName;
        }
        public override string ToString()
        {
            return $"{Name} ({Id})";
        }
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            return ((User)obj).Id == this.Id;
        }
    }
}
