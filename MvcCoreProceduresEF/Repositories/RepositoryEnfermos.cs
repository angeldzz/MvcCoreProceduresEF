using Microsoft.Data.SqlClient;
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

create procedure SP_INSERT_ENFERMO
(@apellido nvarchar(50),@direccion nvarchar(50), @fecha_nac datetime, @s nvarchar(50), @nss nvarchar(50))
AS
DECLARE @inscripcion int
select @inscripcion = MAX(ENFERMO.INSCRIPCION) FROM ENFERMO;
INSERT INTO ENFERMO (INSCRIPCION, APELLIDO, DIRECCION, FECHA_NAC, S, NSS)
    VALUES (CAST(@inscripcion AS NVARCHAR(50)), @apellido, @direccion, @fecha_nac, @s, @nss);
GO

EXEC SP_INSERT_ENFERMO 'Angel', 'VALENCIA', '19670623', 'F', '545343243';
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
            using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            {
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
            public async Task<Enfermo> FindEnfermoAsync(string inscripcion)
        {
            //PARA LLAMAR A UN PROCEDIMIENTO QUE CONTIENE PARAMETROS
            //LA LLAMADA SE REALIZA MEDIANTE EL NOMBRE DEL PROCEDIMIENTO Y CADA
            //PARAMETRO A CONTINUACION
            string sql = "SP_FIND_ENFERMO @inscripcion";
            SqlParameter pamIns = new SqlParameter("@inscripcion", inscripcion);
            //SI LOS DATOS QUE DEVUELVE EL PROCEDURE ESTAN MAPEADOS CON UN MODEL,
            //PODEMOS UTILIZAR EL COMANDO FromSqlRaw PARA RECUPERAR DIRECTAMENTE EL MODEL/S
            //NO PODEMOS CONSULTAR Y EXTRAER A LA VEZ SE DEBE REALIZAR EN DOS PASOS
            Enfermo enfermo = await this.context.Enfermos.FirstOrDefaultAsync(e => e.Inscripcion == inscripcion);
            return enfermo;
        }
        public async Task DeleteEnfermoAsync(string inscripcion)
        {
            string sql = "SP_DELETE_ENFERMO";
            SqlParameter pamIns = new SqlParameter("@inscripcion", inscripcion);
            using (DbCommand com = this.context.Database.GetDbConnection().CreateCommand())
            {
                com.CommandType = System.Data.CommandType.StoredProcedure;
                com.CommandText = sql;
                com.Parameters.Add(pamIns);
                await com.Connection.OpenAsync();
                await com.ExecuteNonQueryAsync();
                await com.Connection.CloseAsync();
                com.Parameters.Clear();
            }
        }
        public async Task DeleteEnfermoRawAsync(string inscripcion)
        {
            string sql = "SP_DELETE_ENFERMO @inscripcion";
            SqlParameter pamIns = new SqlParameter("@inscripcion", inscripcion);
            await this.context.Database.ExecuteSqlRawAsync(sql, pamIns);
        }
        public async Task InsertEnfermoAsync(string apellido, 
            string direccion, DateTime fechaNac, 
            string genero, string nss)
        {
            string sql = "SP_INSERT_ENFERMO @apellido, @direccion, @fecha_nac, @s, @nss";
            SqlParameter pamApell = new SqlParameter("@apellido", apellido);
            SqlParameter pamDir = new SqlParameter("@direccion", direccion);
            SqlParameter pamFecha = new SqlParameter("@fecha_nac", fechaNac);
            SqlParameter pamGen = new SqlParameter("@s", genero);
            SqlParameter pamNss = new SqlParameter("@nss", nss);
            await this.context.Database.ExecuteSqlRawAsync(sql, pamApell, pamDir, pamFecha, pamGen, pamNss);
        }
    }
}
