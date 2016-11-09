using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;

namespace Overwatch
{
    public static class Mouse
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);


        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT point);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        [DllImport("user32.dll")]
        private static extern bool BlockInput(bool block);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        public static void FreezeMouse()
        {
            BlockInput(true);
        }

        public static void ThawMouse()
        {
            BlockInput(false);
        }

        public static void SmoothMoveX(InputSimulator sim, int moveX, int multiplier)
        {
            if (moveX < 0)
            {
                multiplier = -1;
            }
            for (int i = 0; i < moveX * multiplier; i++)
            {
                sim.Mouse.MoveMouseBy(1 * multiplier, 0);
                //Thread.Sleep(1);
            }
        }

        public static void SmoothMoveY(InputSimulator sim, int moveY, int multiplier)
        {
            if (moveY < 0)
            {
                multiplier = -1;
            }
            for (int i = 0; i < moveY * multiplier; i++)
            {
                sim.Mouse.MoveMouseBy(0, 1 * multiplier);
                //Thread.Sleep(1);
            }
        }

        public static void HardMoveY(InputSimulator sim, int moveY, int multiplier)
        {
            sim.Mouse.MoveMouseBy(0, moveY * multiplier);            
        }

        public static void HardMoveX(InputSimulator sim, int moveX, int multiplier)
        {
            sim.Mouse.MoveMouseBy(moveX * multiplier, 0);
        }

      
        public static void LeftMouseDown()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        }

        public static void LeftMouseUp()
        {
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        //This simulates a left mouse click
        public static void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }
    }
}
