using System;
using System.Threading.Tasks;

namespace m.transport.Interfaces
{
    public interface IThumbnail
    {
        Task<string> GetThumbnailPath(string privatePath);
    }
}

