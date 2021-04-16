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
				string sql = $"INSERT INTO Inmueble (IdInmueble,PropietarioId,Direccion,Uso, Tipo," +
					$" Ambientes, Precio, Estado) " +
					$" VALUES (@id, @propietarioId, @direccion, @uso, @tipo, @ambientes, @precio, @estado " +
					$" SELECT SCOPE_IDENTITY();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
				using (var command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@id", i.IdInmueble);
					command.Parameters.AddWithValue(" @propietarioId", i.PropietarioId);
					command.Parameters.AddWithValue("@direccion", i.Direccion);
					command.Parameters.AddWithValue(" @udo", i.Uso);
					command.Parameters.AddWithValue(" @tipo", i.Tipo);
					command.Parameters.AddWithValue(" @ambientes", i.Ambientes);
					command.Parameters.AddWithValue(" @precio", i.Precio);
					command.Parameters.AddWithValue(" @estado", i.Estado);
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
				string sql = $"DELETE FROM Inmueble WHERE IdInmueble = @id";
				using (SqlCommand command = new SqlCommand(sql, connection))
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
		public int Modificar(Inmueble i)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "UPDATE Inmueble SET " +
					"PropietarioId=@propietarioId, Direccion=@direccion, Uso=@uso, Tipo=@tipo, " +
					" Ambientes=@ambientes, Precio=@precio, Estado=@estado " +
					" WHERE IdInmueble = @id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@propietarioId", i.PropietarioId);
					command.Parameters.AddWithValue("@direccion", i.Direccion);
					command.Parameters.AddWithValue("@uso", i.Uso);
					command.Parameters.AddWithValue("@tipo", i.Tipo);
					command.Parameters.AddWithValue("@ambientes", i.Ambientes);
					command.Parameters.AddWithValue("@precio", i.Precio);
					command.Parameters.AddWithValue("@estado", i.Estado);

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
			IList<Inmueble> res = new List<Inmueble>();
			using (SqlConnection connection = new(connectionString))
			{
				string sql = "SELECT IdInmueble, PropietarioId, Direccion,Uso, Tipo, Ambientes, Precio, Estado," +
					$" p.Nombre, p.Apellido" +
					$" FROM Inmuueble i INNER JOIN Propietario p ON i.PropietarioId = p.IdPropietario ";
				using (SqlCommand command = new(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble inmueble = new Inmueble
						{
							IdInmueble = reader.GetInt32(0),
							PropietarioId = reader.GetInt32(1),
							Direccion = reader.GetString(2),
							Uso = reader.GetString(3),
							Tipo = reader.GetString(4),
							Ambientes = reader.GetInt32(5),
							Precio = reader.GetDecimal(6),
							Estado = reader.GetString(7),
							Duenio = new Propietario
							{

								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
							}
						};
						res.Add(inmueble);
					}

					connection.Close();
				}
			}
			return res;
		}
		public Inmueble ObtenerPorId(int id)
		{
			Inmueble inmueble = null;
			using (SqlConnection connection = new (connectionString))
			{
				string sql = "SELECT IdInmueble, PropietarioId, Direccion,Uso, Tipo, Ambientes, Precio, Estado, " +
					$" p.Nombre, p.Apellido" +
					$" FROM Inmuueble i INNER JOIN Propietario p ON i.PropietarioId = p.IdPropietario "+
				$" WHERE IdContarto=@id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
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
							Estado = reader.GetString(7),
							Duenio = new Propietario
							{

								Nombre = reader.GetString(8),
								Apellido = reader.GetString(9),
							}

						};
					}
					connection.Close();
				}

			}
			return inmueble;
		}
	}
}
					
	

