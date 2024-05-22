using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Interpreter322
{
    public partial class Interface : Form
    {
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        public Interface()
        {
            InitializeComponent();
            AllocConsole();
            ShowWindow(GetConsoleWindow(), SW_HIDE);
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            string code = code_input.Text;

            ShowWindow(GetConsoleWindow(), SW_SHOW);
            bool is_open = true;
            while (is_open)
            {
                try
                {
                    Interpreter program = new Interpreter(code);
                    program.Execute();
                    if (!code.Contains("DISPLAY:"))
                    Console.WriteLine("No Error");
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception.StackTrace);
                    Console.WriteLine(exception.Message);
                }
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                is_open = false;
            }
            Console.Clear();
            ShowWindow(GetConsoleWindow(), SW_HIDE);
        }
    }
}
