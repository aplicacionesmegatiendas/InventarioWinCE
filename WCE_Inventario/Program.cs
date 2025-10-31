using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace WCE_Inventario
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        /// 

        [MTAThread]
        static void Main()
        {
            Application.Run(new FrmEntrada());
        }
    }
}