using System;
using System.IO;
using System.Text;
using System.Threading;

namespace NS_ConsoleRemap
{
    public class ConsoleRemapTextBox : TextWriter
    {
        private System.Windows.Forms.TextBox textBox;
        public ConsoleRemapTextBox(System.Windows.Forms.TextBox textBox)
        {
            this.textBox = textBox;
            Console.SetOut(this);
        }

        public override void Write(string value)
        {
            /* 控件创建之后，才能打印数据 */
            if (textBox.IsHandleCreated)
            {
                /* 线程安全的方式调用 */
                textBox.Invoke(
                    new ThreadStart(
                        () =>
                        {
                            DateTime dt = DateTime.Now;
                            string time = dt.ToString("[ yyyy-MM-dd HH:mm:ss.fff ] ");
                            textBox.AppendText(time + value);
                        }
                    )
                );
            }
        }
        public override void WriteLine(string value)
        {
            Write(value + "\r\n");
        }

        public override Encoding Encoding
        {
            get { return Encoding.UTF8; }
        }
    }
}

