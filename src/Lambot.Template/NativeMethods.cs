using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Template;

public static partial class NativeMethods
{
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool OpenClipboard(IntPtr hWndNewOwner);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CloseClipboard();

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool EmptyClipboard();

    [LibraryImport("user32.dll")]
    private static partial IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial IntPtr GlobalLock(IntPtr hMem);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GlobalUnlock(IntPtr hMem);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial IntPtr GlobalFree(IntPtr hMem);

    public const uint CF_TEXT = 1;
    public const uint GMEM_MOVEABLE = 0x0002;
    public static void SetClipboard(string text)
    {
        // 打开剪贴板
        if (!OpenClipboard(IntPtr.Zero))
        {
            throw new Exception("Unable to open clipboard.");
        }

        try
        {
            // 清空剪贴板
            if (!EmptyClipboard())
            {
                throw new Exception("Unable to empty clipboard.");
            }

            // 将文本转换为字节数组
            byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(text);

            // 分配全局内存
            IntPtr hGlobal = GlobalAlloc(GMEM_MOVEABLE, (UIntPtr)(bytes.Length + 1));
            if (hGlobal == IntPtr.Zero)
            {
                throw new Exception("Unable to allocate global memory.");
            }

            // 锁定内存
            IntPtr pGlobal = GlobalLock(hGlobal);
            if (pGlobal == IntPtr.Zero)
            {
                GlobalFree(hGlobal);
                throw new Exception("Unable to lock global memory.");
            }

            try
            {
                // 将文本复制到全局内存
                Marshal.Copy(bytes, 0, pGlobal, bytes.Length);
                Marshal.WriteByte(pGlobal, bytes.Length, 0); // 添加null终止符

                // 设置剪贴板数据
                if (SetClipboardData(CF_TEXT, hGlobal) == IntPtr.Zero)
                {
                    throw new Exception("Unable to set clipboard data.");
                }

                // 剪贴板现在拥有内存，因此不应调用GlobalFree
                hGlobal = IntPtr.Zero;
            }
            finally
            {
                if (pGlobal != IntPtr.Zero)
                {
                    GlobalUnlock(hGlobal);
                }
            }
        }
        finally
        {
            CloseClipboard();
        }
    }
}

