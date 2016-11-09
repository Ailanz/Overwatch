using AForge.Imaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Overwatch
{
    public class ScreenDraw : NativeWindow
    {
        private const int WM_LBUTTONUP = 0x202;

        static int hHook = 0;
        public const int WH_KEYBOARD_LL = 13;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_PAINT = 0xF;


        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);


        public ScreenDraw()
        {
            _hookID = SetHook(_proc);

            if (hHook == 0)
            {
                Console.WriteLine("SetWindowsHookEx Failed");
                return;
            }
            UnhookWindowsHookEx(_hookID);

        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {

            using (Process curProcess = Process.GetCurrentProcess())

            using (ProcessModule curModule = curProcess.MainModule)
            {

                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,

                    GetModuleHandle(curModule.ModuleName), 0);

            }

        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);


        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {

            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {

                int vkCode = Marshal.ReadInt32(lParam);

                Console.WriteLine((Keys)vkCode);

            }

            

            return CallNextHookEx(_hookID, nCode, wParam, lParam);

        }


        public static void DrawScreen(bool isOn)
        {
            //var hook = EasyHook.LocalHook.Create(
            //        EasyHook.LocalHook.GetProcAddress("user32.dll", "MessageBeep"),
            //        new MessageBeepDelegate(MessageBeepHook),
            //        null);
            //hook.ThreadACL.SetInclusiveACL(new int[] { 0 });
            //PlayMessageBeep();
            //Thread.Sleep(1000);
            UnmanagedImage bmp = Form1.imageToDraw;
            if (bmp != null)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Point pt = System.Windows.Forms.Cursor.Position; // Get the mouse cursor in screen coordinates 

                using (Graphics g = Graphics.FromHwnd(ScreenShotDemo.ScreenCapture.User32.GetForegroundWindow()))
                {

                    //g.DrawImage(bmp.ToManagedImage(), new Point(2 * bmp.Width, 2 * bmp.Height));

                    //Thread.Sleep(1000);
                }
                // Console.WriteLine("TIME: " + sw.ElapsedMilliseconds);
                
                sw.Stop();
                sw.Reset();
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }

        //Declare the wrapper managed MouseHookStruct class.
        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public POINT pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        private static extern IntPtr SetWindowsHookEx(int idHook,

            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        [return: MarshalAs(UnmanagedType.Bool)]

        private static extern bool UnhookWindowsHookEx(IntPtr hhk);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,

            IntPtr wParam, IntPtr lParam);


        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        private static extern IntPtr GetModuleHandle(string lpModuleName);

    }
}
