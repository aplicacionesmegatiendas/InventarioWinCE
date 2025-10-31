using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;
using System.Reflection;
using System.Collections.Specialized;
using System.Xml;
using System.Data.SqlClient;
namespace WCE_Inventario
{
    public class Datos
    {
        public static string[] configuracion = null;//CONFIGURACION PARA PREFIJO, MUEBLE,PILOTO Y CONEXION A LA BASE DE DATOS.
        public static string modo = "Archivo";//DETERMINA EL MODO EN COMO EL PROGRAMA OBTENDRA LA INFORMACION.

        private static int rowidBodega = -1;
        private static int rowidInventario = -1;

        public static int RowidBodega 
        {
            get 
            { 
                return rowidBodega;
            }
            set
            {
                rowidBodega = value;
            } 
        }


        public static int RowidInventario
        {
            get
            {
                return rowidInventario;
            }
            set
            {
                rowidInventario = value;
            }
        }

        #region MODO ARCHIVO
        public static DataTable tblDatos = null;//Objeto que contiene la información proveniente del archivo plano y que es cargado en el metodo CargarDatos.

        public static string rutaArchivos = "";

        public static FileStream fsConteo = null;
        public static FileStream fsNovedad = null;
        public static FileStream fsConectorConteo = null;
        public static FileStream fsConectorNovedad = null;

        public static string nombreArchivoConteo = "";
        public static string nombreArchivoNovedad = "";
        public static string nombreArchivoConectorConteo = "";
        public static string nombreArchivoConectorNovedad = "";
                
        /// <summary>
        /// Carga la información de los productos proveniente de el archivo plano.
        /// </summary>
        /// <param name="archivo">Ruta del archivo que contiene la información.</param>
        /// <returns>Devuelve un objeto de tipo DataTable que contiene la información que se cargo.</returns>
        public static DataTable CargarDatos(string archivo)
        {
            if (tblDatos == null)
            {
                tblDatos = new DataTable();
            }
            else
            {
                tblDatos.Rows.Clear();
                tblDatos.Columns.Clear();
            }

            tblDatos.Columns.Add("Barra", typeof(string));
            tblDatos.Columns.Add("Descripción", typeof(string));

            DataColumn[] dc = new DataColumn[1];
            dc[0] = tblDatos.Columns[0];
            tblDatos.PrimaryKey = dc;

            // Path.GetDirectoryName(archivo);
            try
            {
                using (StreamReader sr = new StreamReader(archivo))
                {
                    string linea;
                    while ((linea = sr.ReadLine()) != null)
                    {
                        if (linea.Trim().Length > 0)
                        {
                            string[] campos = linea.Split(','); //Corta por las comas
                            tblDatos.Rows.Add(
                            campos[0],
                            campos[1]
                            );
                        }
                    }
                }
                return tblDatos;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cargar los datos: " + ex.Message);
            }
        }

        /// <summary>
        /// Crea un archivo para escribir datos en el.
        /// </summary>
        /// <param name="archivo">Rura del archivo</param>
        public static void CrearArchivoConteo(string nombre)
        {
            if (fsConteo == null)
            {
                fsConteo = File.Create(rutaArchivos + "\\" + nombre);
            }
        }


        /// <summary>
        /// Crea un archivo para escribir datos en el.
        /// </summary>
        /// <param name="archivo">Rura del archivo</param>
        public static void CrearArchivoNovedad(string nombre)
        {
            if (fsNovedad == null)
            {
                fsNovedad = File.Create(rutaArchivos + "\\" + nombre);
            }
        }

        /// <summary>
        /// Crea el archivo conector.
        /// </summary>
        /// <param name="archivoConteo">Nombre del archivo de conteo.</param>
        /// <param name="archivoConector">Nombre del archivo conector.</param>
        public static void CrearConector(string archivoConteo, string archivoConector,int id_cia)
        {
            string REG_INICIO = "000000100000001001";
            string movimientos = "";
            int nroReg = 2;
            string F_NUMERO_REG = "";//NUMERICO DE 7 (SE RELLENA CON CEROS A LA IZQUIERDA). EMPIEZA EN 2.
            string F_TIPO_REG = "0412";
            string F_SUBTIPO_REG = "00";
            string F_VERSION_REG = "03";
            string F_CIA = id_cia.ToString().PadLeft(3,'0');
            string f410_consecutivo = "";//NUMERICO DE 8 (SE RELLENA CON CEROS A LA IZQUIERDA). (ARCHIVO: CONSECUTIVO INVENTARIO).
            string f_campo = "".PadRight(55, ' ');//ALFANUMERICO 55 (ESPACIOS).
            string f412_id_bodega = "";//ALFANUMERICO 5. (SE RELLENA CON ESPACIOS VACIOS A LA DERECHA). (ARCHIVO: BODEGA DEL FORM ENTRADA).
            string f412_id_ubicacion_aux = "".PadRight(10, ' ');//ALFANUMERICO 10 (ESPACIOS). 
            string f412_id_lote = "".PadRight(15, ' ');//ALFANUMERICO 15 (ESPACIOS).
            string f412_numero_etiqueta = "".PadLeft(9, '0');//ALFANUMERICO 9 (CEROS).
            string f412_cant_conteo1_1 = "00000000000.0000";//NUMERICO 16 11 ENTEROS PUNTO 4 DECIMALES.
            string f412_cant_conteo2_1 = "00000000000.0000";//NUMERICO 16 11 ENTEROS PUNTO 4 DECIMALES.
            string f412_cant_conteo3_1 = "";//NUMERICO 16 11 ENTEROS PUNTO 4 DECIMALES. (ARCHIVO:CANTIDAD DEL FORM PILOTO).
            string f412_cant_conteo1_2 = "00000000000.0000";//NUMERICO 16 11 ENTEROS PUNTO 4 DECIMALES.
            string f412_cant_conteo2_2 = "00000000000.0000";//NUMERICO 16 11 ENTEROS PUNTO 4 DECIMALES.
            string f412_cant_conteo3_2 = "00000000000.0000";//NUMERICO 16 11 ENTEROS PUNTO 4 DECIMALES.
            string f412_id_item = "".PadLeft(7, '0');//NUMERICO 7.
            string f412_referencia_item = "".PadRight(50, ' ');//ALFANUMERICO 50.
            string f412_codigo_barras = "";//ALFANUMERICO 20 (ARCHIVO:COD BARRAS).
            string f412_id_ext1_detalle = "".PadRight(20, ' ');//ALFANUMERICO 20 (ESPACIOS).
            string f412_id_ext2_detalle = "".PadRight(20, ' ');//ALFANUMERICO 20 (ESPACIOS).
            string f412_seccion = "";//ALFANUMERICO 50 (ARCHIVO)
            string REG_CIERRE = "";
            try
            {
                fsConectorConteo = File.Create(archivoConector);
                AddText(fsConectorConteo, REG_INICIO);
                using (StreamReader sr = new StreamReader(archivoConteo))
                {
                    string linea;
                    while ((linea = sr.ReadLine()) != null)
                    {
                        movimientos = "";

                        if (linea.Trim().Length > 0)
                        {
                            F_NUMERO_REG = nroReg.ToString().PadLeft(7, '0');
                            movimientos += F_NUMERO_REG;
                            movimientos += F_TIPO_REG;
                            movimientos += F_SUBTIPO_REG;
                            movimientos += F_VERSION_REG;
                            movimientos += F_CIA;

                            string[] campos = linea.Split(','); //Corta por las comas

                            f410_consecutivo = campos[1];
                            movimientos += f410_consecutivo.PadLeft(8, '0');

                            movimientos += f_campo;

                            f412_id_bodega = campos[0];
                            movimientos += f412_id_bodega.PadRight(5, ' ');

                            movimientos += f412_id_ubicacion_aux;

                            movimientos += f412_id_lote;

                            movimientos += f412_numero_etiqueta;

                            movimientos += f412_cant_conteo1_1;

                            movimientos += f412_cant_conteo2_1;

                            f412_cant_conteo3_1 = CantidadPrecio(campos[4]); ;
                            movimientos += f412_cant_conteo3_1;

                            movimientos += f412_cant_conteo1_2;

                            movimientos += f412_cant_conteo2_2;

                            movimientos += f412_cant_conteo3_2;

                            movimientos += f412_id_item;

                            movimientos += f412_referencia_item;

                            f412_codigo_barras = campos[3];
                            movimientos += f412_codigo_barras.PadRight(20, ' ');

                            movimientos += f412_id_ext1_detalle;

                            movimientos += f412_id_ext2_detalle;

                            f412_seccion = campos[2];
                            movimientos += f412_seccion.PadRight(50, ' ');

                            AddText(fsConectorConteo, movimientos);

                            nroReg++;
                        }
                    }
                }
                REG_CIERRE = nroReg.ToString().PadLeft(7, '0') + "99990001001";
                AddText(fsConectorConteo, REG_CIERRE);
                fsConectorConteo.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el conector: " + ex.Message);
            }
        }

        private static string CantidadPrecio(string cantidad)//Este metodo se usa para dar el formato a las cantidades y precios del conector.
        {
            int i, j;

            string partEnt = "";
            string partDec = "";
            char[] nums = cantidad.ToCharArray();

            for (i = 0; i < nums.Length; i++)
            {
                if (!nums[i].Equals('.'))
                {
                    partEnt += nums[i];
                }
                else
                {
                    break;
                }
            }

            partEnt = partEnt.PadLeft(11, '0');

            for (j = i + 1; j < nums.Length; j++)
            {
                partDec += nums[j];
            }

            partDec = partDec.PadRight(4, '0');

            return (partEnt + "." + partDec);
        }
        #endregion

        #region MODO LINEA
        /// <summary>
        /// Valida si el id de bodega existe.
        /// </summary>
        /// <param name="id">id de la bodega.</param>
        public int ValidarBodega(string id, int id_cia)
        {
            object res = null;
            int ret = -1;
            string SQL = @"SELECT 
                            f150_rowid 
                        FROM 
                            t150_mc_bodegas 
                        WHERE 
                            f150_id=@ID
                            AND f150_id_cia=@ID_CIA";
            try
            {
                SqlCommand cmd = new SqlCommand(SQL, Conexion.AbrirConexion());
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@ID_CIA", id_cia);
                res = cmd.ExecuteScalar();
                if (res != null)
                {
                    ret = Convert.ToInt32(res);
                   // RowidBodega = (int)res;//SE LE ASIGNA EL VALOR DEL rowid
                }
                Conexion.CerrarConexion();
                return ret;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al validar el id de bodega: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtiene el rowid y el estado del inventario.
        /// </summary>
        /// <param name="rowidbodega">rowid de la bodega.</param>
        /// <param name="consecutivo">Consecutivo del inventario.</param>
        /// <returns>Devuelve un array que contiene el rowid del inventario y el código del estado.</returns>
        public int[] ValidarInventario(int rowidbodega, int consecutivo, int id_cia)
        {
            int[] inv = null;
            SqlDataReader dr = null;

            string SQL = @"BEGIN TRY 
	                        IF EXISTS
                            (
                                SELECT 
                                    f410_consecutivo 
                                FROM 
                                    t410_cm_control_fisico 
                                WHERE 
                                    f410_consecutivo=@CONSECUTIVO
                                    AND f410_id_cia=@ID_CIA) 
	                        BEGIN 
		                        SELECT 
                                    f410_rowid, 
                                    f410_ind_estado 
                                FROM 
                                    t410_cm_control_fisico 
                                WHERE 
                                    f410_rowid_bodega=@ID_BODEGA 
                                    AND f410_consecutivo=@CONSECUTIVO 
                                    AND f410_id_cia=@ID_CIA
	                        END 
	                        ELSE 
		                        BEGIN 
			                        RAISERROR ('Consecutivo de inventario no existe.', 16, 1); 
		                        END 
                        END TRY 
                        BEGIN CATCH 
	                        DECLARE @ErrorMessage NVARCHAR(4000); 
                            DECLARE @ErrorSeverity INT; 
                            DECLARE @ErrorState INT; 

                            SELECT @ErrorMessage = ERROR_MESSAGE(), 
                                   @ErrorSeverity = ERROR_SEVERITY(), 
                                   @ErrorState = ERROR_STATE(); 
                           
                            RAISERROR (@ErrorMessage, 
                                       @ErrorSeverity,  
                                       @ErrorState  
                                       ); 
                        END CATCH";
            try
            {
                SqlCommand cmd = new SqlCommand(SQL, Conexion.AbrirConexion());
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ID_BODEGA", rowidbodega);
                cmd.Parameters.AddWithValue("@CONSECUTIVO", consecutivo);
                cmd.Parameters.AddWithValue("@ID_CIA", id_cia);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    inv = new int[2];
                    inv[0] = dr.GetInt32(0);
                    inv[1] = dr.GetInt16(1);
                }
                dr.Close();
                Conexion.CerrarConexion();
                return inv;
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene el rowid y la descripción del producto.
        /// </summary>
        /// <param name="barra">Código de barra del producto.</param>
        /// <returns></returns>
        public string[] ObtenerDatosItem(string barra, int id_cia)
        {
            SqlDataReader dr = null;
            string[] datos = null;
            
            string SQL = @"DECLARE @rowid_item_ext int;
                        declare @rowid_item INT; 
                         SELECT 
                             @rowid_item_ext = f121_rowid, 
                             @rowid_item=f120_rowid 
                         FROM 
                             t131_mc_items_barras 
                             INNER JOIN t121_mc_items_extensiones ON f121_rowid=f131_rowid_item_ext AND f121_id_cia=f131_id_cia 
                             inner join t120_mc_items on f120_id_cia=f121_id_cia and f120_rowid = f121_rowid_item 
                         WHERE 
                             f131_id=@BARRA AND f131_id_cia=1;
                         SELECT 
	                        @rowid_item_ext AS f131_rowid_item_ext,
	                        f120_descripcion 
                        FROM t120_mc_items 
                        WHERE f120_rowid=@rowid_item AND f120_id_cia=@ID_CIA";
            try
            {
                SqlCommand cmd = new SqlCommand(SQL, Conexion.AbrirConexion());
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@BARRA", barra);
                cmd.Parameters.AddWithValue("@ID_CIA", id_cia);
                dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    datos=new string[2];
                    datos[0] = dr.GetInt32(0).ToString();
                    datos[1] = dr.GetString(1);
                }
                dr.Close();
                Conexion.CerrarConexion();
                return datos;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener la descripción del producto: " + ex.Message);
            }
        }

        /// <summary>
        /// Valida si el item esta en estado preparado.
        /// </summary>
        /// <param name="rowidinventario">rowid del inventario.</param>
        /// <param name="rowiditem">rowid del item.</param>
        /// <param name="rowidbodega">rowid de la bodega.</param>
        /// <returns></returns>
        public int ValidarItemPreparado(int rowidinventario,int rowiditem,  int rowidbodega)
        {
            int res = -1;
            string SQL = @"DECLARE @rowid_item_ctrl INT; 
                         SET @rowid_item_ctrl = 0; 
                         EXEC sp_inv_fis_exist_item @rowid_item_ctrl OUTPUT,@INVENTARIO,@ITEM,@BODEGA,null,null; 
                         SELECT @rowid_item_ctrl AS Preparado";
            try
            {
                SqlCommand cmd = new SqlCommand(SQL, Conexion.AbrirConexion());
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@INVENTARIO", rowidinventario);
                cmd.Parameters.AddWithValue("@ITEM", rowiditem);
                cmd.Parameters.AddWithValue("@BODEGA", rowidbodega);

                res = Convert.ToInt32(cmd.ExecuteScalar());
                Conexion.CerrarConexion();
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al validar el estado de preparado del producto: " + ex.Message);
            }
        }

        /// <summary>
        /// Guarda la información del conteo en la base de datos.
        /// </summary>
        /// <param name="rowiditem">RowId de la barra.</param>
        /// <param name="seccion">Compuesto por: Prefijo, Mueble, Piloto y Auditor.</param>
        /// <param name="rowidctrlfisico">RowId del inventario.</param>
        /// <param name="rowiditemctrl">RowId del producto cuando esta preparado.</param>
        /// <param name="rowidbodega">RowId de la bodega.</param>
        /// <param name="cantidad">Cantidad</param>
        /// <param name="usuario">Nombre de usuario unoee</param>
        public void GuardarConteo(int rowidctrlfisico, int rowiditemctrl, int rowiditem, int rowidbodega, float cantidad, string usuario, string seccion, int id_cia)
        {
            string SQL = @"DECLARE @rowid_mov_fis INT = NULL; 
                         SET @rowid_mov_fis=NULL; 
                         SELECT 
                            @rowid_mov_fis = MAX(f412_rowid) 
                         FROM 
                            t412_cm_control_fisico_movto 
                         WHERE 
                            f412_rowid_cntrl_fisico = @ROWID_CTRL_FISICO 
                            AND f412_seccion = @SECCION
                            AND f412_id_cia=@ID_CIA;
                         IF @rowid_mov_fis IS NULL 
                         BEGIN 
                            EXEC sp_inv_fis_movto_adicionar @ID_CIA, 0, @ROWID_CTRL_FISICO, @ROWID_ITEM_CTRL, @ROWID_ITEM, @ROWID_BODEGA, NULL, NULL, 0, 3, @CANTIDAD, 0, @USUARIO, 1, @SECCION;
                         END 
                         ELSE 
                         BEGIN 
                            DECLARE @MSJ AS NVARCHAR(50)
                            SET @MSJ = 'La sección ' +  @SECCION + ' ya esta registrada.' 
                            RAISERROR (@MSJ, 16, 1); 
                         END";
            try
            {
                SqlCommand cmd = new SqlCommand(SQL, Conexion.AbrirConexion());
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ROWID_CTRL_FISICO", rowidctrlfisico);
                cmd.Parameters.AddWithValue("@ROWID_ITEM_CTRL", rowiditemctrl);
                cmd.Parameters.AddWithValue("@ROWID_ITEM",rowiditem);
                cmd.Parameters.AddWithValue("@ROWID_BODEGA", rowidbodega);
                cmd.Parameters.AddWithValue("@CANTIDAD", cantidad);
                cmd.Parameters.AddWithValue("@USUARIO", usuario);
                cmd.Parameters.AddWithValue("@SECCION", seccion);
                cmd.Parameters.AddWithValue("@ID_CIA", id_cia);
                cmd.ExecuteNonQuery();
                Conexion.CerrarConexion();
            }
            catch (SqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// Escribe datos en un archivo.
        /// </summary>
        /// <param name="fs">Archivo donde se escribe.</param>
        /// <param name="value">Datos que se escriben en el archivo.</param>
        public static void AddText(FileStream fs, string value)
        {
            string Resultado = string.Format("{0}{1}", value, Environment.NewLine);

            byte[] info = new UTF8Encoding(true).GetBytes(Resultado);

            fs.Write(info, 0, info.Length);
        }

        /// <summary>
        /// Carga la configuracion predeterminada para los parametros del conteo y del modo de conexión.
        /// </summary>
        public static void CargarConfiguracion()
        {
            FileStream fsConfiguracion = null;
            try
            {
                if (!File.Exists(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\CONFIGURACION.txt"))
                {
                    using (fsConfiguracion = File.Create(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\CONFIGURACION.txt"))
                    {
                        AddText(fsConfiguracion, "M,3,3,[servidor],[usuario],M3g@t13nd@s_2013R1,[basedatos],[usuariounoee],1");
                    }
                }
                using (StreamReader sr = new StreamReader(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\CONFIGURACION.txt"))
                {
                    configuracion = null;
                    string linea;
                    while ((linea = sr.ReadLine()) != null)
                    {
                        if (linea.Trim().Length > 0)
                        {
                            configuracion = linea.Split(','); //Corta por las comas
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cargar la configuración: " + ex.Message);
            }
        }
        
        /// <summary>
        /// Escribe los nuevo valores de configuración en el archivo.
        /// </summary>
        /// <param name="parametros">Los datos que se usan para la configuración.</param>
        public static void EscribirConfiguracion(string parametros)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase) + "\\CONFIGURACION.txt"))
                {
                    sw.WriteLine(parametros);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al escribir la configuración: " + ex.Message);
            }
        }
    }
}
