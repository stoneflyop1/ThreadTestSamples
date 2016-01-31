using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestThread
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestTask.RunCanceled();

            //TestTask.RunContinue();

            TestTask.RunFactory();

            Console.ReadKey();
        }
    }
}
