using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    public class FileContent : IStreamable
    {
       
        public byte[] Content { get; set; }

        public byte[] LoadFormStream(Stream stream, long size)
        {
            Content = null;
            BinaryReader br = new BinaryReader(stream);
            Content = new byte[size];
            br.Read(Content, 0, (int)size);
            return Content;
        }

        public byte[] LoadFromStream(Stream stream, long size)
        {
            Content = null;
            BinaryReader br = new BinaryReader(stream);
            Content = new byte[size];
            br.Read(Content, 0, (int)size);
            return Content;
        }

        /* public byte[] LoadFromStream(Stream stream, long size)
         {
             Content = null;
             BinaryReader br = new BinaryReader(stream);
             Content = new byte[size];
             br.Read(Content,0, (int)size);
             return Content;

         }*/

        public void SaveToStream(Stream stream)
        {
            BinaryWriter _bw = new BinaryWriter(stream);
            _bw.Write(Content);

            Content = new byte[0]; // PROMENENO NQMASHE GO
        }
    }
}
