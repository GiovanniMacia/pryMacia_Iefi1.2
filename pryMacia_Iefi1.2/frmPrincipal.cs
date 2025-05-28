using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pryMacia_Iefi1._2
{
    public partial class frmPrincipal : MaterialForm
    {
        private string nombreUsuario;
        private int rolUsuario;
        private DateTime fechaInicio;
        private DateTime horaInicio;
        private DateTime horaFin;
        private TimeSpan tiempoTranscurrido;


        private int tiempoActual = 0;
        clsConexionBD conexion = new clsConexionBD();
        public frmPrincipal(string nombre, int rol)
        {
            InitializeComponent();
            nombreUsuario = nombre;
            rolUsuario = rol;
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            horaInicio = DateTime.Now;       //hora
            fechaInicio = horaInicio.Date;   //fecha

            Tiempo.Enabled = true; //inicia el timer

            //Usuario activo
            lblUsuarioActivo.Text = $"Bienvenido: {nombreUsuario}";

            //Fecha de Inicio
            lblFechaInicio.Text = $"Fecha de Inicio: {horaInicio.ToString("dd/MM/yyyy")}";


            //control de acceso
            if (rolUsuario == 1)
            {
                menuPrincipal.Visible = true;
                menuTareas.Visible = false;
                menuAdministracion.Visible = true;

            }
            else
            {
                menuPrincipal.Visible = true;
                menuTareas.Visible = true;
                menuAdministracion.Visible = false;
            }
        }
        

        private void Tiempo_Tick(object sender, EventArgs e)
        {
            tiempoActual++;
            TimeSpan ts = TimeSpan.FromSeconds(tiempoActual);
            LblTiempo.Text = ts.ToString(@"hh\:mm\:ss");
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Estás seguro que quieres Cerrar Sesión?", "Cerrar Sesión", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Tiempo.Enabled = false; // Detener el timer si lo usás
                this.Hide(); // Oculta el formulario actual
                frmIngreso login = new frmIngreso();
                login.Show();
            }
        }

        private void menuAuditoria_Click(object sender, EventArgs e)
        {
            frmAuditoria audi = new frmAuditoria();
            audi.ShowDialog();
        }

        private void frmPrincipal_FormClosed(object sender, FormClosedEventArgs e)
        {
            Tiempo.Enabled = false; //detiene el timer

            horaFin = DateTime.Now; //obtiene la hora de cierre 

            //Tiempo Transcurrido
            tiempoTranscurrido = horaFin - horaInicio;

            //Obtengo Id del Usuario por Nombre
            int idUsuario = conexion.ObtenerIdUsuarioPorNombre(nombreUsuario);

            clsRegistro registro = new clsRegistro(0, idUsuario, fechaInicio, horaInicio, horaFin, tiempoTranscurrido);


            conexion.GuardarSesion(registro);
            Application.Exit();
        }

        private void menuUsuarios_Click(object sender, EventArgs e)
        {
            frmGestion gestion = new frmGestion();
            gestion.ShowDialog();
        }
    }
}

