using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_Tanuz.Models
{
	public class RepositorioInmueble : RepositorioBase, IRepositorio<Inmueble>
	{
		public RepositorioInmueble(IConfiguration configuration) : base(configuration)
		{
		}

		public int Alta(Inmueble i)
		{
			int res = -1;
			using (SqlConnection connection = new(connectionString))
			{
				string sql = $"INSERT INTO Inmueble (PropietarioId, Direccion, Uso, Tipo, Ambientes, Precio, Estado, Imagen) " +
					$" VALUES (@propietarioId, @direccion, @uso, @tipo, @ambientes, @precio, @estado,@imagen);" +
					$" SELECT SCOPE_IDENTITY();"; //devuelve el id insertado
				using (SqlCommand command = new(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@propietarioId", i.PropietarioId);
					command.Parameters.AddWithValue("@direccion", i.Direccion);
					command.Parameters.AddWithValue("@uso", i.Uso);
					command.Parameters.AddWithValue("@tipo", i.Tipo);
					command.Parameters.AddWithValue("@ambientes", i.Ambientes);
					command.Parameters.AddWithValue("@precio", i.Precio);
					command.Parameters.AddWithValue("@estado", i.Estado);
					if (String.IsNullOrEmpty(i.Imagen))
					{
						command.Parameters.AddWithValue("@imagen", DBNull.Value);
					}
					else
					{
						command.Parameters.AddWithValue("@imagen", i.Imagen);
					}
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					i.IdInmueble = res;
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
				string sql = $"DELETE FROM Inmueble WHERE IdInmueble = {id}";
				using (SqlCommand command = new(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
		public int Modificar(Inmueble i)
		{
			int res = -1;
			using (SqlConnection connection = new(connectionString))
			{
				string sql = "UPDATE Inmueble SET " +
					$" PropietarioId=@propietarioId, Direccion=@direccion, Uso=@uso, Tipo=@tipo, " +
					$" Ambientes=@ambientes, Precio=@precio, Estado=@estado, Imagen=@imagen " +
					$" WHERE IdInmueble = @id";
				using (SqlCommand command = new(sql, connection))
				{
					command.Parameters.AddWithValue("@propietarioId", i.PropietarioId);
					command.Parameters.AddWithValue("@direccion", i.Direccion);
					command.Parameters.AddWithValue("@uso", i.Uso);
					command.Parameters.AddWithValue("@tipo", i.Tipo);
					command.Parameters.AddWithValue("@ambientes", i.Ambientes);
					command.Parameters.AddWithValue("@precio", i.Precio);
					command.Parameters.AddWithValue("@estado", i.Estado);
					command.Parameters.AddWithValue("@imagen", i.Imagen);
					command.Parameters.AddWithValue("@id", i.IdInmueble);
					
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
		public IList<Inmueble> Obtener()
		{
			List<Inmueble> res = new List<Inmueble>();
			using (SqlConnection connection = new(connectionString))
			{
				string sql = $"SELECT IdInmueble , PropietarioId, Direccion, Uso, Tipo, Ambientes,Precio, Estado, Imagen, p.Nombre, p.Apellido" +
					$" FROM Inmueble i INNER JOIN Propietario p ON i.PropietarioId = p.IdPropietario";
				using (SqlCommand command = new(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble entidad = new Inmueble
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
						res.Add(entidad);
					}
					connection.Close();
				}
			}
			return res;
		}
		public Inmueble ObtenerPorId(int id)
		{
			Inmueble inmueble = null;
			using (SqlConnection connection = new(connectionString))
			{
				string sql = $"SELECT IdInmueble, PropietarioId, Direccion, Uso, Tipo, Ambientes,  Precio, Estado, Imagen, p.Nombre, p.Apellido" +
					$" FROM Inmueble i INNER JOIN Propietario p ON i.PropietarioId = p.IdPropietario" +
					$" WHERE IdInmueble=@id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						inmueble = new()
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
					}
					connection.Close();
				}

			}
			return inmueble;
		}
		public int EstadoDisponible(Inmueble inmueble)
		{
			int i = -1;
			using (SqlConnection connection = new(connectionString))
			{
				string sql = $"UPDATE Inmueble SET Estado= {1} " +
					$"WHERE IdInmueble = @id";
				using (SqlCommand command = new(sql, connection))
				{
					command.Parameters.AddWithValue("@id", inmueble.IdInmueble);
					command.CommandType = CommandType.Text;
					connection.Open();
					i = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return i;
		}
		public int EstadoNoDisponible(Inmueble inmueble)
		{
			int i = -1;
			using (SqlConnection connection = new(connectionString))
			{
				string sql = $"UPDATE Inmueble SET Estado= {0} " +
					$"WHERE IdInmueble = @id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{

					command.Parameters.AddWithValue("@id", inmueble.IdInmueble);
					command.CommandType = CommandType.Text;
					connection.Open();
					i = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return i;
		}
		public IList<Inmueble> BuscarPorPropietario(int id)
		{
			IList<Inmueble> res = new List<Inmueble>();
			Inmueble i = null;
			using (SqlConnection connection = new(connectionString))
			{
				string sql = $"SELECT IdInmueble, PropietarioId, Direccion, Uso, Tipo, Ambientes, Precio, Estado, Imagen" +
					$" FROM Inmueble" +
					$" WHERE PropietarioId=@propietarioId AND Estado = 1";
				using (SqlCommand command = new(sql, connection))
				{
					command.Parameters.Add("@propietarioId", SqlDbType.Int).Value = id;
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
						};
						res.Add(i);
					}
					connection.Close();
				}
			}
			return res;
		}
		public IList<Inmueble> ObtenerDisponibles()
		{
			IList<Inmueble> inmuebles = new List<Inmueble>();
			using (SqlConnection connection = new(connectionString))
			{
				string sql = $"SELECT IdInmueble, PropietarioId, Direccion, Uso, Tipo, Ambientes,  Precio, Estado,Imagen, p.Nombre, p.Apellido" +
					$" FROM Inmueble i INNER JOIN Propietario p ON i.PropietarioId = p.IdPropietario" +
					$" WHERE Estado= {1} ";
				using (SqlCommand command = new(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble i = new Inmueble
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
						inmuebles.Add(i);
					}
					connection.Close();
				}
			}
			return inmuebles;
		}

		
		
		public IList<Inmueble> BuscarPropietario(int id)
		{
			IList<Inmueble> res = new List<Inmueble>();
			Inmueble i = null;
			using (SqlConnection connection = new(connectionString))
			{
				string sql = $"SELECT IdPropietario, PropietarioId, Direccion, Uso, Tipo, Ambientes,  Precio, Estado,Imagen , p.Nombre, p.Apellido" +
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
					
	

