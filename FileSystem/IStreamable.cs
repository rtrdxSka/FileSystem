using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    public interface IStreamable
    {
        void SaveToStream(Stream stream);
        void LoadFromStream(Stream stream, long size);
    }
}
