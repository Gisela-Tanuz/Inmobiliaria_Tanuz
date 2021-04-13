using System.Collections.Generic;

namespace Inmobiliaria_Tanuz.Models
{
   
        public interface IRepositorio<T>
        {
            IList<T> Obtener();
            int Alta(T p);
            int Baja(int id);
            int Modificar(T p);
            T ObtenerPorId(int id);
        }
    
}