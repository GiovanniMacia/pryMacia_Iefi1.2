using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using System.Data.SqlClient;

namespace pryMacia_Iefi1._2
{
    public partial class frmIngreso : MaterialForm
    {
        public frmIngreso()
        {
            InitializeComponent();
            var skinManager = MaterialSkinManager.Instance;
            skinManager.AddFormToManage(this);
            skinManager.Theme = MaterialSkinManager.Themes.DARK; // O LIGHT

            skinManager.ColorScheme = new ColorScheme(
                Primary.BlueGrey800, Primary.BlueGrey900,
                Primary.BlueGrey500, Accent.LightBlue200,
                TextShade.WHITE);
            txtContraseña.Password = true;
            chkMostrar.Checked = false;
            txtUsuario.Font = new Font("Georgia", 12, FontStyle.Regular);
            txtContraseña.Font = new Font("Georgia", 12, FontStyle.Regular);
        }

        private void frmIngreso_Load(object sender, EventArgs e)
        {

        }

        private void chkMostrar_CheckedChanged(object sender, EventArgs e)
        {
            txtContraseña.Password = !chkMostrar.Checked;
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            string username = txtUsuario.Text.Trim();
            string password = txtContraseña.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor, completá ambos campos.", "Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string connectionString = "Server=localhost\\SQLEXPRESS;Database=Iefi;Trusted_Connection=True;";
            string query = "SELECT RolId FROM Usuarios WHERE Nombre = @usuario AND Clave = @clave";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@usuario", username);
                    cmd.Parameters.AddWithValue("@clave", password);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        int rolId = Convert.ToInt32(reader["RolId"]);
                        MessageBox.Show("Inicio de sesión exitoso. RolId: " + rolId);

                        frmPrincipal ventana = new frmPrincipal(username, rolId);
                        ventana.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Usuario o contraseña incorrectos.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al conectar con la base de datos: " + ex.Message);
                }
            }
        }
    }
}
