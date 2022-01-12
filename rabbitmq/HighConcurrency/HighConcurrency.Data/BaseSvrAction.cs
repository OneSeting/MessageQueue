using HighConcurrency.Data.Enumeration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HighConcurrency.Data
{
    public class BaseSvrAction
    {
        /// <summary>
        /// 返回一个数据结构
        /// </summary>
        /// <param name="data"></param>
        /// <param name="msg"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        protected ReturnBox UniData(object data, string msg = "", UniStatusCode statusCode = UniStatusCode.Success)
        {
            if (data == null)
                throw new AggregateException("Data is null");
            var result = new ReturnBox
            {
                Message = msg,
                Data = data,
                Code = (int)statusCode
            };
            return result;
        }

        /// <summary>
        /// 返回一个字符串
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="meta"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        protected ReturnBox UniMessage(string msg, string meta = "", UniStatusCode statusCode = UniStatusCode.Success)
        {
            var result = new ReturnBox
            {
                Message = msg,
                Meta = meta,
                Code = (int)statusCode
            };
            return result;
        }

        /// <summary>
        /// 返回一个分页
        /// </summary>
        /// <param name="data"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalCount"></param>
        /// <param name="msg"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        protected ReturnBox UniPagedBox(object data, int pageIndex = 0, int pageSize = 0,long totalCount = 0, string msg = "", UniStatusCode statusCode = UniStatusCode.Success)
        {
            if (data == null)
                throw new ArgumentException("Parameter is null");

            var result = new ReturnBox
            {
                Data = data,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount,
                Message = msg,
                Code = (int)statusCode
            };
            return result;
        }

        /// <summary>
        /// 返回一个状态
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected ReturnBox UniStatus(bool isSuccess,string msg = "")
        {
            msg = isSuccess? (string.IsNullOrEmpty(msg)? "Success": msg) :(string.IsNullOrEmpty(msg) ? "Failed" : msg);

            if (isSuccess)
                return UniMessage("Success");

            return UniError();
        }

        /// <summary>
        /// 返回正确的状态
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        protected ReturnBox UniSuccess(string msg = "",UniStatusCode statusCode = UniStatusCode.Success)
        {
            return UniMessage(string.IsNullOrEmpty(msg)? "Success": msg, "", statusCode);
        }

        /// <summary>
        /// 返回一个失败的状态
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        protected ReturnBox UniError(string msg = "",UniStatusCode statusCode = UniStatusCode.Error)
        {
            return UniMessage(string.IsNullOrEmpty(msg) ? "Failed" : msg, "", statusCode);
        }

        /// <summary>
        /// Convert Object to Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected T ConvertObjToModel<T>(object obj)
        {
            return JsonConvert.DeserializeObject<T>(obj.ToString());
        }
    }
}
