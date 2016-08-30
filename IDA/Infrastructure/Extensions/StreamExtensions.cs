using System.IO;

namespace ConceptONE.Infrastructure.Extensions
{
    public static class StreamExtensions
    {

        public static void SaveTo(this Stream stream, string localPath)
        {
            using (FileStream fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write))
                stream.CopyTo(fileStream);
        }

        public static byte[] ReadAllBytes(this Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            byte[] result = new byte[] { };
            int read;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    memoryStream.Write(buffer, 0, read);

                result = memoryStream.ToArray();
            }

            return result;
        }

    }
}
