using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quorum
{
    public class IdentifiedLazy<T> : IdentifiedLazy<long, T>
    {
        public IdentifiedLazy(long identifier, Func<long, T> getter) :
            base(identifier, getter)
        {

        }
    }

    public class IdentifiedLazy<I, T> : Lazy<T>
    {
        public I Identifier { get; set; }

        public IdentifiedLazy(I identifier, Func<I, T> getter) :
            base(delegate { return getter(identifier); })
        {
            Identifier = identifier;
        }
    }
}
