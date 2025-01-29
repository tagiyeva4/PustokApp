namespace TemplatePustokApp.Helpers
{
    public static class FileManager
    {
        public static string SaveImage(this IFormFile file,string path,string folder)
        {
            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            string fullPath = Path.Combine(path, fileName);
            using FileStream fileStream = new FileStream(path, FileMode.Create);//using de yaziriq cunki isi bitenden sonra dispose elesin
            file.CopyTo(fileStream);
            return fileName;

        }
        public static bool CheckSize(this IFormFile file,int maxSize)
        {
            return file.Length >= maxSize;
        }
        public static bool CheckType(this IFormFile file, string[] types)
        {
         return types.Contains(file.ContentType);
        }
         public static bool DeleteFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
                return true;
            }
            return false;
        }
    }
}
