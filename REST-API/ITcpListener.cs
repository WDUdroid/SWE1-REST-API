using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace REST_API
{
    public interface ITcpListener : IDisposable
    {
        Stream GetStream();
    }
}
