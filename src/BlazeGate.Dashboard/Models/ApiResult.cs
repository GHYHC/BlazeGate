﻿namespace BlazeGate.Dashboard.Models
{
    public class ApiResult<T>
    {
        public int Code { get; set; }

        public bool Success { get; set; }

        public string Msg { get; set; }

        public T Data { get; set; }
    }
}