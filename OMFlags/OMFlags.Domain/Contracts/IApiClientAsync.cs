using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMFlags.Domain.Contracts
{
    public interface IApiClientAsync : IAsyncDisposable
    {
        Task<T> GetAsync<T>(Uri uri);
    }
}
