using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace pryMacia_Iefi1._2
{
    internal class clsConexionBD
    {
        public string cadena = "Server = localhost\\SQLEXPRESS;Database=Iefi;Trusted_Connection=True;";
        //private string cadena2 = "Server=localhost;Database=Iefi;Trusted_Connection=True;"; 

        public void ConectarBD()
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadena))
                {
                    conexion.Open();
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("Error en la conexión a la base de datos: " + error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void ListarBD(DataGridView grilla)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadena))
                {
                    conexion.Open();

                    // Usamos alias para que coincidan con los nombres de columna en el DataGridView
                    string query = @"SELECT 
                                Id, 
                                Nombre AS NombreUsuario, 
                                Correo, 
                                Direccion, 
                                Estado, 
                                RolId 
                             FROM Usuarios";

                    SqlCommand comando = new SqlCommand(query, conexion);
                    SqlDataAdapter adaptador = new SqlDataAdapter(comando);

                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);
                    grilla.DataSource = tabla;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("No se pudieron cargar los usuarios correctamente. Revise su conexión o intente más tarde.", "Error de carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void BuscarporNombre(DataGridView Grilla, string nombreUsuario)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadena))
                {
                    conexion.Open();
                    string query = @"SELECT 
                                Id, 
                                Nombre AS NombreUsuario, 
                                Correo, 
                                Direccion, 
                                Estado, 
                                RolId 
                             FROM Usuarios 
                             WHERE Nombre LIKE @nombre";

                    SqlCommand comando = new SqlCommand(query, conexion);
                    comando.Parameters.AddWithValue("@nombre", "%" + nombreUsuario + "%");

                    SqlDataAdapter adaptador = new SqlDataAdapter(comando);
                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    if (tabla.Rows.Count == 0)
                    {
                        MessageBox.Show("No se encontró el usuario", "Resultado de búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        Grilla.DataSource = tabla;
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("Error al buscar el usuario: " + error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void RegistrarUsuario(clsUsuario nuevoUsuario)
        {
            SqlConnection conexion = null;

            try
            {
                // Verificar si el nombre de usuario ya existe
                if (UsuarioExiste(nuevoUsuario.Nombre, 0))
                {
                    MessageBox.Show("Ya existe un usuario con ese nombre.", "Usuario duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                conexion = new SqlConnection(cadena);
                conexion.Open();

                string sql = @"INSERT INTO Usuarios (Nombre, clave, Correo, Direccion, Estado, RolId) 
                       VALUES (@Nombre, @Clave, @Email, @Direccion, @Estado, @Rol)";

                using (SqlCommand cmd = new SqlCommand(sql, conexion))
                {
                    cmd.Parameters.AddWithValue("@Nombre", nuevoUsuario.Nombre);
                    cmd.Parameters.AddWithValue("@Clave", nuevoUsuario.clave);
                    cmd.Parameters.AddWithValue("@Email", nuevoUsuario.Correo);
                    cmd.Parameters.AddWithValue("@Direccion", nuevoUsuario.Direccion);
                    cmd.Parameters.AddWithValue("@Estado", nuevoUsuario.Estado);
                    cmd.Parameters.AddWithValue("@Rol", 2); // Siempre rol de usuario al registrar

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Registro completado correctamente.", "Confirmación", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("No se pudo registrar el usuario. Detalle: " + ex.Message, "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se produjo un error inesperado: " + ex.Message, "Error General", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conexion != null && conexion.State == System.Data.ConnectionState.Open)
                    conexion.Close();
            }
        }
        public void ModificarUsuario(clsUsuario usuario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(cadena))
                {
                    conn.Open();

                    string sql = @"UPDATE Usuarios 
                           SET Nombre = @Nombre,
                               clave = @Clave,
                               Correo = @Correo,
                               
                               Direccion = @Direccion,
                               Estado = @Estado,
                               RolId = @RolId
                           WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {

                        cmd.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                        cmd.Parameters.AddWithValue("@Clave", usuario.clave);
                        cmd.Parameters.AddWithValue("@Correo", usuario.Correo);
                        cmd.Parameters.AddWithValue("@Direccion", usuario.Direccion);
                        cmd.Parameters.AddWithValue("@Estado", usuario.Estado);
                        cmd.Parameters.AddWithValue("@RolId", usuario.RolId);
                        cmd.Parameters.AddWithValue("@Id", usuario.Id);


                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Datos del usuario actualizados con éxito.", "Modificación exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al actualizar el usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void EliminarUsuario(int id)
        {
            using (SqlConnection conexion = new SqlConnection(cadena))
            {
                conexion.Open();
                string query = "DELETE FROM Usuarios WHERE Id = @id";
                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }
        public void DesactivarUsuario(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(cadena))
                {
                    conn.Open();
                    string sql = "UPDATE Usuarios SET Estado = 0 WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Usuario desactivado correctamente.", "Estado actualizado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al desactivar el usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public bool UsuarioExiste(string nombreUsuario, int idExcluir)
        {
            using (SqlConnection conexion = new SqlConnection(cadena))
            {
                conexion.Open();
                string query = "SELECT COUNT(*) FROM Usuarios WHERE Nombre = @nombre AND Id != @id";
                SqlCommand cmd = new SqlCommand(query, conexion);
                cmd.Parameters.AddWithValue("@nombre", nombreUsuario);
                cmd.Parameters.AddWithValue("@id", idExcluir);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        public void ActivarUsuario(int id)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(cadena))
                {
                    conn.Open();
                    string sql = "UPDATE Usuarios SET Estado = 1 WHERE Id = @Id";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Usuario reactivado correctamente.", "Activación exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al activar el usuario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public List<clsUsuario> ObtenerUsuariosActivos()
        {
            List<clsUsuario> lista = new List<clsUsuario>();

            try
            {
                using (SqlConnection conn = new SqlConnection(cadena))
                {
                    conn.Open();
                    string sql = "SELECT * FROM Usuarios WHERE Estado = 1";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            clsUsuario usuario = new clsUsuario
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                clave = reader["clave"].ToString(),
                                Correo = reader["Correo"].ToString(),
                                Direccion = reader["Direccion"].ToString(),
                                Estado = Convert.ToBoolean(reader["Estado"]),
                                RolId = Convert.ToInt32(reader["RolId"])
                            };
                            lista.Add(usuario);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener usuarios: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return lista;
        }
        public void GuardarSesion(clsRegistro registros)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadena))
                {
                    conexion.Open();
                    string query = "INSERT INTO Sesiones (IdUsuario, FechaInicio, HoraInicio, HoraFin, TiempoTranscurrido) VALUES (@idUsuario, @fechaInicio, @horaInicio, @horaFin, @tiempoTranscurrido)";


                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@idUsuario", registros.IdUsuario);
                        comando.Parameters.AddWithValue("@fechaInicio", registros.FechaInicio);
                        comando.Parameters.AddWithValue("@horaInicio", registros.HoraInicio.TimeOfDay);
                        comando.Parameters.AddWithValue("@horaFin", registros.HoraFin.TimeOfDay);
                        comando.Parameters.AddWithValue("@tiempoTranscurrido", registros.TiempoTranscurrido);

                        comando.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("Error al guardar Sesión: " + error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void GuardarSesionAutomatica(int idUsuario, DateTime horaInicio, DateTime horaFin)
        {
            TimeSpan tiempoTranscurrido = horaFin - horaInicio;

            clsRegistro nuevaSesion = new clsRegistro
            {
                IdUsuario = idUsuario,
                FechaInicio = DateTime.Today,
                HoraInicio = horaInicio,
                HoraFin = horaFin,
                TiempoTranscurrido = tiempoTranscurrido
            };

            GuardarSesion(nuevaSesion); // Usa el método que ya hiciste
        }
        public int ObtenerIdUsuarioPorNombre(string nombreUsuario)
        {
            int id = 0;

            try
            {
                using (SqlConnection conexion = new SqlConnection(cadena))
                {
                    conexion.Open();
                    string query = "SELECT Id FROM Usuarios WHERE Nombre = @NombreUsuario";
                    using (SqlCommand comando = new SqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);


                        var res = comando.ExecuteScalar(); //busca en la BD

                        if (res != null)
                        {
                            id = Convert.ToInt32(res);
                        }
                    }
                }
            }
            catch (Exception error)
            {
                MessageBox.Show("Error al obtener IdUsuario: " + error.Message);
            }
            return id;
        }



        //Listar Sesiones
        public void ListarSesiones(DataGridView Grilla)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadena))
                {
                    conexion.Open();

                    string query = "SELECT u.Nombre, s.FechaInicio, s.HoraInicio, s.HoraFin, s.TiempoTranscurrido FROM Sesiones s INNER JOIN Usuarios u ON s.IdUsuario = u.Id ORDER BY s.FechaInicio DESC, s.HoraInicio DESC;";


                    SqlCommand comando = new SqlCommand(query, conexion);
                    SqlDataAdapter adaptador = new SqlDataAdapter(comando);

                    DataTable tabla = new DataTable();
                    adaptador.Fill(tabla);

                    Grilla.DataSource = tabla;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show($"No se pudieron cargar las sesiones correctamente. Revise su conexión o intente más tarde. Detalles del error: {error.Message}", "Error de carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

}
