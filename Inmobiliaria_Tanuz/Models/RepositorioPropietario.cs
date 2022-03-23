using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_Tanuz.Models
{
    public class RepositorioPropietario : RepositorioBase, IRepositorio<Propietario>
    {
        

        public RepositorioPropietario (IConfiguration configuration) : base(configuration)
        {

        }
        public IList<Propietario> Obtener()
        {
            IList<Propietario> res = new List<Propietario>();
            using (SqlConnection connection = new(connectionString))
            {
                string sql = $"SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email, Usuario, Contraseña, AvatarProp" +
                    $" FROM Propietario";
                using (SqlCommand command = new(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Propietario p = new()
                        {
                            IdPropietario = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            Dni = reader.GetString(3),
                            Telefono = reader["Telefono"].ToString(),
                            Email = reader.GetString(5),
                            Usuario = reader.GetString(6),
                            Contraseña = reader.GetString(7),
                            AvatarProp = reader["AvatarProp"].ToString(),
                        };
                        res.Add(p);
                    }
                    connection.Close();
                }
            }
            return res;
        }

       
        public int Alta(Propietario p)
        {
            var res = -1;
            using (SqlConnection connection = new(connectionString))
            {
                string sql = $"INSERT INTO Propietario (Nombre, Apellido, Dni, Telefono, Email, Usuario, Contraseña, AvatarProp )" +
                    $" VALUES (@nombre, @apellido, @dni, @telefono, @email, @usuario, @contraseña,@avatarProp);" +
                    $" SELECT SCOPE_IDENTITY();";
                using (var command = new SqlCommand(sql, connection))
                {

                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@dni", p.Dni);
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@email", p.Email);
                    command.Parameters.AddWithValue("@usuario", p.Usuario);
                    command.Parameters.AddWithValue("@contraseña", p.Contraseña);
                    if (String.IsNullOrEmpty(p.AvatarProp))
                    {
                        command.Parameters.AddWithValue("@avatarProp", DBNull.Value);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@avatarProp", p.AvatarProp);
                    }

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    p.IdPropietario = res;
                    connection.Close();

                }
            }

            return res;
        }

        public int Modificar(Propietario p)
        {
            int res = -1;
            using (SqlConnection connection = new(connectionString))
            {
                string sql = $"UPDATE Propietario SET Nombre=@nombre, Apellido=@apellido, Dni=@dni, Telefono=@telefono," +
                    $" Email=@email, Usuario=@usuario,  Contraseña=@contraseña, AvatarProp=@avatarProp " +
                    $"WHERE IdPropietario = @id";
                using (SqlCommand command = new(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@nombre", p.Nombre);
                    command.Parameters.AddWithValue("@apellido", p.Apellido);
                    command.Parameters.AddWithValue("@dni", p.Dni);
                    command.Parameters.AddWithValue("@telefono", p.Telefono);
                    command.Parameters.AddWithValue("@email", p.Email);
                    command.Parameters.AddWithValue("@usuario", p.Usuario);
                    command.Parameters.AddWithValue("@contraseña", p.Contraseña);
                    command.Parameters.AddWithValue("@avatarProp", p.AvatarProp);
                    command.Parameters.AddWithValue("@id", p.IdPropietario);
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
                string sql = $"DELETE FROM Propietario WHERE IdPropietario = @id";
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
        public Propietario ObtenerPorId(int id)
        {
            Propietario p = null;
            using (SqlConnection connection = new(connectionString))
            {
                string sql = $"SELECT IdPropietario, Nombre, Apellido, Dni, " +
                    $" Telefono, Email, Usuario, Contraseña, AvatarProp FROM Propietario " +
                    $"WHERE IdPropietario=@id;";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    //SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        p = new Propietario()

                        {
                            IdPropietario = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            Dni = reader.GetString(3),
                            Telefono = reader.GetString(4),
                            Email = reader.GetString(5),
                            Usuario = reader.GetString(6),
                            Contraseña= reader.GetString(7),
                            AvatarProp = reader["AvatarProp"].ToString(),

                        };

                    }
                    connection.Close();
                }
            }
            return p;
        }
        public Propietario ObtenerPorEmail(string email)
        {
            Propietario p = null;
            using (SqlConnection connection = new(connectionString))
            {
                string sql = $"SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email, Usuario, Contraseña, AvatarProp FROM Propietario" +
                    $" WHERE Email=@email";
                using (SqlCommand command = new(sql, connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add("@email", SqlDbType.VarChar).Value = email;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        p = new Propietario
                        {
                            IdPropietario = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            Dni = reader.GetString(3),
                            Telefono = reader.GetString(4),
                            Email = reader.GetString(5),
                            Usuario = reader.GetString(6),
                            Contraseña = reader.GetString(7),
                            AvatarProp = reader["AvatarProp"].ToString(),
                        };
                    }
                    connection.Close();
                }
            }
            return p;
        }
        public IList<Propietario> BuscarPorNombre(string nombre)
        {
            IList<Propietario> res = new List<Propietario>();
            Propietario p = null;
            nombre = "%" + nombre + "%";
            using (SqlConnection connection = new(connectionString))
            {
                string sql = $"SELECT IdPropietario, Nombre, Apellido, Dni, Telefono, Email, Usuario, Contraseña, AvatarProp FROM Propietario" +
                    $" WHERE Nombre LIKE @nombre OR Apellido LIKE @nombre";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.Add("@nombre", SqlDbType.VarChar).Value = nombre;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        p = new Propietario
                        {
                            IdPropietario = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            Dni = reader.GetString(3),
                            Telefono = reader.GetString(4),
                            Email = reader.GetString(5),
                            Usuario = reader.GetString(6),
                            Contraseña = reader.GetString(7),
                            AvatarProp = reader["AvatarProp"].ToString(),
                        };
                        res.Add(p);
                    }
                    connection.Close();
                }
            }
            return res;
        }
        public IList<Inmueble> BuscarPropietario(int id)
        {
            IList<Inmueble> res = new List<Inmueble>();
            Inmueble i = null;
            using (SqlConnection connection = new(connectionString))
            {
                string sql = $"SELECT IdInmueble, PropietarioId, Direccion, Uso, Tipo, Ambientes,  Precio, Estado, Imagen , p.Nombre, p.Apellido" +
                    $" FROM Inmueble i INNER JOIN Propietario p ON i.PropietarioId = p.IdPropietario" +
                    $" WHERE PropietarioId=@id";
                using (SqlCommand command = new(sql, connection))
                {
                    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        i = new Inmueble
                        {
                            IdInmueble = reader.GetInt32(0),
                            PropietarioId = reader.GetInt32(1),
                            Direccion = reader.GetString(2),
                            Uso = reader.GetString(3),
                            Tipo = reader.GetString(4),
                            Ambientes = reader.GetInt32(5),
                            Precio = reader.GetDecimal(6),
                            Estado = reader.GetBoolean(7),
                            Imagen = reader["Imagen"].ToString(),
                            Duenio = new Propietario
                            {
                                Nombre = reader.GetString(9),
                                Apellido = reader.GetString(10),
                            }
                        };
                        res.Add(i);
                    }
                    connection.Close();
                }
            }
            return res;
        }
    }
}
