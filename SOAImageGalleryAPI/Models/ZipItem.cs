using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOAImageGalleryAPI.Models
{
    public class ZipItem
    {
        public string FileName { get; set; }
        public Stream Content { get; set; }

        public ZipItem() { }

        public ZipItem(string filename, Stream content)
        {
            this.FileName = filename;
            this.Content = content;
        }

        public ZipItem(string filename, string contentStr, Encoding encoding)
        {
            // convert string to stream
            var byteArray = encoding.GetBytes(contentStr);
            var memoryStream = new MemoryStream(byteArray);

            this.FileName = filename;
            this.Content = memoryStream;
        }

    }
}
