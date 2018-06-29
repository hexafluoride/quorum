using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum
{
    public class Session
    {
        public User User { get; set; }
        public string Id { get; set; }

        public DateTime Created { get; set; }
        public DateTime ValidUntil { get; set; }

        public bool Valid { get => ValidUntil > DateTime.UtcNow; }

        public Session()
        {

        }

        public Session(User user)
        {
            User = user;
            Id = Guid.NewGuid().ToString("N");

            Created = DateTime.UtcNow;
            ValidUntil = Created.AddDays(1);
        }

        public override string ToString()
        {
            return string.Format("[id={0}, user={1}, {2}]", Id, User, Valid ? "valid" : "invalid");
        }
    }
}
