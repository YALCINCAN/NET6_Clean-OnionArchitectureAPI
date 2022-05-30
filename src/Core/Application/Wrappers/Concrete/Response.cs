using Application.Wrappers.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Wrappers.Concrete
{
    public class Response : IResponse
    {
        public bool Success { get; }

        public int StatusCode { get; }

        public Response(bool success, int statuscode)
        {
            Success = success;
            StatusCode = statuscode;
        }

        public Response(bool success)
        {
            Success = success;
        }
    }
}
