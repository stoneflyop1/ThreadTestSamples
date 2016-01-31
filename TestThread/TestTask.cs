using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
namespace TestThread
{
    class TestTask
    {
        private static Int32 Sum(CancellationToken ct, Int32 n)
        {
            var sum = 0;
            for(;n>0;n--)
            {
                ct.ThrowIfCancellationRequested();

                checked { sum += n; }
            }
            return sum;
        }

        public static void RunCanceled()
        {
            var cts = new CancellationTokenSource();
            var t = Task.Run(() => Sum(cts.Token, 1000000000), cts.Token);

            //Thread.Sleep(500); //会导致Sum执行

            cts.Cancel();

            try
            {
                Console.WriteLine("The sum is: " + t.Result);
            }
            catch(AggregateException x)
            {
                x.Handle(e => e is OperationCanceledException);

                Console.WriteLine("Sum was canceled");
            }
        }

        public static Task RunContinue()
        {
            var t = Task.Run(() => Sum(CancellationToken.None, 10000));

            var cwt = t.ContinueWith(task => Console.WriteLine("The sum is: " + task.Result));

            return cwt;
        }

        public static void RunFactory()
        {
            var parent = new Task(() =>
            {
                var cts = new CancellationTokenSource();
                var tf = new TaskFactory<Int32>(cts.Token, TaskCreationOptions.AttachedToParent,
                    TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);

                var childTasks = new[]
                {
                    tf.StartNew(() => Sum(cts.Token, 10000)),
                    tf.StartNew(()=>Sum(cts.Token, 20000)),
                    tf.StartNew(()=>Sum(cts.Token, Int32.MaxValue))
                };

                for (var task = 0; task < childTasks.Length; task++)
                {
                    childTasks[task].ContinueWith(t => cts.Cancel(), 
                        TaskContinuationOptions.OnlyOnFaulted);
                }

                tf.ContinueWhenAll(childTasks,
                    completedTasks =>
                        completedTasks.Where(t => 
                        t.Status == TaskStatus.RanToCompletion).Max(t => t.Result), CancellationToken.None).ContinueWith(
                    t=>Console.WriteLine("The maximum is: " + t.Result), TaskContinuationOptions.ExecuteSynchronously);

            });

            parent.ContinueWith(p =>
            {
                var sb = new StringBuilder("The following exception(s) occurred: " + Environment.NewLine);

                foreach(var e in p.Exception.Flatten().InnerExceptions)
                {
                    sb.AppendLine("    " + e.GetType());
                }
                Console.WriteLine(sb);

            }, TaskContinuationOptions.OnlyOnFaulted);

            parent.Start();
        }
    }
}
