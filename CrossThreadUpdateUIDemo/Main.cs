using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DateTime = System.DateTime;

namespace CrossThreadUpdateUIDemo
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void btnInvoke_Click(object sender, EventArgs e)
        {
            var thread = new Thread(() =>
            {
                while (true)
                {
                    Action action = () => txtTime1.Text = DateTime.Now.ToLongTimeString();

                    if (InvokeRequired && !Disposing)
                    {
                        Invoke(action);
                    }
                    else
                    {
                        action();
                    }

                    Task.Delay(1000);
                }
            }) {IsBackground = true};

            thread.Start();
        }

        private void btnSyncContext_Click(object sender, EventArgs e)
        {
            var uiContext = SynchronizationContext.Current;

            var thread = new Thread(ctx =>
            {
                var context = ctx as SynchronizationContext;
                if (context == null)
                {
                    return;
                }

                while (true)
                {
                    context.Send(state =>
                    {
                        var dateString = state as string;
                        if (string.IsNullOrEmpty(dateString))
                        {
                            return;
                        }

                        txtTime2.Text = dateString;
                    }, DateTime.Now.ToLongTimeString());
                }
            }) {IsBackground = true};

            thread.Start(uiContext);
        }
    }
}