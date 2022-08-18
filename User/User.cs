using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserNamespace {
    [Serializable]
    public class User {
        public int CardNumber { get; set; } = 7;
        public string Name { get; set; }
        public string Id { get; set; }
        public User() { Name = "您"; Id = "Local User"; }
        public User(string n, string i) { Name = n; Id = i; }
        public bool Equals(User usr) {
            return this.Id == usr.Id;
        }
    }
}
