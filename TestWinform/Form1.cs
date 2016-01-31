using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace TestWinform
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            _scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }

        private readonly TaskScheduler _scheduler;

        private CancellationTokenSource _cts;

        private static Int32 Sum(CancellationToken ct, Int32 n)
        {
            var sum = 0;
            for (; n > 0; n--)
            {
                ct.ThrowIfCancellationRequested();

                checked { sum += n; }
            }
            return sum;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts = null;
            }
            else
            {
                Text = "Operation Running...";
                _cts = new CancellationTokenSource();

                var t = Task.Run(() => Sum(_cts.Token, 20000), _cts.Token);

                t.ContinueWith(task => Text = "Result: " + task.Result, CancellationToken.None,
                    TaskContinuationOptions.OnlyOnRanToCompletion, _scheduler);

                t.ContinueWith(task => Text = "Operation Canceled...", CancellationToken.None,
                    TaskContinuationOptions.OnlyOnCanceled, _scheduler);

                t.ContinueWith(task => Text = "Operation Faulted...", CancellationToken.None,
                    TaskContinuationOptions.OnlyOnFaulted, _scheduler);
            }

            base.OnMouseClick(e);
        }
    }
}
