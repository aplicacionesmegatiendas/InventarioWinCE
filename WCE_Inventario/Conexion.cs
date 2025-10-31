using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
namespace WCE_Inventario
{
    public class Conexion
    {
        public static SqlConnection conn = null;

        /// <summary>
        /// Hace una prueba de conexión a la base de datos con los parametros que se sumistran y comprueba la existencia del usuario unoee
        /// </summary>
        /// <param name="dataSource">Nombre o IP del servidor.</param>
        /// <param name="initialCatalog">Nombre de la base de datos.</param>
        /// <param name="userId">Nombre de usuario.</param>
        /// <param name="pwd">Contraseña</param>
        /// <param name="usuariounoee">Nombre de usuario unoee.</param>
        /// <returns></returns>
        public static bool ProbarConexion(string dataSource, string initialCatalog, string userId, string pwd, string usuariounoee)
        {
            int c = -1;
            bool r = false;
            try
            {
                using (conn = new SqlConnection("Data Source=" + dataSource + ";Initial Catalog=" + initialCatalog + ";User ID=" + userId + ";Pwd=" + pwd))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(f552_nombre) FROM t552_ss_usuarios where f552_nombre =@NOMB", conn))
                    {
                        cmd.Parameters.AddWithValue("@NOMB", usuariounoee);
                        c = (int) cmd.ExecuteScalar();
                        if (c <= 0)
                        {
                            throw new Exception("El usuario unoee no existe");
                        }
                    }
                }
                conn.Close();
                conn.Dispose();
                conn = null;
                r = true;
                return r;
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al realizar la prueba de conexión: " + ex.Message);
            }
        }

        /// <summary>
        /// Establece conexión con un servidor SQL SERVER.
        /// </summary>
        /// <returns>Devuelve una conexión abierta.</returns>
        public static SqlConnection AbrirConexion()
        {
            string CadenaConexion = "Data Source=" + Datos.configuracion[3] + ";Initial Catalog=" + Datos.configuracion[6] + ";User ID=" + Datos.configuracion[4] + ";Pwd=" + Datos.configuracion[5];
            try
            {
               // if (conn == null)
               // {
                    conn = new SqlConnection(CadenaConexion);
                    conn.Open();
               // }
                //else
                //{
                //    if (conn.State != ConnectionState.Open)
                //    {
                //        conn.Open();
                //    }
                //}
                return conn;
            }
            catch (SqlException ex)
            {
                throw new Exception("Error al establecer la conexión con el servidor: " + ex.Message);
            }
        }

        public static void CerrarConexion()
        {
            if (conn.State.Equals(ConnectionState.Open))
            {
                conn.Close();
            }
        }
    }
}
