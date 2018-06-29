using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum.Providers
{
    public class DummySessionProvider : ISessionProvider
    {
        public Dictionary<string, Session> Sessions = new Dictionary<string, Session>();

        public Session CreateSession(User user)
        {
            var session = new Session(user);
            Sessions.Add(session.Id, session);

            return session;
        }

        public void DestroySession(Session session)
        {
            if (Sessions.ContainsKey(session.Id))
                Sessions.Remove(session.Id);
        }

        public Session RetrieveSession(string session)
        {
            if (Sessions.ContainsKey(session))
                return Sessions[session];

            return null;
        }
    }
}
