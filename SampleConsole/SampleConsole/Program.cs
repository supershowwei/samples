using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace SampleConsole
{
    public delegate bool HandlerRoutine(CtrlTypes ctrlType);

    internal class Program
    {
        private static readonly AutoResetEvent Waiting = new AutoResetEvent(false);

        private static bool isClosing;

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(HandlerRoutine handler, bool add);

        private static bool ConsoleCtrlHandler(CtrlTypes ctrlType)
        {
            switch (ctrlType)
            {
                case CtrlTypes.CTRL_C_EVENT:
                    isClosing = true;
                    Console.WriteLine("CTRL+C received!");
                    break;

                case CtrlTypes.CTRL_BREAK_EVENT:
                    isClosing = true;
                    Console.WriteLine("CTRL+BREAK received!");
                    break;

                case CtrlTypes.CTRL_CLOSE_EVENT:
                    isClosing = true;
                    Console.WriteLine("Program being closed!");
                    break;

                case CtrlTypes.CTRL_LOGOFF_EVENT:
                case CtrlTypes.CTRL_SHUTDOWN_EVENT:
                    isClosing = true;
                    Console.WriteLine("User is logging off!");
                    break;
            }

            return true;
        }

        private static void Main(string[] args)
        {
            SetConsoleCtrlHandler(ConsoleCtrlHandler, true);

            Task.Run(
                () =>
                    {
                        while (!isClosing)
                        {
                            Thread.Sleep(1000 - DateTime.Now.Millisecond);
                            Console.WriteLine(DateTime.Now.ToString("HH:mm:ss.fff") + " Test");
                        }

                        Waiting.Set();
                    });

            Console.WriteLine("Something executing...");

            Waiting.WaitOne();
        }
    }
}