using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hook.Net
{
    public class HookHelper
    {
        /// <summary>
        /// 回调函数委托
        /// </summary>
        /// <param name="nCode">钩子链传递回来的参数，0表示此消息(被之前的消息钩子)丢弃，非0表示此消息继续有效</param>
        /// <param name="wParam">消息参数(MsgType枚举)</param>
        /// <param name="lParam">消息参数（KBDLLHOOKSTRUCT和MOUSEHOOKSTRUCT枚举）</param>
        /// <returns></returns>
        private delegate int HookPro(int nCode, int wParam, IntPtr lParam);

        /// <summary>
        /// 钩子执行的用户函数委托
        /// </summary>
        /// <param name="param">按键结构体</param>
        /// <param name="handle"></param>
        public delegate void ProcessKeyHandle(KBDLLHOOKSTRUCT param, out bool handle);

        private static int _hHookValue = 0;

        private HookPro _HookProcedure;

        /// <summary>
        /// 它可以向操作系统(Windows)注册一个特定类型的消息拦截处理方法
        /// </summary>
        /// <param name="idHook">钩子类型，此处用整形的枚举表示</param>
        /// <param name="lpfn">钩子发挥作用时的回调函数</param>
        /// <param name="hInstance">应用程序实例的模块句柄(一般来说是你钩子回调函数所在的应用程序实例模块句柄)</param>
        /// <param name="threadId">与安装的钩子子程相关联的线程的标识符</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private static extern int SetWindowsHookEx(int idHook, HookPro lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(int idHook, int nCode, int wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern int GetCurrentThreadId();

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string name);

        /// <summary>
        /// 按键输入函数
        /// </summary>
        /// <param name="bVk">按键枚举</param>
        /// <param name="bScan"></param>
        /// <param name="dwFlags"></param>
        /// <param name="dwExtraInfo"></param>
        [DllImport("user32.dll", EntryPoint = "keybd_event", SetLastError = true)]
        public static extern void keybd_event(Keys bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        private IntPtr _hookWindowPtr = IntPtr.Zero;

        public HookHelper() { }

        private static ProcessKeyHandle _clientMethod = null;

        [DllImport("user32")]
        public static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern short GetKeyState(int vKey);

        /// <summary>
        /// 安装钩子
        /// </summary>
        /// <param name="clientMethod">钩子执行的用户函数</param>
        public void InstallHook(ProcessKeyHandle clientMethod)
        {
            _clientMethod = clientMethod;
            if (_hHookValue == 0)
            {
                _HookProcedure = new HookPro(OnHookProc);
                string moduleName = Process.GetCurrentProcess().MainModule.ModuleName;
                _hookWindowPtr = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
                //安装线程钩子
                //_hHookValue = SetWindowsHookEx(2, _HookProcedure, IntPtr.Zero, GetCurrentThreadId());
                //安装全局钩子
                _hHookValue = SetWindowsHookEx((int)HookType.WH_KEYBOARD_LL, _HookProcedure, _hookWindowPtr, 0);
                if (_hHookValue == 0) UninstallHook();
            }
        }

        /// <summary>
        /// 卸载钩子
        /// </summary>
        public void UninstallHook()
        {
            if (_hHookValue != 0)
            {
                if (UnhookWindowsHookEx(_hHookValue))
                {
                    _hHookValue = 0;
                }
            }
        }

        /// <summary>
        /// 钩子回调函数
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private static int OnHookProc(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                KBDLLHOOKSTRUCT hookStruct = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                if (_clientMethod != null)
                {
                    bool handle = false;
                    ///Tylan: Judge if the event is KeyDown or not.
                    if (lParam.ToInt32() > 0 && wParam == 0x100)
                    {
                        _clientMethod(hookStruct, out handle);
                    }
                    if (handle) return 1; //返回1是为了禁止转换前的按键输出
                }
            }
            return CallNextHookEx(_hHookValue, nCode, wParam, lParam);
        }
    }
}
