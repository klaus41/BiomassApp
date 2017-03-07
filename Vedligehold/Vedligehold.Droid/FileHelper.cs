using System;
using System.IO;
using Vedligehold.Database;
using Vedligehold.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileHelper))]

namespace Vedligehold.Droid
{
    public class FileHelper : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(path, filename);
        }
    }
}