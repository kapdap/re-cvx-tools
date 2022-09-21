using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace RECVXFlagTool.Utilities
{
	public static class WindowHelper
	{
		internal delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

		[DllImport("user32.dll")]
		internal static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern int GetWindowTextLength(IntPtr hWnd);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		public static List<IntPtr> EnumerateProcessWindowHandles(int processId)
		{
			List<IntPtr> handles = new();

			foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
			{
				_ = EnumThreadWindows(thread.Id,
					(hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);
			}

			return handles;
		}

		public static string GetWindowTitle(IntPtr hWnd)
		{
			int length = GetWindowTextLength(hWnd) + 1;
			StringBuilder title = new(length);
			_ = GetWindowText(hWnd, title, length);
			return title.ToString();
		}

		public static string GetClassName(IntPtr hWnd)
		{
			StringBuilder className = new(256);
			_ = GetClassName(hWnd, className, className.Capacity);
			return className.ToString();
		}
	}
}