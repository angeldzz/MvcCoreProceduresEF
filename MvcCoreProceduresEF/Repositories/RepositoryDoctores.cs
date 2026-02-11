using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;

namespace MvcCoreProceduresEF.Repositories
{
    #region STORED PROCEDURES
    /*
create procedure SP_GET_ESPECIALIDADES
AS
SELECT DISTINCT ESPECIALIDAD FROM DOCTOR
GO

CREATE PROCEDURE SP_UPDATE_SALARIO_DOCTORES
(@especialidad nvarchar(50), @incremento int)
AS
	UPDATE DOCTOR SET SALARIO = (SALARIO + @incremento) WHERE ESPECIALIDAD = @especialidad
GO
CREATE PROCEDURE SP_DOCTORES_ESPECIALIDAD
(@especialidad nvarchar(50))
AS
	SELECT * FROM DOCTOR WHERE ESPECIALIDAD = @especialidad
GO


EXEC SP_GET_ESPECIALIDADES
EXEC SP_UPDATE_SALARIO_DOCTORES 'Psiquiatría', 20
EXEC SP_DOCTORES_ESPECIALIDAD 'Cardiología'
     */
    #endregion
    public class RepositoryDoctores
    {
        private EnfermosContext context;
        public RepositoryDoctores(EnfermosContext context)
        {
            this.context = context;
        }
        public async Task<List<string>> GetEspecialidadAsync()
        {
            string sql = "SP_GET_ESPECIALIDADES";
            var especialidades = new List<string>();
            using (var command = this.context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                await this.context.Database.OpenConnectionAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        especialidades.Add(reader["ESPECIALIDAD"].ToString());
                    }
                }
                await this.context.Database.CloseConnectionAsync();
            }
            return especialidades;
        }
        public async Task<List<Doctor>> GetDoctoresPorEspecialidadAsync(string especialidad)
        {
            string sql = "SP_DOCTORES_ESPECIALIDAD @especialidad";
            var param = new SqlParameter("@especialidad", especialidad);
            var doctores = await this.context.Doctores.FromSqlRaw(sql, param).ToListAsync();
            return doctores;
        }
        public async Task IncrementarSalarioDoctoresAsync(string especialidad, int incremento)
        {
            string sql = "SP_UPDATE_SALARIO_DOCTORES @especialidad, @incremento";
            var esp = new SqlParameter("@especialidad", especialidad);
            var incre = new SqlParameter("@incremento", incremento);
            await this.context.Database.ExecuteSqlRawAsync(sql, esp, incre);
        }
        public async Task IncrementarSalarioDoctoresAsyncNORAW(string especialidad, int incremento)
        {
            string sql = "UPDATE DOCTOR SET SALARIO = (SALARIO + @incremento) WHERE ESPECIALIDAD = @especialidad";
            var esp = new SqlParameter("@especialidad", especialidad);
            var incre = new SqlParameter("@incremento", incremento);
            await this.context.Database.ExecuteSqlRawAsync(sql, esp, incre);
        }
    }
}
