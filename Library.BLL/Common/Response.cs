using System;
using System.Collections.Generic;
using System.Text;

namespace Library.BLL.Common
{
    public record Response<T>(T? Data , string? Message = null, bool IsSuccess = true)
    {
    }
}
