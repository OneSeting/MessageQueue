using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HighConcurrency.Data.Model.BaseClass
{
    public class ConcreteClass
    {

        /// <summary>
        /// 查询的基类
        /// </summary>
        public class QueryBox 
        {
            protected int? QueryPage { set; get; } = 1;

            protected int? QueryIndex { set; get; } = 20;
        }

        public class Identity
        {
           public int Id {set; get; }
        }


    }
}
