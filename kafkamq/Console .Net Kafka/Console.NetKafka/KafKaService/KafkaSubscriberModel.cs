using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafKaService
{
    public class KafkaSubscriberModel
    {
        /// <summary>
        /// 主题
        /// </summary>
        public string Topic { get; set; }
        /// <summary>
        /// 分区
        /// </summary>
        public int? Partition { get; set; }
    }
}
