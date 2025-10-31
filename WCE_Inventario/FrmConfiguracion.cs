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
    public partial class FrmConfiguracion : Form
    {
        FrmConfiguracion2 _frmConfiguracion2 = null;
        public FrmConfiguracion()
        {
            InitializeComponent();
        }

        private void NumerosEnteros(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

        private void FrmConfiguracion_Load(object sender, EventArgs e)
        {
            try
            {
                Datos.CargarConfiguracion();
                txt_prefijo.Text = Datos.configuracion[0];
                txt_mueble.Text = Datos.configuracion[1];
                txt_piloto.Text = Datos.configuracion[2];
                txt_id_cia.Text = Datos.configuracion[8];
                if (Datos.modo.Equals("Archivo"))
                {
                    rdb_archivo.Checked = true;
                    btn_config.Enabled = false;
                }
                else
                {
                    rdb_linea.Checked = true;
                    btn_config.Enabled = true;
                }
                this.WindowState = FormWindowState.Maximized;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
            }

        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            if (txt_prefijo.Text.Trim().Equals("") || txt_mueble.Text.Trim().Equals("") || txt_piloto.Text.Trim().Equals(""))
            {
                MessageBox.Show("Escriba todos los datos de configuración", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return;
            }
            if (int.Parse(txt_mueble.Text.Trim()) <= 0 || int.Parse(txt_piloto.Text.Trim()) <= 0)
            {
                MessageBox.Show("El valor para mueble y piloto debe ser mayor a cero", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return;
            }
            try
            {
                Datos.EscribirConfiguracion(txt_prefijo.Text.Trim().ToUpper() + "," + txt_mueble.Text.Trim() + "," + txt_piloto.Text.Trim() + "," + Datos.configuracion[3] + "," + Datos.configuracion[4] + "," + Datos.configuracion[5] + "," + Datos.configuracion[6] + "," + Datos.configuracion[7] + "," + txt_id_cia.Text.Trim());
                txt_prefijo.Text = ""; 
                txt_mueble.Text = ""; 
                txt_piloto.Text = "";
                txt_id_cia.Text = "";
                Datos.CargarConfiguracion();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
            }
        }

        private void FrmConfiguracion_Activated(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
        }

        private void txt_prefijo_GotFocus(object sender, EventArgs e)
        {
            txt_prefijo.SelectAll();
        }

        private void txt_mueble_GotFocus(object sender, EventArgs e)
        {
            txt_mueble.SelectAll();
        }

        private void txt_piloto_GotFocus(object sender, EventArgs e)
        {
            txt_piloto.SelectAll();
        }

        private void rdb_linea_CheckedChanged(object sender, EventArgs e)
        {
            if (rdb_linea.Checked)
            {
                Datos.modo = "Linea";
                btn_config.Enabled = true;
                btn_ok.Enabled = false;
            }
        }

        private void rdb_archivo_CheckedChanged(object sender, EventArgs e)
        {
            if (rdb_archivo.Checked)
            {
                Datos.modo = "Archivo";
                btn_config.Enabled = false;
                btn_ok.Enabled = true;
            }
        }

        private void btn_config_Click(object sender, EventArgs e)
        {
            _frmConfiguracion2 = new FrmConfiguracion2();
            _frmConfiguracion2.ShowDialog();
            if (_frmConfiguracion2.DialogResult.Equals(DialogResult.OK))
            {
                btn_ok.Enabled = true;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_ParentChanged(object sender, EventArgs e)
        {

        }
    }
}