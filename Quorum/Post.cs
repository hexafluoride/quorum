using Quorum.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum
{
    public class Post
    {
        public int Id { get; set; }
        public Thread Thread { get; set; }

        public Lazy<User> Author { get; set; }

        public string RawContent { get; set; }
        public string RenderedContent { get; set; }

        public DateTime Created { get; set; }

        internal Post()
        {

        }

        public Post(Thread response_to, User author, string content)
        {
            Thread = response_to;
            Author = new IdentifiedLazy<User>(author.Identifier, delegate { return author; });
            RawContent = content;
        }

        public static IdentifiedLazy<Post> CreateLazily(long id, IPostProvider provider)
        {
            return new IdentifiedLazy<Post>(id, provider.GetPost);
        }
    }
}
