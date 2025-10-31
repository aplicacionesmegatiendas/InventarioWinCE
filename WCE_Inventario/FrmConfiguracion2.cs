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
    public partial class FrmConfiguracion2 : Form
    {
        public FrmConfiguracion2()
        {
            InitializeComponent();
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            try
            {
                Datos.EscribirConfiguracion(Datos.configuracion[0] + "," + Datos.configuracion[1] + "," + Datos.configuracion[2] + "," + txt_servidor.Text.Trim() + "," + txt_usuario.Text.Trim() + "," + txt_contra.Text.Trim() + "," + txt_basedatos.Text.Trim() + "," + txt_usuariounoee.Text.Trim() + "," + txt_id_cia.Text.Trim());
                txt_servidor.Text = ""; txt_usuariounoee.Text = ""; txt_contra.Text = "";
                txt_basedatos.Text = ""; txt_usuariounoee.Text = ""; txt_id_cia.Text = "";
                Datos.CargarConfiguracion();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
            }
        }

        private void FrmConfiguracion2_Activated(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private void FrmConfiguracion2_Load(object sender, EventArgs e)
        {
            try
            {
                Datos.CargarConfiguracion();
                txt_servidor.Text = Datos.configuracion[3];
                txt_usuario.Text = Datos.configuracion[4];
                txt_contra.Text = Datos.configuracion[5];
                txt_basedatos.Text = Datos.configuracion[6];
                txt_usuariounoee.Text = Datos.configuracion[7];
                txt_id_cia.Text = Datos.configuracion[8];
                this.WindowState = FormWindowState.Maximized;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
            }
        }

        private void btn_probar_Click(object sender, EventArgs e)
        {
            if (txt_servidor.Text.Trim().Equals("") || txt_usuario.Text.Trim().Equals("") || txt_contra.Text.Trim().Equals("") || txt_basedatos.Text.Trim().Equals("") || txt_usuariounoee.Text.Trim().Equals(""))
            {
                MessageBox.Show("Escriba todos los datos de configuración", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return;
            }
            try
            {
                btn_ok.Enabled = false;
                if (Conexion.ProbarConexion(txt_servidor.Text, txt_basedatos.Text, txt_usuario.Text, txt_contra.Text, txt_usuariounoee.Text))
                {
                    btn_ok.Enabled = true;
                    MessageBox.Show("La conexión se realizo correctamente", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                }
                else
                {
                    btn_ok.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
            }
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}