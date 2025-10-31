using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace WCE_Inventario
{
    public partial class FrmEntrada : Form
    {
        FrmConteo _frmConteo = null;
        FrmConfiguracion _frmConfiguracion = null;
        public FrmEntrada()
        {
            InitializeComponent();
        }

        private void txt_bodega_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar.Equals((char)13))
            {
                txt_consecutivo.Focus();
            }
        }

        private void btn_cargar_Click(object sender, EventArgs e)
        {
            try
            {
                string archivo = "";
                _openFileDialog.Filter = "Archivo de texto (.txt)|*.txt";
                if (_openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    archivo = _openFileDialog.FileName;
                    if (Datos.CargarDatos(archivo) != null)
                    {
                        MessageBox.Show("El archivo se cargo exitosamente", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                    }
                    else
                    {
                        MessageBox.Show("No hay datos disponibles", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    }
                    //FrmListado _FrmListado = new FrmListado();
                    //_FrmListado.WindowState = FormWindowState.Maximized;
                    //_FrmListado.ShowDialog();
                    Cursor.Current = Cursors.Default;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            if (txt_bodega.Text.Equals("") || txt_consecutivo.Text.Equals(""))
            {
                MessageBox.Show("Primero llene todos los campos", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return;
            }
            
            if (Datos.modo.Equals("Archivo"))
            {
                if (Datos.tblDatos == null || Datos.tblDatos.Rows.Count == 0)
                {
                    MessageBox.Show("Primero debe cargar los datos", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    return;
                }
            }
            else
            {
                try
                {
                    Datos.RowidBodega = -1;
                    Datos.RowidInventario = -1;
                    Datos datos = new Datos();
                    Datos.RowidBodega = datos.ValidarBodega(txt_bodega.Text.Trim(),Convert.ToInt32(Datos.configuracion[8]));
                    if (Datos.RowidBodega > 0)
                    {
                        int[] info = datos.ValidarInventario(Datos.RowidBodega, Convert.ToInt32(txt_consecutivo.Text.Trim()), Convert.ToInt32(Datos.configuracion[8]));
                        if (info != null)
                        {
                            if (!info[1].Equals(1))
                            {
                                MessageBox.Show("Este inventario no esta en estado \"Cargue.\"", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                                txt_consecutivo.Focus();
                                txt_consecutivo.SelectAll();
                                return;
                            }
                            Datos.RowidInventario = info[0];
                        }
                    }
                    else
                    {
                        MessageBox.Show("El código de la bodega no es valido.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation,MessageBoxDefaultButton.Button1);
                        txt_bodega.Focus();
                        txt_bodega.SelectAll();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                    return;
                }
            }
            if (_frmConteo == null)
            {
                _frmConteo = new FrmConteo(this);
            }
            _frmConteo.Show();
            _frmConteo.WindowState = FormWindowState.Maximized;
            this.Hide();
        }

        private void FrmEntrada_Activated(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            if (Datos.modo == "Archivo")
            {
                btn_cargar.Visible = true;
            }
            else
            {
                btn_cargar.Visible = false;
            }
        }

        private void mni_salir_Click(object sender, EventArgs e)
        {
            if (Conexion.conn != null)
            {
                if (Conexion.conn.State == ConnectionState.Open)
                {
                    Conexion.conn.Close();
                }
            }
            Application.Exit();
        }

        private void mni_cargar_Click(object sender, EventArgs e)
        {
            try
            {
                string archivo = "";
                string ruta = "";
                _openFileDialog.Filter = "Archivo de texto (.txt)|*.txt";
                if (_openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    archivo = _openFileDialog.FileName;
                    ruta = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);

                    Datos.CrearConector(archivo, ruta + "\\CONECTOR_" + string.Format("{0:yyyMMdd}", DateTime.Now) + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".txt", Convert.ToInt32(Datos.configuracion[8]));
                    MessageBox.Show("Archivo conector creado exitosamente", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        private void mni_configuracion_Click(object sender, EventArgs e)
        {
            _frmConfiguracion = new FrmConfiguracion();
            _frmConfiguracion.ShowDialog();
        }

        private void FrmEntrada_Load(object sender, EventArgs e)
        {
            try
            {
                Datos.rutaArchivos = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                Datos.modo = "Archivo";
                Datos.CargarConfiguracion();
                
                this.WindowState = FormWindowState.Maximized;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }
    }
}