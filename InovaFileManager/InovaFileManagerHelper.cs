using System;
using System.Collections.Generic;
using System.Text;

namespace InovaFileManager
{
    public class InovaFileManagerHelper
    {
        public static string Path = @"c:\MyFiles";
        public static void Configure(string path)
        {
            Path = @path;
        }
    }
}
