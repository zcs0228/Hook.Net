using Hook.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChangeKeys
{
    public partial class Form1 : Form
    {
        private HookHelper _keyboardHook = null;

        struct ComboItem
        {
            private string text;
            private string value;

            public ComboItem(string text, string value)
            {
                this.text = text;
                this.value = value;
            }

            public override string ToString()
            {
                return this.text;
            }

            public string ToValue()
            {
                return this.value;
            }
        }

        public Form1()
        {
            InitializeComponent();
            for (int alp = 65; alp <= 90; alp++)
            {
                ComboItem item = new ComboItem(((Keys)alp).ToString(), alp.ToString());
                comboBox1.Items.Add(item);
                comboBox2.Items.Add(item);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Install the hook.
            _keyboardHook = new HookHelper();
            _keyboardHook.InstallHook(this.OnKeyPress);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Cancel the hook.
            if (_keyboardHook != null) _keyboardHook.UninstallHook();
        }

        public void OnKeyPress(KBDLLHOOKSTRUCT hookStruct, out bool handle)
        {
            handle = false;
            if (((Keys)hookStruct.vkCode).ToString() == comboBox1.SelectedItem.ToString())
            {
                handle = true;
                //Exchange the keys.
                hookStruct.vkCode = int.Parse(((ComboItem)comboBox2.SelectedItem).ToValue());
                Keys key = (Keys)hookStruct.vkCode;
                //MessageBox.Show((key == Keys.None ? "" : key.ToString()));

                /**********************第一种按键方式*************************/
                //System.Windows.Forms.SendKeys.Send(key.ToString().ToLower());
                /***************************************************************/

                /**********************第二种按键方式*************************/
                #region 单独按键
                HookHelper.keybd_event(key, 0, 0, 0);
                #endregion

                #region Ctrl+A 组合按键
                //HookHelper.keybd_event(Keys.ControlKey, 0, 0, 0);
                //HookHelper.keybd_event(Keys.A, 0, 0, 0);
                //HookHelper.keybd_event(Keys.ControlKey, 0, 2, 0);
                #endregion

                #region Ctrl+Alt+A 组合按键
                //HookHelper.keybd_event(Keys.ControlKey, 0, 0, 0);
                //HookHelper.keybd_event(Keys.Menu, 0, 0, 0);
                //HookHelper.keybd_event(Keys.A, 0, 0, 0);
                #endregion
                /***************************************************************/
            }
        }
    }
}
