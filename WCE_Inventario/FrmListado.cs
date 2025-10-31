using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WCE_Inventario
{
    public partial class FrmListado : Form
    {
        //DataTable dt;
        public FrmListado(/*DataTable dt*/)
        {
            InitializeComponent();
           // this.dt = dt;
        }

        private void FrmListado_Load(object sender, EventArgs e)
        {
            dg_listado.DataSource = Datos.tblDatos;
            this.WindowState = FormWindowState.Maximized;
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmListado_Activated(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }
    }
}