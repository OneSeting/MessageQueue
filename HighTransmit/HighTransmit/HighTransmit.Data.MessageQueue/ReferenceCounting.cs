using System;
using System.Collections.Generic;
using System.Text;

namespace HighTransmit.Data.MessageQueue
{
    public static class ReferenceCounting
    {
        public static int Count { set; get; } = 0;
    }
}
