using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vedligehold.Models;

namespace Vedligehold.Database
{
    public class TimeRegistrationDatabase
    {
        readonly SQLiteAsyncConnection database;

        public TimeRegistrationDatabase(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath);
            database.CreateTableAsync<TimeRegistrationModel>().Wait();
        }

  
    }
}