using System;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
namespace TMS
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#if(DEBUG)
            Application.Run(new frmMain());
#else
            dlgLogin dlgLogin = new dlgLogin();
            if (dlgLogin.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new frmMain());
            }
#endif
        }


    }
}
