using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Pasta.Screenshot
{
    /// <summary>
    /// Allows low level keyboard hooks.
    /// Code mostly taken from: https://blogs.msdn.microsoft.com/toub/2006/05/03/low-level-keyboard-hook-in-c/
    /// </summary>
    internal class KeyboardHook : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;

        private const int WM_KEYDOWN = 0x0100;

        private LowLevelKeyboardProc proc;

        private IntPtr hookID = IntPtr.Zero;

        private Predicate<Keys> keysFilter;

        /// <summary>
        /// Occurs when the key is pressed that satisfies the filter.
        /// </summary>
        public event EventHandler<KeyEventArgs> OnKeyHook;

        /// <summary>
        /// Instantiates <see cref="KeyboardHook"/>.
        /// </summary>
        /// <param name="keysFilter">
        /// The filter for pressed keys.
        /// Only keys that satisfy the filter trigger OnKeyHook event.
        /// </param>
        public KeyboardHook(Predicate<Keys> keysFilter)
        {
            this.keysFilter = keysFilter;
            this.proc = HookCallback;
            this.hookID = SetHook(proc);
        }

        public void Dispose()
        {
            UnhookWindowsHookEx(hookID);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                var key = (Keys)vkCode;
                if (keysFilter(key))
                {
                    OnKeyHook?.Invoke(this, new KeyEventArgs(key));
                }
            }

            return CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
