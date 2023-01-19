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

        public void LoadFromStream(Stream stream, long size)
        {
            Content = null;
            BinaryReader br = new BinaryReader(stream);
          //  br.ReadInt64();
         //   br.ReadInt64();
            Content = new byte[size];
            br.Read(Content,0, (int)size);
            for (int i = 0; i < Content.Length; i++)
            {
                Console.WriteLine(Content[i]);
            }
           // br.Read()
        }

        public void SaveToStream(Stream stream)
        {
            BinaryWriter _bw = new BinaryWriter(stream);
            _bw.Write(Content);

            Content = new byte[0]; // PROMENENO NQMASHE GO
        }
    }
}
