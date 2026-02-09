using Microsoft.Extensions.Primitives;
using MvcNetCoreLinqToSqlInjection.Models;

namespace MvcNetCoreLinqToSqlInjection.Repositories
{
    public interface IRepositoryDoctores
    {
        List<Doctor> GetDoctoresEspecialidad(string especialidad);

        Doctor FindDoctor(int idDoctor);
        Task UpdateDoctorAsync(int idDoctor, string apellido,
            string especialidad, int salario, int idHospital);

        List<Doctor> GetDoctores();

        Task CreateDoctorAsync
            (int idDoctor, string apellido
            , string especialidad, int salario, int idHospital);

        Task DeleteDoctorAsync(int idDoctor);
    }
}
