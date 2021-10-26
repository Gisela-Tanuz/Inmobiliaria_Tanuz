using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_Tanuz.Models
{
    public class RepositorioInquilino : RepositorioBase, IRepositorio<Inquilino>
    {
        public RepositorioInquilino(IConfiguration configuration) : base(configuration)
        {

        }
        public IList<Inquilino> Obtener()
        {
            IList<Inquilino> res = new List<Inquilino>();
            using (SqlConnection connection = new(connectionString))
            {
                string sql = $"SELECT Id, Nombre, Apellido, Dni, Telefono, Email, NombreGarante, DireccionGarante," +
                    $"TelGarante, LugarDeTrabajo FROM Inquilino";
                using (SqlCommand command = new(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Inquilino i = new()
                        {
                            Id= reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            Dni = reader.GetString(3),
                            Telefono = reader["Telefono"].ToString(),
                            Email = reader.GetString(5),
                            NombreGarante = reader.GetString(6),
                            DireccionGarante = reader.GetString(7),
                            TelGarante = reader.GetString(8),
                            LugarDeTrabajo = reader.GetString(9),
                        };
                        res.Add(i);
                    }
                    connection.Close();
                }
            }
            return res;
        }

        public int Alta(Inquilino i)
        {
            var res = -1;
            using (SqlConnection connection = new(connectionString))
            {
                string sql = $"INSERT INTO Inquilino (Nombre, Apellido, Dni, Telefono, Email, NombreGarante, DireccionGarante,TelGarante, LugarDeTrabajo)" +
                    $"VALUES (@nombre, @apellido, @dni, @telefono, @email, @nombreGarante, @direccionGarante, @telGarante, @lugarDeTrabajo)" +
                    $"SELECT SCOPE_IDENTITY();";
                using (var command = new SqlCommand(sql, connection))
                {

                    command.Parameters.AddWithValue("@nombre", i.Nombre);
                    command.Parameters.AddWithValue("@apellido", i.Apellido);
                    command.Parameters.AddWithValue("@dni", i.Dni);
                    command.Parameters.AddWithValue("@telefono", i.Telefono);
                    command.Parameters.AddWithValue("@email", i.Email);
                    command.Parameters.AddWithValue("@nombreGarante", i.NombreGarante);
                    command.Parameters.AddWithValue("@direccionGarante", i.DireccionGarante);
                    command.Parameters.AddWithValue("@telGarante", i.TelGarante);
                    command.Parameters.AddWithValue("@lugarDeTrabajo", i.LugarDeTrabajo);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    i.Id = res;
                    connection.Close();

                }
            }

            return res;
        }

        public int Modificar(Inquilino i)
        {
            int res = -1;
            using (SqlConnection connection = new(connectionString))
            {
                string sql = $"UPDATE Inquilino SET" +
                    $" Nombre=@nombre, Apellido=@apellido, Dni=@dni, Telefono=@telefono, Email=@email," +
                    $" NombreGarante=@nombreGarante, DireccionGarante=@direccionGarante, TelGarante=@telGarante, LugarDeTrabajo=@lugarDeTrabajo" +
                    $" WHERE Id = @id";
                using (SqlCommand command = new(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", i.Nombre);
                    command.Parameters.AddWithValue("@apellido", i.Apellido);
                    command.Parameters.AddWithValue("@dni", i.Dni);
                    command.Parameters.AddWithValue("@telefono", i.Telefono);
                    command.Parameters.AddWithValue("@email", i.Email);
                    command.Parameters.AddWithValue("@nombreGarante", i.NombreGarante);
                    command.Parameters.AddWithValue("@direccionGarante", i.DireccionGarante);
                    command.Parameters.AddWithValue("@telGarante", i.TelGarante);
                    command.Parameters.AddWithValue("@lugarDeTrabajo", i.LugarDeTrabajo);
                    command.Parameters.AddWithValue("@id", i.Id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (SqlConnection connection = new(connectionString))
            {
                string sql = $"DELETE FROM Inquilino WHERE Id = @id";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return res;
        }
        public Inquilino ObtenerPorId(int id)
        {
            Inquilino i = null;
            using (SqlConnection connection = new(connectionString))
            {
                string sql = $"SELECT Id, Nombre, Apellido, Dni, Telefono, Email, NombreGarante, DireccionGarante," +
                $"TelGarante, LugarDeTrabajo  FROM Inquilino WHERE Id=@id;";
                using (var command = new SqlCommand(sql, connection))
                {

                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        i = new Inquilino();

                        {
                            i.Id = int.Parse(reader["Id"].ToString());
                            i.Nombre = reader["Nombre"].ToString();
                            i.Apellido = reader["Apellido"].ToString();
                            i.Dni = reader["Dni"].ToString();
                            i.Telefono = reader["Telefono"].ToString();
                            i.Email = reader["Email"].ToString();
                            i.NombreGarante = reader["NombreGarante"].ToString();
                            i.DireccionGarante = reader["DireccionGarante"].ToString();
                            i.TelGarante = reader["TelGarante"].ToString();
                            i.LugarDeTrabajo = reader["LugarDeTrabajo"].ToString();
                        };

                    }
                    connection.Close();
                }
            }
            return i;
        }
    }
}

