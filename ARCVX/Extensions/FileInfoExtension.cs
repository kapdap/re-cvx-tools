using System.IO;

namespace ARCVX.Extensions
{
    public static class FileInfoExtension
    {
        public static FileStream OpenReadShared(this FileInfo file) =>
            file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    }
}
