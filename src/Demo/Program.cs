using System;
using System.Windows.Forms;

namespace Demo
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if NET6_0
            ApplicationConfiguration.Initialize();
#endif
            Application.Run(new Form1());
        }
    }
}