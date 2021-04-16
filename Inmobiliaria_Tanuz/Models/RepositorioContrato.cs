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
				string sql = $"INSERT INTO Contrato (IdContrato,InquilinoId,InmuebleId,FechaInicio,FechaFin ) " +
					" VALUES (@id, @inquilinoId, @inmuebleId, @fechaInicio, @fechaFin; " +
					" SELECT SCOPE_IDENTITY();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
				using (var command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@id", con.IdContrato);
					command.Parameters.AddWithValue(" @inquilinoId", con.InquilinoId);
					command.Parameters.AddWithValue("@inmuebleId", con.InmuebleId);
					command.Parameters.AddWithValue("@fechaInicio", con.FechaInicio);
					command.Parameters.AddWithValue(" @fechaFin", con.FechaFin);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					con.IdContrato= res;
					connection.Close();
				}
			}

			return res;
        }
		public int Baja(int id)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"DELETE FROM Contrato WHERE IdContrato = @id";
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
		public int Modificar(Contrato con)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "UPDATE Contrato SET " +
					"InquilinoId=@inquilinoId, InmuebleId=@inmuebleId, FechaInicio=@fechaInicio, FechaFin=@fechaFin" +
					"WHERE IdContrato = @id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
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
			}
			return res;
		}
		public IList<Contrato> Obtener()
		{
			IList<Contrato> res = new List<Contrato>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "SELECT IdContrato, InquilinoId, InmuebleId, FechaInicio, FechaFin, " +
					$" i.Nombre, i.Apellido , im.IdInmueble" +
					$" FROM ((Contrato c INNER JOIN Inquilino i ON c.InquilinoId = i.IdInquilino) " +
					$" INNER JOIN Inmueble im ON c.InmuebleId = im.IdInmueble)";
				using (SqlCommand command = new(sql, connection))
				{
					command.CommandType = CommandType.Text;
                    connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
						{
							Contrato contrato = new Contrato
							{
								IdContrato = reader.GetInt32(0),
								InquilinoId = reader.GetInt32(1),
								InmuebleId= reader.GetInt32(2),
								FechaInicio = reader.GetDateTime(3),
								FechaFin = reader.GetDateTime(4),

								Inquilino = new Inquilino
								{

									Nombre = reader.GetString(5),
									Apellido = reader.GetString(6),
								},
								Duenio = new Inmueble
								{
									PropietarioId = reader.GetInt32(7),

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
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = "SELECT IdContrato,InquilinoId,InmuebleId, FechaInicio, FechaFin , i.Nombre, i.Apellido, im.IdInmueble" +
					" FROM ((Contrato c INNER JOIN Inquilino i ON c.InquilinoId = i.IdInquilino) " +
					$" INNER JOIN Inmbuenle im ON c.InmuebleId = im.IdInmueble) "+
				    $" WHERE IdContarto=@id";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						Contrato con = new Contrato
						{
							IdContrato = reader.GetInt32(0),
							InquilinoId = reader.GetInt32(1),
							InmuebleId = reader.GetInt32(2),
							FechaInicio = reader.GetDateTime(3),
							FechaFin = reader.GetDateTime(4),

							Inquilino = new Inquilino
							{
								IdInquilino = reader.GetInt32(5),
								Nombre = reader.GetString(6),
								Apellido = reader.GetString(7),
							},
							Duenio = new Inmueble
							{
								PropietarioId = reader.GetInt32(8)
							}
						};
					}
					connection.Close();
				}

	        }
			return contrato;
		}

	}
}
