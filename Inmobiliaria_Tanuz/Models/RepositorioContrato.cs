using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_Tanuz.Models
{
	public class RepositorioContrato : RepositorioBase, IRepositorio<Contrato>
	{
		public RepositorioContrato(IConfiguration configuration) : base(configuration)
		{

		}
		public int Alta(Contrato con)
		{
			int res = -1;
			using (SqlConnection connection = new(connectionString))
			{
				string sql = $"INSERT INTO Contrato (InquilinoId, InmuebleId, FechaInicio, FechaFin ) " +
					$" VALUES (@inquilinoId, @inmuebleId, @fechaInicio, @fechaFin);" +
					$" SELECT SCOPE_IDENTITY();";//devuelve el id insertado
				using (SqlCommand command = new(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@inquilinoId", con.InquilinoId);
					command.Parameters.AddWithValue("@inmuebleId", con.InmuebleId);
					command.Parameters.AddWithValue("@fechaInicio", con.FechaInicio);
					command.Parameters.AddWithValue("@fechaFin", con.FechaFin);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					con.IdContrato = res;
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
				string sql = $"DELETE FROM Contrato WHERE IdContrato = {id}";
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
		public int Modificar(Contrato con)
		{
			int res = -1;
			using (SqlConnection connection = new(connectionString))
			{
				string sql = "UPDATE Contrato SET " +
					$" InquilinoId=@inquilinoId, InmuebleId=@inmuebleId, FechaInicio=@fechaInicio, FechaFin=@fechaFin " +
					$" WHERE Id = @id";
				using SqlCommand command = new(sql, connection);
				command.Parameters.AddWithValue("@inquilinoId", con.InquilinoId);
				command.Parameters.AddWithValue("@inmuebleId", con.InmuebleId);
				command.Parameters.AddWithValue("@fechaInicio", con.FechaInicio);
				command.Parameters.AddWithValue("@fechaFin", con.FechaFin);
				command.Parameters.AddWithValue("@id", con.IdContrato);
				command.CommandType = CommandType.Text;
				connection.Open();
				res = command.ExecuteNonQuery();
				connection.Close();
			}
			return res;
		}
		public IList<Contrato> Obtener()
		{
			IList<Contrato> res = new List<Contrato>();
			using (SqlConnection connection = new(connectionString))
			{
				string sql = "SELECT IdContrato, InquilinoId, InmuebleId, FechaInicio, FechaFin, " +
					  $" i.Nombre, i.Apellido, im.Direccion, im.Uso, im.Tipo, im.Precio, im.Estado, p.Nombre, p.Apellido " +
					  $" FROM Contrato c INNER JOIN Inquilino i ON c.InquilinoId = i.IdInquilino " +
					  $" INNER JOIN Inmueble im ON c.InmuebleId = im.IdInmueble " +
					  $"INNER JOIN Propietario p ON im.PropietarioId = p.IdPropietario ";
				using (SqlCommand command = new(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Contrato contrato = new()
						{
							IdContrato = reader.GetInt32(0),
							InquilinoId = reader.GetInt32(1),
							InmuebleId = reader.GetInt32(2),
							FechaInicio = reader.GetDateTime(3),
							FechaFin = reader.GetDateTime(4),

							Inquilino = new Inquilino
							{
								Nombre = reader.GetString(5),
								Apellido = reader.GetString(6),
							},
							Inmueble = new Inmueble
							{
								Direccion = reader.GetString(7),
								Uso = reader.GetString(8),
								Tipo = reader.GetString(9),
								Precio = reader.GetDecimal(10),
								Estado = reader.GetBoolean(11),
								Duenio = new Propietario
								{
									Nombre = reader.GetString(12),
									Apellido = reader.GetString(13),
								}
							}
						};
						res.Add(contrato);
					}

					connection.Close();
				}
			}
			return res;
		}
		public Contrato ObtenerPorId(int id)
		{
			Contrato contrato = null;
			using (SqlConnection connection = new(connectionString))
			{
				string sql = "SELECT IdContrato, InquilinoId, InmuebleId, FechaInicio, FechaFin, " +
					  $" i.Nombre, i.Apellido, im.Direccion,im.Uso, im.Tipo, im.Precio, im.Estado, p.Nombre, p.Apellido " +
					  $" FROM Contrato c INNER JOIN Inquilino i ON c.InquilinoId = i.IdInquilino " +
					  $" INNER JOIN Inmueble im ON c.InmuebleId = im.IdInmueble " +
					  $"INNER JOIN Propietario p ON im.PropietarioId = p.IdPropietario " +
					  $" WHERE IdContrato=@id";
				using (SqlCommand command = new(sql, connection))
				{
					command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						contrato = new()
						{
							IdContrato = reader.GetInt32(0),
							InquilinoId = reader.GetInt32(1),
							InmuebleId = reader.GetInt32(2),
							FechaInicio = reader.GetDateTime(3),
							FechaFin = reader.GetDateTime(4),

							Inquilino = new Inquilino
							{
								Nombre = reader.GetString(5),
								Apellido = reader.GetString(6),
							},
							Inmueble = new Inmueble
							{
								Direccion = reader.GetString(7),
								Uso = reader.GetString(8),
								Tipo = reader.GetString(9),
								Precio = reader.GetDecimal(10),
								Estado = reader.GetBoolean(11),
								Duenio = new Propietario
								{
									Nombre = reader.GetString(12),
									Apellido = reader.GetString(13),
								}
							}
						};

						connection.Close();
					}

				}
				return contrato;
			}

		}
		public Contrato ObtenerPorInmuebles(int id)
		{
			Contrato c = null;
			using (SqlConnection connection = new(connectionString))
			{
				string sql = $" SELECT IdContrato, c.InquilinoId, c.InmuebleId, FechaInicio, FechaFin, " +
					$" i.Nombre, i.Apellido ," +
					$" im.Direccion, im.Uso, im.Tipo, im.Precio" +
					$" FROM Contrato c INNER JOIN Inmueble im ON c.InmuebleId = im.IdInmueble " +
					$" INNER JOIN Inquilino i ON c.InquilinoId = i.IdInquilino " +
					$" WHERE c.InmuebleId = @id";

				using (SqlCommand command = new(sql, connection))
				{
					command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						c = new Contrato
						{
							IdContrato = reader.GetInt32(0),
							InquilinoId = reader.GetInt32(1),
							InmuebleId = reader.GetInt32(2),
							FechaInicio = reader.GetDateTime(3),
							FechaFin = reader.GetDateTime(4),

							Inquilino = new Inquilino
							{
								Nombre = reader.GetString(5),
								Apellido = reader.GetString(6),
							},

							Inmueble = new Inmueble
							{
								Direccion = reader.GetString(7),
								Uso = reader.GetString(8),
								Tipo = reader.GetString(9),
								Precio = reader.GetDecimal(10),
							}


						};
					}

				}
				connection.Close();
			}

			return c;

		}
		public List<Contrato> ObtenerContratoVigente(DateTime fechaInicio, DateTime fechaFin)
		{
			List<Contrato> res = new List<Contrato>();

			using (SqlConnection connection = new(connectionString))
			{
				string sql = $" SELECT IdContrato, InquilinoId, InmuebleId, FechaInicio, FechaFin, " +
					$" i.Nombre, i.Apellido ," +
					$" im.Direccion, im.Uso, im.Tipo, im.Precio,p.Nombre, p.Apellido" +
					$" FROM Contrato c INNER JOIN Inmueble im ON c.InmuebleId = im.IdInmueble " +
					$" INNER JOIN Inquilino i ON c.InquilinoId = i.IdInquilino " +
					$"INNER JOIN Propietario p ON im.PropietarioId = p.IdPropietario "+
					$" WHERE FechaInicio <= @fechaFin AND FechaFin >= @fechaInicio;";
				using (SqlCommand command = new(sql, connection))
				{
					command.Parameters.Add("@fechaInicio", SqlDbType.Date).Value = fechaInicio;
					command.Parameters.Add("@fechaFin", SqlDbType.Date).Value = fechaFin;
					command.CommandType = CommandType.Text;
					connection.Open();

					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Contrato c = new Contrato
						{
							IdContrato = reader.GetInt32(0),
							InquilinoId = reader.GetInt32(1),
							InmuebleId = reader.GetInt32(2),
							FechaInicio = reader.GetDateTime(3),
							FechaFin = reader.GetDateTime(4),

							Inquilino = new Inquilino
							{
								Nombre = reader.GetString(5),
								Apellido = reader.GetString(6),
							},

							Inmueble = new Inmueble
							{
								Direccion = reader.GetString(7),
								Uso = reader.GetString(8),
								Tipo = reader.GetString(9),
								Precio = reader.GetDecimal(10),
								Duenio = new Propietario
								{
									Nombre = reader.GetString(11),
									Apellido = reader.GetString(12),
								}
							}
						};
						res.Add(c);
					}
					connection.Close();
				}
			}
			return res;
		}
	}
}
