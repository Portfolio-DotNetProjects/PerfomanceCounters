using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using System.Threading;

namespace StackOverflow
{
    [SupportedOSPlatform("windows")]
    class Program
    {
        const int MIN_ITERATIONS = int.MaxValue / 1000;
        const int MAX_ITERATIONS = MIN_ITERATIONS + 10000;

        long m_totalIterations = 0;
        readonly object m_totalItersLock = new object();
        // The method that will be called when the thread is started.
        public void DoWork()
        {
            Console.WriteLine(
                "ServerClass.InstanceMethod is running on another thread.");

            var x = GetNumber();
        }

        private int GetNumber()
        {
            var rand = new Random();
            var iters = rand.Next(MIN_ITERATIONS, MAX_ITERATIONS);
            var result = 0;
            lock (m_totalItersLock)
            {
                m_totalIterations += iters;
            }
            // we're just spinning here
            // and using Random to frustrate compiler optimizations
            for (var i = 0; i < iters; i++)
            {
                result = rand.Next();
            }
            return result;
        }
        static void Main()
        {

            for (int i = 0; i < 200; i++)
            {
                CreateThreads();
            }

            Process p = Process.GetCurrentProcess();

            PerformanceCounter myAppCpu =
                new(
                    "Processo", "% tempo privilegiado", p.ProcessName, true);
            Teste();

            Console.WriteLine("Press the any key to stop...\n");
            while (!Console.KeyAvailable)
            {
                Process currentProcess = Process.GetCurrentProcess();
                long totalBytesOfMemoryUsed = currentProcess.PrivateMemorySize64;
                double pct = myAppCpu.NextValue();
                Console.WriteLine($"{p.ProcessName} CPU % = {pct} ");
                Console.WriteLine($"{p.ProcessName} memory  = {totalBytesOfMemoryUsed} ");
                Thread.Sleep(250);
            }
        }
        public static void Teste()
        {
            string machineName = "";
            PerformanceCounterCategory[] categories;
            Process p = Process.GetCurrentProcess();

            // Copy the machine name argument into the local variable.

            machineName = p.MachineName;


            // Generate a list of categories registered on the specified computer.
            try
            {
                if (machineName.Length > 0)
                {
                    categories = PerformanceCounterCategory.GetCategories(machineName);
                }
                else
                {
                    categories = PerformanceCounterCategory.GetCategories();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to get categories on " +
                    (machineName.Length > 0 ? "computer \"{0}\":" : "this computer:"), machineName);
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine("These categories are registered on " +
                (machineName.Length > 0 ? "computer \"{0}\":" : "this computer:"), machineName);

            // Create and sort an array of category names.
            string[] categoryNames = new string[categories.Length];
            int objX;
            for (objX = 0; objX < categories.Length; objX++)
            {
                categoryNames[objX] = categories[objX].CategoryName;
            }
            Array.Sort(categoryNames);

            for (objX = 0; objX < categories.Length; objX++)
            {
                Console.WriteLine("{0,4} - {1}", objX + 1, categoryNames[objX]);
            }

            PerformanceCounterCategory pcc = new PerformanceCounterCategory("Processo", machineName);
            PerformanceCounter[] counters = pcc.GetCounters(p.ProcessName);

            foreach (var counter in counters)
            {
                Console.WriteLine($"{counter.CounterName}");
            }
        }
        public static void CreateThreads()
        {
            Program p = new Program();
            Thread InstanceCaller = new Thread(new ThreadStart(p.DoWork));
            // Start the thread.
            InstanceCaller.Start();

            Console.WriteLine("The Main() thread calls this after "
                + "starting the new InstanceCaller thread.");

        }

    }
}


