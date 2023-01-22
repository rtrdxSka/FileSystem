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
        byte[] Content { get; set; }

        void SaveToStream(Stream stream);
        byte[] LoadFromStream(Stream stream, long size);
        byte[] LoadFormStream(Stream stream, long size);
        /* byte[] LoadFormStream(Stream stream, long size);*/
    }
}
