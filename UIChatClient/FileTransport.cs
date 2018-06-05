using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIChatClient
{
    public class FileTransport
    {
        public string FileName { get; set; }
        public string Expansion { get; set; }
        public byte[] Data { get; set; }
    }
}
