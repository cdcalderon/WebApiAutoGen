using System;
using System.IO;

namespace YPrime.eCOA.DTOLibrary
{
    [Serializable]
    public class ExportStream
    {
        public string Name { get; set; }

        public MemoryStream MemoryStream { get; set; }

        public string Extension { get; set; }
    }
}