using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum.Providers
{
    interface IThreadProvider
    {
        Thread GetThread(long id);
        Thread[] GetThreadsByBoardBumpOrdered(long board, long offset, long count);

        long CreateThread();
        long CreateThread(long opening_post);
        long CreateThread(long opening_post, long board);
        bool UpdateThread(long thread_id, Thread thread);
    }
}
