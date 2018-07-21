using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum.Providers
{
    public interface IPostProvider
    {
        Post GetPost(long id);
        Post[] GetPostsByThread(long thread_id, long start, long count);

        long CreatePost();
        bool UpdatePost(long post_id, Post post);
    }
}
