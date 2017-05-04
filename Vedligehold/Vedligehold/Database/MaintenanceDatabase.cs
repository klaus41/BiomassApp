using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;

namespace Vedligehold.Database
{
    public class MaintenanceDatabase
    {
        readonly SQLiteAsyncConnection database;

        public MaintenanceDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<MaintenanceTask>().Wait();
            database.CreateTableAsync<TimeRegistrationModel>().Wait();
            database.CreateTableAsync<MaintenanceActivity>().Wait();
            database.CreateTableAsync<ConnectionSettings>().Wait();
            database.CreateTableAsync<PictureModel>().Wait();
            database.CreateTableAsync<JobRecLine>().Wait();
        }

        public Task<List<MaintenanceTask>> GetTasksAsync()
        {
            return database.Table<MaintenanceTask>().ToListAsync();
        }

        public Task<MaintenanceTask> GetTaskAsync(int id)
        {
            return database.Table<MaintenanceTask>().Where(i => i.no == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveTaskAsync(MaintenanceTask task)
        {
            return database.InsertAsync(task);
        }

        public Task<int> UpdateTaskAsync(MaintenanceTask task)
        {
            return database.UpdateAsync(task);
        }
        public Task<int> DeleteTaskAsync(MaintenanceTask task)
        {
            return database.DeleteAsync(task);
        }

        public Task<int> DeleteAll()
        {
            return database.ExecuteAsync("delete from " + "MaintenanceTask");
        }
        public Task<List<TimeRegistrationModel>> GetTimeRegsAsync()
        {
            return database.Table<TimeRegistrationModel>().ToListAsync();
        }

        public Task<TimeRegistrationModel> GetTimeRegAsync(int id)
        {
            return database.Table<TimeRegistrationModel>().Where(i => i.No == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveTimeRegAsync(TimeRegistrationModel task)
        {
            return database.InsertAsync(task);
        }

        public Task<int> UpdateTimeRegAsync(TimeRegistrationModel task)
        {
            return database.UpdateAsync(task);
        }
        public Task<int> DeleteTimeRegAsync(TimeRegistrationModel task)
        {
            return database.DeleteAsync(task);
        }

        public Task<int> DeleteAllTimeReg()
        {
            return database.ExecuteAsync("delete from " + "TimeRegistrationModel");
        }

        public Task<List<MaintenanceActivity>> GetAcitivitiesAsync()
        {
            return database.Table<MaintenanceActivity>().ToListAsync();
        }

        public Task<MaintenanceActivity> GetAcitivitiyAsync(int id)
        {
            return database.Table<MaintenanceActivity>().Where(i => i.Maint_Activity_No == id).FirstOrDefaultAsync();
        }
        
        public Task<int> UpdateActivityAsync(MaintenanceActivity act)
        {
            // return database.ExecuteAsync("Update MaintenanceActivity SET done=true WHERE Maint_Activity_No=" + act.Maint_Activity_No);
            //database.ExecuteAsync("delete from MaintenanceActivity WHERE Maint_Activity_No =" + act.Maint_Activity_No);
            //return database.InsertAsync(act);
            return database.UpdateAsync(act);
        }
        public Task<int> SaveActivityASync(MaintenanceActivity task)
        {
            return database.InsertAsync(task);
        }

        public Task<int> DeleteAllActivities()
        {
            return database.ExecuteAsync("delete from " + "MaintenanceActivity");
        }
        public Task<int> DeleteActivity(MaintenanceActivity act)
        {
            return database.DeleteAsync(act);
        }
        public Task<ConnectionSettings> GetConnectionSetting(int id)
        {
            return database.Table<ConnectionSettings>().Where(i => i.ID == id).FirstOrDefaultAsync();
        }

        public Task<int> UpdateConnectionSetting(ConnectionSettings act)
        {
            return database.UpdateAsync(act);
        }
        public Task<int> SaveConnectionSetting(ConnectionSettings task)
        {
            return database.InsertAsync(task);
        }
        public Task<List<PictureModel>> GetPicturesAsync()
        {
            return database.Table<PictureModel>().ToListAsync();
        }

        public Task<PictureModel> GetPictureAsync(string id)
        {
            return database.Table<PictureModel>().Where(i => i.UniqueID == id).FirstOrDefaultAsync();
        }

        public Task<int> SavePictureAsync(PictureModel task)
        {
            return database.InsertAsync(task);
        }

        public Task<int> UpdatePictureAsync(PictureModel task)
        {
            return database.UpdateAsync(task);
        }
        public Task<int> DeletePictureAsync(PictureModel task)
        {
            return database.DeleteAsync(task);
        }

        public Task<int> DeletePictureAsync()
        {
            return database.ExecuteAsync("delete from " + "PictureModel");
        }
        public Task<List<JobRecLine>> GetJobRecLinesAsync()
        {
            return database.Table<JobRecLine>().ToListAsync();
        }

        public Task<JobRecLine> GetJobRecLineAsync(Guid id)
        {
            return database.Table<JobRecLine>().Where(i => i.JobRecLineGUID == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveJobRecLineAsync(JobRecLine task)
        {
            return database.InsertAsync(task);
        }

        public Task<int> UpdateJobRecLineAsync(JobRecLine task)
        {
            return database.UpdateAsync(task);
            //return database.ExecuteAsync("UPDATE JobRecLine SET Quantity = 99");

            //return database.ExecuteAsync("UPDATE JobRecLine SET Quantity = 3666 WHERE JobRecLineGUID = " + task.JobRecLineGUID);
        }
        public Task<int> DeleteJobRecLineAsync(JobRecLine task)
        {
            return database.DeleteAsync(task);
        }

        public Task<int> DeleteJobRecLinesAsync()
        {
            return database.ExecuteAsync("delete from " + "JobRecLine");
        }
    }
}
