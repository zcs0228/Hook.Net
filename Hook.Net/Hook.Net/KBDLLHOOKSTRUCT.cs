using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Hook.Net
{
    [StructLayout(LayoutKind.Sequential)]
    public class KBDLLHOOKSTRUCT
    {
        /// <summary>
        /// 虚拟按键码(1--254)
        /// </summary>
        public int vkCode;
        /// <summary>
        /// 硬件按键扫描码
        /// </summary>
        public int scanCode;
        /// <summary>
        /// 键按下：128 抬起：0
        /// </summary>
        public int flags;
        /// <summary>
        /// 消息时间戳间
        /// </summary>
        public int time;
        /// <summary>
        /// 额外信息
        /// </summary>
        public int dwExtraInfo;
    }
}
