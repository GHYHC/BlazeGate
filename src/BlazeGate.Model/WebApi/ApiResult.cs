using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Model.WebApi
{
    public class ApiResult<T>
    {
        public int Code { get; set; }

        public bool Success { get; set; }

        public string Msg { get; set; }

        public T Data { get; set; }

        public static ApiResult<T> Result(bool success, T data, string msg = null)
        {
            if (success)
            {
                return SuccessResult(data, msg);
            }
            else
            {
                return FailResult(msg);
            }
        }

        public static ApiResult<T> SuccessResult(T data, string msg = null)
        {
            return new ApiResult<T>()
            {
                Code = 200,
                Success = true,
                Msg = msg ?? "Success",
                Data = data
            };
        }

        public static ApiResult<T> FailResult(string msg = null)
        {
            return new ApiResult<T>()
            {
                Code = 200,
                Success = false,
                Msg = msg ?? "Failure",
                Data = default
            };
        }
    }
}