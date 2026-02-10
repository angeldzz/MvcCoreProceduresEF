using Microsoft.EntityFrameworkCore;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;
using System.Data.Common;

namespace MvcCoreProceduresEF.Repositories
{
    #region Stored Procedures
    /*
create procedure SP_ALL_ENFERMOS
as
select * from ENFERMO
go

create procedure SP_FIND_ENFERMO
(@inscripcion nvarchar(50))
as
select * from ENFERMO
where INSCRIPCION=@inscripcion
go

create procedure SP_DELETE_ENFERMO
(@inscripcion nvarchar(50))
as
delete from ENFERMO where INSCRIPCION=@inscripcion
go
     */
    #endregion
    public class RepositoryEnfermos
    {
        private EnfermosContext context;
        public RepositoryEnfermos(EnfermosContext context)
        {
            this.context = context;
        }
        public async Task<List<Enfermo>> GetEnfermosAsync()
        {
            //vamos a utilizar un using para hacer un command que necesita una cadenad de conexion
            //el objeto connection lo ofrece EF
            using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand()) {
                string sql = "SP_ALL_ENFERMOS";
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.CommandText = sql;
                // Abrir la conexion partir del command
                await com.Connection.OpenAsync();
                // Ejecutar el command y obtener un datareader
                DbDataReader reader = await com.ExecuteReaderAsync();
                // Debemos mepar los datos manualmente
                List<Enfermo> enfermos = new List<Enfermo>();
                while (await reader.ReadAsync())
                {
                    Enfermo enfermo = new Enfermo();
                    enfermo.Inscripcion = reader["INSCRIPCION"].ToString();
                    enfermo.Apellido = reader["APELLIDO"].ToString();
                    enfermo.Direccion = reader["DIRECCION"].ToString();
                    enfermo.FechaNacimiento = DateTime.Parse(reader["FECHA_NAC"].ToString());
                    enfermo.Genero = reader["S"].ToString();
                    enfermo.Nss = reader["NSS"].ToString();
                    enfermos.Add(enfermo);
                }
                await reader.CloseAsync();
                await com.Connection.CloseAsync();
                return enfermos;
            }
        }
    }
}
