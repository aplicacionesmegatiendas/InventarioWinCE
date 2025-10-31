using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Globalization;

namespace WCE_Inventario
{
    public partial class FrmConteo : Form
    {
        private int rowidItem_ext = -1;//Guarda el rowid del item.
        private int rowidItemControl = -1;

        FrmEntrada frm;

        string separador = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;//Esta variable contiene el caracter que el sistema operativo usa para la separación de cifras decimales.

        bool conteo = false;
        bool novedad = false;

        public FrmConteo(FrmEntrada frm)
        {
            InitializeComponent();
            this.frm = frm;
        }

        private string Buscar(string barra)
        {
            DataRow dr = null;
            string descripcion = "";
            try
            {
                dr = Datos.tblDatos.Rows.Find(barra);
                if (dr == null)
                {
                    MessageBox.Show("El código no existe", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                    txt_cod_barras.Focus();
                    txt_cod_barras.SelectAll();
                    conteo = false;
                    novedad = true;
                }
                else
                {
                    descripcion = Convert.ToString(dr[1]);
                    conteo = true;
                    novedad = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
            }
            return descripcion;
        }

        private void NumerosEnteros(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

        private void btn_atras_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.frm.Show();
        }

        private void txt_cod_barras_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                try
                {
                    if (Datos.modo.Equals("Archivo"))
                    {
                        string descrip = Buscar(txt_cod_barras.Text);
                        if (!descrip.Equals(""))
                        {
                            txt_descripcion.Text = descrip;
                            txt_cantidad.Focus();
                        }
                    }
                    else
                    {
                        //AQUI VA LO DE LA BASE DA DATOS
                        rowidItem_ext = -1;
                        rowidItemControl = -1;
                        txt_descripcion.Text = "";

                        Datos datos = new Datos();
                        string[] info = datos.ObtenerDatosItem(txt_cod_barras.Text.Trim(),Convert.ToInt32(Datos.configuracion[8]));

                        if (info == null)
                        {
                            MessageBox.Show("El producto no existe.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                            txt_cod_barras.Focus();
                            txt_cod_barras.SelectAll();
                            return;
                        }
                        rowidItem_ext = Convert.ToInt32(info[0]);
                        txt_descripcion.Text = info[1];

                        //VALIDAR QUE EL PRODUCTO ESTE PREPARADO.
                        btn_ok.Enabled = false;
                        rowidItemControl = datos.ValidarItemPreparado(Datos.RowidInventario, rowidItem_ext, Datos.RowidBodega);
                        if (rowidItemControl > 0)
                        {
                            btn_ok.Enabled = true;
                            txt_cantidad.Focus();
                        }
                        else
                        {
                            MessageBox.Show("El producto no esta preparado", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                            txt_cod_barras.Focus();
                            txt_cod_barras.SelectAll();
                            return;
                        }
                        //VALIDAR INVENTARIO.
                        int[] info2 = datos.ValidarInventario(Datos.RowidBodega, Convert.ToInt32(frm.txt_consecutivo.Text.Trim()), Convert.ToInt32(Datos.configuracion[8]));
                        if (info2 != null)
                        {
                            if (!info2[1].Equals(1))
                            {
                                MessageBox.Show("El inventario no esta en estado \"Cargue.\"", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                                btn_ok.Enabled = false;
                                return;
                            }
                            Datos.RowidInventario = info2[0];
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                }
            }
        }

        private void txt_auditor_TextChanged(object sender, EventArgs e)
        {
            if (txt_auditor.Text.Trim().Equals(""))
            {
                txt_mueble.Text = "";
                txt_piloto.Text = "";
            }
            else
            {
                txt_piloto.Text = "1";
            }
            txt_cod_barras.Text = "";
            txt_descripcion.Text = "";
            txt_cantidad.Text = "";
        }

        private void txt_mueble_TextChanged(object sender, EventArgs e)
        {
            txt_piloto.Text = "1";
            txt_cod_barras.Text = "";
            txt_descripcion.Text = "";
            txt_cantidad.Text = "";
        }

        private void txt_piloto_TextChanged(object sender, EventArgs e)
        {
            txt_cod_barras.Text = "";
            txt_descripcion.Text = "";
            txt_cantidad.Text = "";
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            if (txt_cantidad.Text == "" || txt_auditor.Text == "" || txt_mueble.Text == "" || txt_piloto.Text == "" || txt_cod_barras.Text == "")
            {
                MessageBox.Show("Escriba todos los campos", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                return;
            }
            try
            {
                if (Datos.modo.Equals("Archivo"))
                {
                    Datos.rutaArchivos = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    if (Datos.nombreArchivoConteo.Equals(""))
                    {
                        Datos.nombreArchivoConteo = string.Format("{0:yyyMMdd}", DateTime.Now) + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".txt";
                        Datos.nombreArchivoConectorConteo = Datos.nombreArchivoConteo;
                    }

                    if (Datos.nombreArchivoNovedad.Equals(""))
                    {
                        Datos.nombreArchivoNovedad = string.Format("{0:yyyMMdd}", DateTime.Now) + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".txt";
                        Datos.nombreArchivoConectorNovedad = Datos.nombreArchivoNovedad;
                    }

                    if (conteo == true)
                    {
                        Datos.CrearArchivoConteo("CONTEO_" + Datos.nombreArchivoConteo);
                        Datos.AddText(Datos.fsConteo, frm.txt_bodega.Text.Trim() + "," + frm.txt_consecutivo.Text.Trim() + "," + Datos.configuracion[0] + "-" + txt_mueble.Text.Trim().PadLeft(int.Parse(Datos.configuracion[1]), '0') + "-" + txt_piloto.Text.Trim().PadLeft(int.Parse(Datos.configuracion[2]), '0') + "-" + txt_auditor.Text.Trim().ToUpper() + "," + txt_cod_barras.Text.Trim() + "," + txt_cantidad.Text.Trim().Replace(',','.') + "," + txt_descripcion.Text.Trim());
                    }

                    if (novedad == true)
                    {
                        Datos.CrearArchivoNovedad("NOVEDAD_" + Datos.nombreArchivoNovedad);
                        Datos.AddText(Datos.fsNovedad, frm.txt_bodega.Text.Trim() + "," + frm.txt_consecutivo.Text.Trim() + "," + Datos.configuracion[0] + "-" + txt_mueble.Text.Trim().PadLeft(int.Parse(Datos.configuracion[1]), '0') + "-" + txt_piloto.Text.Trim().PadLeft(int.Parse(Datos.configuracion[2]), '0') + "-" + txt_auditor.Text.Trim().ToUpper() + "," + txt_cod_barras.Text.Trim() + "," + txt_cantidad.Text.Trim().Replace(',','.') + "," + txt_descripcion.Text.Trim());
                    }
                    txt_piloto.Text = (Convert.ToInt32(txt_piloto.Text) + 1).ToString();
                    txt_cod_barras.Text = "";
                    txt_descripcion.Text = "";
                    txt_cantidad.Text = "";
                    txt_cod_barras.Focus();

                    btn_finalizar.Enabled = true;
                    conteo = false;
                    novedad = false;
                }
                else
                {
                    Datos datos = new Datos();

                    int[] info2 = datos.ValidarInventario(Datos.RowidBodega, Convert.ToInt32(frm.txt_consecutivo.Text.Trim()), Convert.ToInt32(Datos.configuracion[8]));
                    if (info2 != null)
                    {
                        if (!info2[1].Equals(1))
                        {
                            MessageBox.Show("El inventario no esta en estado \"Cargue.\"", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                            btn_ok.Enabled = false;
                            return;
                        }
                        Datos.RowidInventario = info2[0];
                    }

                     datos.GuardarConteo(Datos.RowidInventario, this.rowidItemControl, this.rowidItem_ext, Datos.RowidBodega, Convert.ToSingle(txt_cantidad.Text),
                        Datos.configuracion[7], Datos.configuracion[0] + "-" + txt_mueble.Text.Trim().PadLeft(int.Parse(Datos.configuracion[1]), '0') + "-" + txt_piloto.Text.Trim().PadLeft(int.Parse(Datos.configuracion[2]), '0') + "-" + txt_auditor.Text.Trim().ToUpper(), Convert.ToInt32(Datos.configuracion[8]));

                    btn_ok.Enabled = false;

                    txt_piloto.Text = (Convert.ToInt32(txt_piloto.Text) + 1).ToString();
                    txt_cod_barras.Text = "";
                    txt_descripcion.Text = "";
                    txt_cantidad.Text = "";
                    txt_cod_barras.Focus();
                    btn_finalizar.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
            }
        }

        private void btn_finalizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (Datos.modo.Equals("Archivo"))
                {
                    if (Datos.fsConteo != null)
                    {
                        Datos.fsConteo.Close();
                        Datos.fsConteo = null;
                        Datos.CrearConector(Datos.rutaArchivos + "\\CONTEO_" + Datos.nombreArchivoConteo, Datos.rutaArchivos + "\\C_CONECTOR_" + Datos.nombreArchivoConectorConteo, Convert.ToInt32(Datos.configuracion[8]));
                    }

                    if (Datos.fsNovedad != null)
                    {
                        Datos.fsNovedad.Close();
                        Datos.fsNovedad = null;
                        Datos.CrearConector(Datos.rutaArchivos + "\\NOVEDAD_" + Datos.nombreArchivoNovedad, Datos.rutaArchivos + "\\N_CONECTOR_" + Datos.nombreArchivoConectorNovedad, Convert.ToInt32(Datos.configuracion[8]));
                    }

                    Datos.nombreArchivoConteo = "";
                    Datos.nombreArchivoConectorConteo = "";
                    Datos.nombreArchivoNovedad = "";
                    Datos.nombreArchivoConectorNovedad = "";
                    conteo = false;
                    novedad = false;

                    MessageBox.Show("Archivo conector creado exitosamente", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);

                    txt_auditor.Text = "";
                    txt_mueble.Text = "";
                    txt_piloto.Text = "";
                    txt_cod_barras.Text = "";
                    txt_descripcion.Text = "";
                    txt_cantidad.Text = "";
                }
                else
                {
                    if (Conexion.conn != null)
                    {
                        if (Conexion.conn.State == ConnectionState.Open)
                        {
                            Conexion.conn.Close();
                        }
                    }
                }

                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
            }
        }

        private void FrmPiloto_Closing(object sender, CancelEventArgs e)
        {
            Application.Exit();
        }

        private void txt_auditor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetterOrDigit(e.KeyChar) || char.IsControl(e.KeyChar)))
            {
                e.Handled = true;
            }
        }

        private void FrmConteo_Activated(object sender, EventArgs e)
        {
            txt_mueble.MaxLength = int.Parse(Datos.configuracion[1]);
            txt_piloto.MaxLength = int.Parse(Datos.configuracion[2]);
            this.WindowState = FormWindowState.Maximized;
        }

        private void txt_cantidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsDigit(e.KeyChar) || char.IsControl(e.KeyChar) || char.IsPunctuation(e.KeyChar)))
            {
                e.Handled = true;
            }
            if (separador.Equals(","))
            {
                if (e.KeyChar.Equals('.'))
                    e.Handled = true;
            }
            if (separador.Equals("."))
            {
                if (e.KeyChar.Equals(','))
                    e.Handled = true;
            }

            if (e.KeyChar == (char)(Keys.Enter))
            {
                if (txt_cantidad.Text == "" || txt_auditor.Text == "" || txt_mueble.Text == "" || txt_piloto.Text == "" || txt_cod_barras.Text == "")
                {
                    MessageBox.Show("Escriba todos los campos", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                    return;
                }
                try
                {
                    if (Datos.modo.Equals("Archivo"))
                    {
                        Datos.rutaArchivos = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                        if (Datos.nombreArchivoConteo.Equals(""))
                        {
                            Datos.nombreArchivoConteo = string.Format("{0:yyyMMdd}", DateTime.Now) + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".txt";
                            Datos.nombreArchivoConectorConteo = Datos.nombreArchivoConteo;
                        }

                        if (Datos.nombreArchivoNovedad.Equals(""))
                        {
                            Datos.nombreArchivoNovedad = string.Format("{0:yyyMMdd}", DateTime.Now) + "_" + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + ".txt";
                            Datos.nombreArchivoConectorNovedad = Datos.nombreArchivoNovedad;
                        }

                        if (conteo == true)
                        {
                            Datos.CrearArchivoConteo("CONTEO_" + Datos.nombreArchivoConteo);
                            Datos.AddText(Datos.fsConteo, frm.txt_bodega.Text.Trim() + "," + frm.txt_consecutivo.Text.Trim() + "," + Datos.configuracion[0] + "-" + txt_mueble.Text.Trim().PadLeft(int.Parse(Datos.configuracion[1]), '0') + "-" + txt_piloto.Text.Trim().PadLeft(int.Parse(Datos.configuracion[2]), '0') + "-" + txt_auditor.Text.Trim().ToUpper() + "," + txt_cod_barras.Text.Trim() + "," + txt_cantidad.Text.Trim().Replace(',','.') + "," + txt_descripcion.Text.Trim());
                        }

                        if (novedad == true)
                        {
                            Datos.CrearArchivoNovedad("NOVEDAD_" + Datos.nombreArchivoNovedad);
                            Datos.AddText(Datos.fsNovedad, frm.txt_bodega.Text.Trim() + "," + frm.txt_consecutivo.Text.Trim() + "," + Datos.configuracion[0] + "-" + txt_mueble.Text.Trim().PadLeft(int.Parse(Datos.configuracion[1]), '0') + "-" + txt_piloto.Text.Trim().PadLeft(int.Parse(Datos.configuracion[2]), '0') + "-" + txt_auditor.Text.Trim().ToUpper() + "," + txt_cod_barras.Text.Trim() + "," + txt_cantidad.Text.Trim().Replace(',', '.') + "," + txt_descripcion.Text.Trim());
                        }

                        txt_piloto.Text = (Convert.ToInt32(txt_piloto.Text) + 1).ToString();
                        txt_cod_barras.Text = "";
                        txt_descripcion.Text = "";
                        txt_cantidad.Text = "";
                        txt_cod_barras.Focus();

                        btn_finalizar.Enabled = true;
                        conteo = false;
                        novedad = false;
                    }
                    else
                    {
                        Datos datos = new Datos();

                        //VALIDAR INVENTARIO.
                        int[] info2 = datos.ValidarInventario(Datos.RowidBodega, Convert.ToInt32(frm.txt_consecutivo.Text.Trim()), Convert.ToInt32(Datos.configuracion[8]));
                        if (info2 != null)
                        {
                            if (!info2[1].Equals(1))
                            {
                                MessageBox.Show("El inventario no esta en estado \"Cargue.\"", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                                btn_ok.Enabled = false;
                                return;
                            }
                            Datos.RowidInventario = info2[0];
                        }

                        datos.GuardarConteo(Datos.RowidInventario, this.rowidItemControl, this.rowidItem_ext, Datos.RowidBodega, Convert.ToSingle(txt_cantidad.Text),
                            Datos.configuracion[7], Datos.configuracion[0] + "-" + txt_mueble.Text.Trim().PadLeft(int.Parse(Datos.configuracion[1]), '0') + "-" + txt_piloto.Text.Trim().PadLeft(int.Parse(Datos.configuracion[2]), '0') + "-" + txt_auditor.Text.Trim().ToUpper(), Convert.ToInt32(Datos.configuracion[8]));

                        btn_ok.Enabled = false;

                        txt_piloto.Text = (Convert.ToInt32(txt_piloto.Text) + 1).ToString();
                        txt_cod_barras.Text = "";
                        txt_descripcion.Text = "";
                        txt_cantidad.Text = "";
                        txt_cod_barras.Focus();
                        btn_finalizar.Enabled = true;
                        //MessageBox.Show("Conteo ingresado correctamente", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk,MessageBoxDefaultButton.Button1);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1);
                }

                // txt_piloto.Text = (Convert.ToInt32(txt_piloto.Text) + 1).ToString();
                //txt_cod_barras.Text = "";
                //txt_descripcion.Text = "";
                //txt_cantidad.Text = "";
                //txt_cod_barras.Focus();
            }
        }

        private void FrmConteo_Load(object sender, EventArgs e)
        {
            Datos.CargarConfiguracion();
            this.WindowState = FormWindowState.Maximized;
        }
    }
}