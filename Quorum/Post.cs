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
        public long Id { get; set; }
        public long Thread { get; set; }
        public long Board { get; set; }

        public long Author { get; set; }

        public string RawContent { get; set; }
        public string RenderedContent { get; set; }
        public string Title { get; set; }

        public string Renderer { get; set; }

        public DateTime Created { get; set; }

        internal Post()
        {

        }

        public Post(long thread, long author, string content, string content_renderer = "markdown")
        {
            Thread = thread;
            Author = author;

            RawContent = content;
            Renderer = content_renderer;
        }
    }

    public interface IPostRenderer
    {
        string Name { get; }
        string Render(Post post);
    }
}
