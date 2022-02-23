using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Inmobiliaria_Tanuz.Models
{
	public class RepositorioPagos : RepositorioBase, IRepositorio<Pagos>
	{
		public RepositorioPagos(IConfiguration configuration) : base(configuration)
		{

		}
		public int Alta(Pagos p)
		{
			int res = -1;
			using (SqlConnection connection = new(connectionString))
			{
				string sql = $"INSERT INTO Pago (ContratoId, NroDePago, Fecha, Importe )" +
					$" VALUES (@contratoId, @nroDePago, @fecha, @importe )" +
					$" SELECT SCOPE_IDENTITY();";//devuelve el id insertado
				using (var command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@contratoId", p.ContratoId);
					command.Parameters.AddWithValue("@nroDePago", p.NroDePago);
					command.Parameters.AddWithValue("@fecha", p.Fecha);
					command.Parameters.AddWithValue("@importe", p.Importe);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					p.IdPago = res;
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
				string sql = $"DELETE FROM Pago WHERE IdPago = @id";
				using (SqlCommand command = new (sql, connection))
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
		public int Modificar(Pagos p)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "UPDATE Pago SET " +
					$" ContratoId=@contratoId, NroDePago=@nroDePago, Fecha=@fecha, Importe=@importe " +
					$" WHERE IdPago=@id";
				using (SqlCommand command = new(sql, connection))
				{
					command.Parameters.AddWithValue("@contratoId", p.ContratoId);
					command.Parameters.AddWithValue("@nroDePago", p.NroDePago);
					command.Parameters.AddWithValue("@fecha", p.Fecha);
					command.Parameters.AddWithValue("@importe", p.Importe);
					command.Parameters.AddWithValue("@IdPago", p.IdPago);
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
		public IList<Pagos> Obtener()
		{
			IList<Pagos> res = new List<Pagos>();
			using (SqlConnection connection = new(connectionString))
			{
				string sql = "SELECT IdPago , ContratoId, NroDePago, Fecha, Importe, i.Uso, i.Tipo, i.Precio " +
					$" FROM Pago p INNER JOIN Contrato c ON p.ContratoId = c.IdContrato" +
					 $" INNER JOIN Inmueble i ON c.InmuebleId = i.IdInmueble ";
				using (SqlCommand command = new(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Pagos pagos = new()
						{
							IdPago = reader.GetInt32(0),
							ContratoId = reader.GetInt32(1),
							NroDePago = reader.GetInt32(2),
							Fecha = reader.GetDateTime(3),
							Importe = reader.GetDecimal(4),
							contrato = new Contrato
							{
								//IdContrato = reader.GetInt32(1),
								Inmueble = new Inmueble
								{
									Uso = reader.GetString(5),
									Tipo = reader.GetString(6),
									Precio = reader.GetDecimal(7),
								}
							}

						};
						res.Add(pagos);
					}

					connection.Close();
				}
			}
			return res;
		}
		public Pagos ObtenerPorId(int id)
		{
			Pagos pagos = null;
			using (SqlConnection connection = new(connectionString))
			{

				string sql = "SELECT IdPago, ContratoId, NroDePago, Fecha, Importe, i.Direccion, i.Uso, i.Tipo, i.Precio" +
					$" FROM Pago p INNER JOIN Contrato c ON p.ContratoId = c.IdContrato " +
					$"INNER JOIN Inmueble i ON c.InmuebleId = i.IdInmueble "+ 
					$" WHERE IdPago=@id";
				using (SqlCommand command = new(sql, connection))
				{
					command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						pagos = new Pagos
						{
							IdPago = reader.GetInt32(0),
							ContratoId = reader.GetInt32(1),
							NroDePago = reader.GetInt32(2),
							Fecha = reader.GetDateTime(3),
							Importe = reader.GetDecimal(4),
							contrato = new Contrato
							{
								//Id = reader.GetInt32(5),
								Inmueble = new Inmueble
								{
									Direccion = reader.GetString(5),
									Uso = reader.GetString(6),
									Tipo = reader.GetString(7),
									Precio = reader.GetDecimal(8),
								}
							}

						};
					}
					connection.Close();
				}

			}
			return pagos;
		}
		public IList<Pagos> ObtenerPagoxContrato(int id)
		{
			Pagos p = null;
			IList<Pagos> res = new List<Pagos>();
				using (SqlConnection connection = new(connectionString))
				{
					string sql = $"SELECT IdPago, p.ContratoId, NroDePago, Fecha, Importe, " +
						 $"c.InquilinoId, c.InmuebleId," +
						$"im.Direccion, im.Uso, im.Tipo," +
						$"i.Nombre, i.Apellido " +
						$" FROM Pago p INNER JOIN Contrato c ON c.IdContrato = p.ContratoId " +
						$" INNER JOIN Inmueble im ON im.IdInmueble = c.InmuebleId " +
						$" INNER JOIN Inquilino i ON i.IdInquilino = c.InquilinoId " +
						$" WHERE p.ContratoId=@id";

					using (SqlCommand command = new(sql, connection))
					{
						command.CommandType = CommandType.Text;
						command.Parameters.AddWithValue("@id", id);
						connection.Open();
						var reader = command.ExecuteReader();
						while (reader.Read())
						{
		           	       p = new Pagos
							{
								IdPago = reader.GetInt32(0),
								ContratoId = reader.GetInt32(1),
								NroDePago = reader.GetInt32(2),
								Fecha = reader.GetDateTime(3),
								Importe = reader.GetDecimal(4),
								contrato = new Contrato
								{
									
									InmuebleId = reader.GetInt32(5),
									InquilinoId = reader.GetInt32(6),
									Inmueble = new Inmueble
									{ 
										Direccion = reader.GetString(7),
										Uso = reader.GetString(8),
										Tipo = reader.GetString(9),
									},
									Inquilino = new Inquilino
									{
										Nombre = reader.GetString(10),
										Apellido = reader.GetString(11),
									},
								}
							};
							res.Add(p);
						}
						connection.Close();
					}
				}
				return res;

			}
		}

}

