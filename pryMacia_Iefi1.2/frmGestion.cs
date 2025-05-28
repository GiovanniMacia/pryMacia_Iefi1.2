using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pryMacia_Iefi1._2
{
    public partial class frmGestion : MaterialForm
    {
        clsConexionBD conexion = new clsConexionBD();
        int? usuarioSeleccionado = null;
        public frmGestion()
        {
            InitializeComponent();
        }

        private void frmGestion_Load(object sender, EventArgs e)
        {

        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string nombreBuscar = txtBuscar.Text.Trim();
            if (string.IsNullOrEmpty(nombreBuscar))
            {
                MessageBox.Show("Por favor ingresá un nombre para buscar.");
                return;
            }

            conexion.BuscarporNombre(dgvGestion, nombreBuscar);
        }

        private void btnRestablecer_Click(object sender, EventArgs e)
        {
            conexion.ListarBD(dgvGestion); // Vuelve a llenar la grilla con todos los productos
            txtBuscar.Clear();
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
              if (ValidarCampos())
            {
                string Nombre = txtUsuario.Text;
                string clave = txtContraseña.Text;
                string Correo = txtCorreo.Text;
                string Direccion = txtDireccion.Text;

            
                clsUsuario nuevousuario = new clsUsuario(0, Nombre, clave, Correo, Direccion, true, 2);


                conexion.RegistrarUsuario(nuevousuario);
                conexion.ListarBD(dgvGestion);

                LimpiarCampos();

                btnAgregar.Enabled = false;
                btnModificar.Enabled = true;
                btnEliminar.Enabled = true;
            }
            else
            {
                MessageBox.Show("Por favor, complete todos los campos requeridos.", "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                MessageBox.Show("El nombre de usuario no puede estar vacío.");
                return false;
            }

            if (usuarioSeleccionado == null && string.IsNullOrWhiteSpace(txtContraseña.Text))
            {
                MessageBox.Show("La contraseña no puede estar vacía al agregar un usuario.");
                return false;
            }

            return true;
        }
        private void LimpiarCampos()
        {
            txtUsuario.Clear();
            txtCorreo.Clear();
            txtDireccion.Clear();
            txtContraseña.Clear();
            chkEstado.Checked = false;
            cmbRol.SelectedIndex = -1;
            usuarioSeleccionado = 0;
        }

        private void dgvGestion_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvGestion_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow fila = dgvGestion.Rows[e.RowIndex];

            usuarioSeleccionado = Convert.ToInt32(fila.Cells["Id"].Value);
            txtUsuario.Text = fila.Cells["NombreUsuario"].Value.ToString(); // <- nombre con alias
            txtCorreo.Text = fila.Cells["Correo"].Value.ToString();
            txtDireccion.Text = fila.Cells["Direccion"].Value.ToString();
            chkEstado.Checked = Convert.ToBoolean(fila.Cells["Estado"].Value);
            cmbRol.SelectedValue = Convert.ToInt32(fila.Cells["RolId"].Value);

            txtContraseña.Clear(); // Seguridad: nunca se muestra la contraseña
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            if (usuarioSeleccionado == 0)
            {
                MessageBox.Show("Debe seleccionar un usuario para modificar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtCorreo.Text))
            {
                MessageBox.Show("El nombre y correo son obligatorios.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(conexion.cadena))
                {
                    cn.Open();

                    string query;

                    if (!string.IsNullOrWhiteSpace(txtContraseña.Text))
                    {
                        query = @"UPDATE Usuarios 
                          SET Nombre = @Nombre, Correo = @Correo, Direccion = @Direccion, 
                              Estado = @Estado, RolId = @RolId, Clave = @Clave 
                          WHERE Id = @Id";
                    }
                    else
                    {
                        query = @"UPDATE Usuarios 
                          SET Nombre = @Nombre, Correo = @Correo, Direccion = @Direccion, 
                              Estado = @Estado, RolId = @RolId 
                          WHERE Id = @Id";
                    }

                    SqlCommand comando = new SqlCommand(query, cn);
                    comando.Parameters.AddWithValue("@Nombre", txtUsuario.Text.Trim());
                    comando.Parameters.AddWithValue("@Correo", txtCorreo.Text.Trim());
                    comando.Parameters.AddWithValue("@Direccion", txtDireccion.Text.Trim());
                    comando.Parameters.AddWithValue("@Estado", chkEstado.Checked);
                    comando.Parameters.AddWithValue("@RolId", cmbRol.SelectedValue);
                    comando.Parameters.AddWithValue("@Id", usuarioSeleccionado);

                    if (!string.IsNullOrWhiteSpace(txtContraseña.Text))
                    {
                        comando.Parameters.AddWithValue("@Clave", txtContraseña.Text.Trim());
                    }

                    int filasAfectadas = comando.ExecuteNonQuery();

                    if (filasAfectadas > 0)
                    {
                        MessageBox.Show("Usuario modificado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        conexion.ListarBD(dgvGestion);
                        LimpiarCampos();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo modificar el usuario.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            
                if (usuarioSeleccionado == null)
                {
                    MessageBox.Show("Seleccioná un usuario para eliminar.");
                    return;
                }

                var confirmacion = MessageBox.Show("¿Deseás eliminar este usuario?", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirmacion != DialogResult.Yes)
                    return;

                conexion.EliminarUsuario((int)usuarioSeleccionado);
                MessageBox.Show("Usuario eliminado correctamente.");
                conexion.ListarBD(dgvGestion);
                LimpiarCampos();
                usuarioSeleccionado = null;
            
        }
    }
}
