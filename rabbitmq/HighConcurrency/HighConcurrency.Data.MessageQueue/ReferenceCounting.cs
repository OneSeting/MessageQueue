using System;
using System.Collections.Generic;
using System.Text;

namespace HighConcurrency.Data.MessageQueue
{
    public static class ReferenceCounting
    {
        public static int COUNT { set; get; } = 0;
    }
}
