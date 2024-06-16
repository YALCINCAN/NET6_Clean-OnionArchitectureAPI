using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Wrappers.Abstract
{
    public interface IPagedDataResponse<T> : IResponse
    {
        int TotalItems { get; }
        T Data { get; }
    }
}
