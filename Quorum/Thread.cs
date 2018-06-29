using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum
{
    public class Thread
    {
        public long Id { get; set; }
        public long Board { get; set; }

        public long OpeningPost { get; set; }

        public string Title { get; set; }

        public Thread()
        {

        }
    }
}
