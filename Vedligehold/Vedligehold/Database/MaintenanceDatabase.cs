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
    }
}
