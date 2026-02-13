using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;

namespace MvcCoreProceduresEF.Repositories
{
    #region Vistas y Procedimientos SQL
    /*
create view V_EMPLEADOS_DEPARTAMENTOS
AS
	SELECT CAST(
	ISNULL(ROW_NUMBER() OVER (ORDER BY EMP.APELLIDO), 0) 
		as Int)
	AS ID,
	EMP.APELLIDO, EMP.OFICIO, EMP.SALARIO, 
	DEPT.DNOMBRE AS DEPARTAMENTO,
	DEPT.LOC AS LOCALIDAD
	FROM EMP
	INNER JOIN DEPT
	ON EMP.DEPT_NO = DEPT.DEPT_NO
GO
    ----------------------------------------------
    create view V_TRABAJADORES
AS
	SELECT EMP_NO AS IDTRABAJADOR,
	APELLIDO, OFICIO, SALARIO FROM EMP
	UNION
	SELECT DOCTOR_NO, APELLIDO,ESPECIALIDAD,SALARIO FROM DOCTOR
	UNION
	SELECT EMPLEADO_NO, APELLIDO, FUNCION, SALARIO FROM PLANTILLA
GO
	----------------------------------------------
    create procedure SP_TRABAJADORES_OFICIO
(@oficio nvarchar(50),
@personas int = 0 out,
@media int = 0 out,
@suma int = 0 out)
AS
	select * from V_TRABAJADORES
	WHERE OFICIO = @oficio
	select @personas = COUNT(IDTRABAJADOR),
		@media = AVG(SALARIO),
		@suma = sum(SALARIO) from V_TRABAJADORES
	WHERE OFICIO = @oficio
GO

     */
    #endregion
    public class RepositoryEmpleados
    {
        private HospitalContext context;
        public RepositoryEmpleados(HospitalContext context)
        {
            this.context = context;
        }
        public async Task<List<VistaEmpleado>> GetVistaEmpleadosDepartamentosAsync()
        {
            var consulta = from datos in context.VistaEmpleados
                           select datos;
            return await consulta.ToListAsync();
        }
		public async Task<TrabajadoresModel> GetTrabajadoresModelAsync()
		{
			//Primero con LINQ
			var consulta = from datos in this.context.Trabajadores
						   select datos;
			TrabajadoresModel model = new TrabajadoresModel();
			model.Trabajadores = await consulta.ToListAsync();
			model.Personas = await consulta.CountAsync();
			model.SumaSalarial = await consulta.SumAsync(z => z.Salario);
			model.MediaSalarial = (int) await consulta.AverageAsync(z => z.Salario);
			return model;
        }
		public async Task<List<string>> GetOficiosAsync()
		{
			var consulta = (from datos in this.context.Trabajadores select datos.Oficio).Distinct();
			return await consulta.ToListAsync();
        }
		public async Task<TrabajadoresModel> GetTrabajadoresModelOficioAsync(string oficio)
		{
			//Ya QUE TENEMOS MODEL, VAMOS A LLAMARLO CON EF
			//La unica diferencia cuando tenemos parametros de salida
			//es indicar la palabra OUT en la declaracion de las variables
			string sql = "SP_TRABAJADORES_OFICIO @oficio, @personas OUT, @media OUT, @suma OUT";
			SqlParameter PamOficio = new SqlParameter("@oficio", oficio);
			SqlParameter PamPersonas = new SqlParameter("@personas", -1);
            PamPersonas.Direction = System.Data.ParameterDirection.Output;
			SqlParameter PamMedia = new SqlParameter("@media", -1);
			PamMedia.Direction = System.Data.ParameterDirection.Output;
			SqlParameter PamSuma = new SqlParameter("@suma", -1);
			PamSuma.Direction = System.Data.ParameterDirection.Output;
			var consulta = this.context.Trabajadores.FromSqlRaw(sql, PamOficio, PamPersonas, PamMedia, PamSuma);
			TrabajadoresModel model = new TrabajadoresModel();
            model.Trabajadores = await consulta.ToListAsync();
			model.Personas = int.Parse(PamPersonas.Value.ToString());
            model.MediaSalarial = int.Parse(PamMedia.Value.ToString());
			model.SumaSalarial = int.Parse(PamSuma.Value.ToString());
            return model;

        }
    }
}
