using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RECVXFlagTool.Utilities
{
    public static class URLHelper
    {
        public static void OpenURL(string url)
        {
            try
            {
                _ = Process.Start(url);
            }
            catch
            {
                // https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    _ = Process.Start(new ProcessStartInfo("cmd", $"/c start {url.Replace("&", "^&")}") { CreateNoWindow = true });
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    _ = Process.Start("xdg-open", url);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    _ = Process.Start("open", url);
                else
                    throw;
            }
        }
    }
}
