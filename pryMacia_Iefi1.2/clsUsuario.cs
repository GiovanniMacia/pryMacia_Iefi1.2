using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pryMacia_Iefi1._2
{
    internal class clsUsuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string clave { get; set; }
        public string Correo { get; set; }
        public string Direccion { get; set; }   
        public bool Estado { get; set; }         
        public int RolId { get; set; } = 2;
        public clsUsuario() { }

        public clsUsuario(int id, string nombre, string clave,string Correo, string Direccion, bool Estado, int rolId)
        {
            this.Id = id;
            this.Nombre = nombre;
            this.clave = clave;
            this.Correo = Correo;
            this.Direccion = Direccion;
            this.Estado = Estado;
            this.RolId = rolId;
        }
    }
}
