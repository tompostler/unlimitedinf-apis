using System;

namespace Unlimitedinf.Apis.Client
{
    internal class TomIsLazyException : NotImplementedException
    {
        public TomIsLazyException()
        { }

        public TomIsLazyException(string message)
            : base(message)
        { }

        public TomIsLazyException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
