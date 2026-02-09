using Microsoft.AspNetCore.Http.HttpResults;
using MvcNetCoreLinqToSqlInjection.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using static Azure.Core.HttpHeader;

#region STORED PROCEDURES

//create or replace procedure SP_DELETE_DOCTOR
//(p_iddoctor DOCTOR.DOCTOR_NO%type)
//AS
//BEGIN
//    delete from DOCTOR where DOCTOR_NO=p_iddoctor;
//commit;
//end;
//create or replace procedure SP_UPDATE_DOCTOR
//(p_iddoctor DOCTOR.DOCTOR_NO%TYPE
//, p_apellido DOCTOR.APELLIDO%TYPE
//, p_especialidad DOCTOR.ESPECIALIDAD%TYPE
//, p_salario DOCTOR.SALARIO%TYPE
//, p_idhospital DOCTOR.HOSPITAL_COD%TYPE)
//as
//begin
//    update DOCTOR set APELLIDO=p_apellido
//    , ESPECIALIDAD = p_especialidad
//    , SALARIO = p_salario, HOSPITAL_COD = p_idhospital
//    where DOCTOR_NO=p_iddoctor;
//commit;
//end;

#endregion

namespace MvcNetCoreLinqToSqlInjection.Repositories
{
    public class RepositoryDoctoresOracle: IRepositoryDoctores
    {
        private DataTable tablaDoctor;
        private OracleConnection cn;
        private OracleCommand com;

        public RepositoryDoctoresOracle()
        {
            string connectionString = @"Data Source=LOCALHOST:1521/FREEPDB1;Persist Security Info=true;User Id=SYSTEM;Password=oracle";
            this.cn = new OracleConnection(connectionString);
            this.com = new OracleCommand();
            this.com.Connection = this.cn;
            this.tablaDoctor = new DataTable();
            string sql = "select * from DOCTOR";
            OracleDataAdapter ad =
                new OracleDataAdapter(sql, this.cn);
            ad.Fill(this.tablaDoctor);
        }

        public List<Doctor> GetDoctores()
        {
            var consulta = from datos in
                               this.tablaDoctor.AsEnumerable()
                           select datos;
            List<Doctor> doctores = new List<Doctor>();
            foreach (var row in consulta)
            {
                Doctor doc = new Doctor
                {
                    IdDoctor = row.Field<int>("DOCTOR_NO"),
                    Apellido = row.Field<string>("APELLIDO"),
                    Especialidad = row.Field<string>("ESPECIALIDAD"),
                    Salario = row.Field<int>("SALARIO"),
                    IdHospital = row.Field<int>("HOSPITAL_COD")
                };
                doctores.Add(doc);
            }
            return doctores;
        }

        public async Task CreateDoctorAsync
            (int idDoctor, string apellido
            , string especialidad, int salario, int idHospital)
        {
            string sql = "insert into DOCTOR values "
                + " (:idhospital, :id, :apellido "
                + ", :especialidad, :salario)";
            //AQUI VAN LOS PARAMETROS...
            OracleParameter pamIdDoctor =
                new OracleParameter(":id", idDoctor);
            OracleParameter pamApellido =
                new OracleParameter(":apellido", apellido);
            OracleParameter pamEspe =
                new OracleParameter(":especialidad", especialidad);
            OracleParameter pamSal =
                new OracleParameter(":salario", salario);
            OracleParameter pamIdHospital =
                new OracleParameter(":idhospital", idHospital);
            this.com.Parameters.Add(pamIdHospital);
            this.com.Parameters.Add(pamIdDoctor);
            this.com.Parameters.Add(pamApellido);
            this.com.Parameters.Add(pamEspe);
            this.com.Parameters.Add(pamSal);
            
            this.com.CommandType = CommandType.Text;
            this.com.CommandText = sql;
            await this.cn.OpenAsync();
            await this.com.ExecuteNonQueryAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();
        }

        public async Task DeleteDoctorAsync(int idDoctor)
        {
            string sql = "SP_DELETE_DOCTOR";
            OracleParameter pamId =
                new OracleParameter(":p_iddoctor", idDoctor);
            this.com.Parameters.Add(pamId);
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = sql;
            await this.cn.OpenAsync();
            await this.com.ExecuteNonQueryAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();
        }

        public Doctor FindDoctor(int idDoctor)
        {
            var consulta = from datos in
                               this.tablaDoctor.AsEnumerable()
                           where datos.Field<int>("DOCTOR_NO") == idDoctor
                           select datos;
            Doctor doc = new Doctor();
            var row = consulta.First();
            doc.IdDoctor = row.Field<int>("DOCTOR_NO");
            doc.Apellido = row.Field<string>("APELLIDO");
            doc.Especialidad = row.Field<string>("ESPECIALIDAD");
            doc.Salario = row.Field<int>("SALARIO");
            doc.IdHospital = row.Field<int>("HOSPITAL_COD");
            return doc;
        }

        public async Task UpdateDoctorAsync
            (int idDoctor, string apellido, string especialidad, int salario, int idHospital)
        {
            string sql = "SP_UPDATE_DOCTOR";
            this.com.Parameters.Add(":p_iddoctor", idDoctor);
            this.com.Parameters.Add(":p_apellido", apellido);
            this.com.Parameters.Add(":p_especialidad", especialidad);
            this.com.Parameters.Add(":p_salario", salario);
            this.com.Parameters.Add(":p_idhospital", idHospital);
            this.com.CommandType = CommandType.StoredProcedure;
            this.com.CommandText = sql;
            await this.cn.OpenAsync();
            await this.com.ExecuteNonQueryAsync();
            await this.cn.CloseAsync();
            this.com.Parameters.Clear();
        }

        public List<Doctor> GetDoctoresEspecialidad(string especialidad)
        {
            var consulta = from datos in
                               this.tablaDoctor.AsEnumerable()
                           where (datos.Field<string>("ESPECIALIDAD"))
                           .ToUpper()
                           .StartsWith(especialidad.ToUpper())
                           select datos;
            List<Doctor> doctores = new List<Doctor>();
            foreach (var row in consulta)
            {
                Doctor doc = new Doctor
                {
                    IdDoctor = row.Field<int>("DOCTOR_NO"),
                    Apellido = row.Field<string>("APELLIDO"),
                    Especialidad = row.Field<string>("ESPECIALIDAD"),
                    Salario = row.Field<int>("SALARIO"),
                    IdHospital = row.Field<int>("HOSPITAL_COD")
                };
                doctores.Add(doc);
            }
            return doctores;
        }
    }
}
