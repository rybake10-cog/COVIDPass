// © 2020 Partners HealthCare Technology & Innovation Department
using System;
using System.Data;
using System.Threading.Tasks;
using CovidPass.Common.Constants;
using CovidPass.Db.Services;
using CovidPass.Common.Utils;

namespace CovidPass.Db
{
    public interface IDatabase : IDisposable
    {
        Task<int> InsertApplication(string lastname, string firstname, string login, string employeeid,
               string email, string site, string initials, bool nosymptoms, bool influenza, bool fever, bool cough, bool shortnessofbreath,
               bool sorethroat, bool diarrheanausea, bool runnynose, bool muscleache, bool newlossofsmell, string language, string createdby, DateTime created);
    }

    public class Database : BaseDbService, IDatabase
    {
        public Database(IDbConnection connection) : base(connection)
        {
        }

        public Task<int> InsertApplication(string lastname, string firstname, string login, string employeeid,
            string email, string site, string initials, bool nosymptoms, bool influenza, bool fever, bool cough, bool shortnessofbreath,
            bool sorethroat, bool diarrheanausea, bool runnynose, bool muscleache, bool newlossofsmell, string language, string createdby, DateTime created)
        {
            return Execute(
                $@"begin transaction;
                insert into {DbConstants.SchemaName}.{DbConstants.ApplicationTable}
                    (lastname,firstname,login,employeeid,email,nosymptoms,influenza,fever,cough,shortnessofbreath,sorethroat,diarrheanausea,runnynose, muscleache, newlossofsmell, language, createdby, created, site, initials)
                    values (@lastname,@firstname,@login,@employeeid,@email,@nosymptoms,@influenza,@fever,@cough,@shortnessofbreath,@sorethroat,@diarrheanausea, @runnyNose, @muscleAche, @newLossOfSmell, @language, @createdby, @created, @site, @initials);
                commit;", new
                {
                    lastname,
                    firstname,
                    login,
                    employeeid,
                    email,
                    nosymptoms,
                    influenza,
                    fever,
                    cough,
                    shortnessofbreath,
                    sorethroat,
                    diarrheanausea,
                    runnynose,
                    muscleache,
                    newlossofsmell,
                    createdby,
                    created,
                    site,
                    initials,
                    language
                });
        }
    }
}
