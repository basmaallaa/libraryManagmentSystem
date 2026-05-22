using Library.BLL.Common;
using Library.BLL.ModelVM.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library.BLL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Response<string>> RegisterAsync(RegisterVM vm);
        Task<Response<string>> LoginAsync(LoginVM vm);
        Task LogoutAsync();
    }
}
