using System;
using System.Collections.Generic;
using System.Text;

namespace HighConcurrency.Data
{
    public class ReturnBox
    {
        public int Code { get; set; } = 0;
        public object Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Meta { get; set; } = string.Empty;
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 0;
        public long TotalCount { get; set; } = 0;
    }
}
