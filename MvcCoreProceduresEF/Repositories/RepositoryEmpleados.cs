using Microsoft.EntityFrameworkCore;
using MvcCoreProceduresEF.Data;
using MvcCoreProceduresEF.Models;

namespace MvcCoreProceduresEF.Repositories
{
    #region Vista
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

    }
}
