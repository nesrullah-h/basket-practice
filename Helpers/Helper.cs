using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace fiorello.Helpers
{
    public class Helper
    {
        public static void DeleteImg(string root, string folder, string file)
        {
            string fullPath = Path.Combine(file, root, folder);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}

