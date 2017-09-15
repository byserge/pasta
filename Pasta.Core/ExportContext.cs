using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Pasta.Core
{
    public class ExportContext : MarshalByRefObject
    {
        private Dictionary<string, object> contextItems = new Dictionary<string, object>();

        /// <summary>
        /// Context items stored for the session.
        /// </summary>
        /// <param name="key">The Item key.</param>
        /// <returns>The context item.</returns>
        public object this[string key] {
            get
            {
                object item;
                if (!contextItems.TryGetValue(key, out item))
                    item = null;
                return item;
            }
            set
            {
                contextItems[key] = value;
            }
        }

        public Image Image { get; private set; }

        public byte[] CreateImageByteArray(Guid imageFormatGuid)
        {
            using (var stream = new MemoryStream())
            {
                Image.Save(stream, new System.Drawing.Imaging.ImageFormat(imageFormatGuid));
                return stream.ToArray();
            }
        }

        public ExportContext(Image image)
        {
            Image = image;
        }

        public Stream CreateImageStream(Guid imageFormatGuid)
        {
            var stream = new MemoryStream();
            Image.Save(stream, new System.Drawing.Imaging.ImageFormat(imageFormatGuid));
            return stream;
        }

        public Stream CreateImageStream()
        {
            return CreateImageStream(Image.RawFormat.Guid);
        }
    }
}