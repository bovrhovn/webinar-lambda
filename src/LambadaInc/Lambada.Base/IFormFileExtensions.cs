namespace Lambada.Base
{
    public static class IFormFileExtensions
    {
        public static string GetFileExtension(this Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (file == null) return string.Empty;

            var split = file.FileName.Split(".");

            return split.Length < 1 ? string.Empty : split[1];
        }
    }
}