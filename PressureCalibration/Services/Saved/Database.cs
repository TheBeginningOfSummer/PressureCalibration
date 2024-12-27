using CSharpKit.FileManagement;
using SQLite;
using System.Collections;

namespace Calibration.Services
{
    public class Database : ParameterManager
    {
        public string DatabasePath { get; set; } = "Data";
        public string DatabaseName { get; set; } = "Data.db";

        public SQLiteAsyncConnection Connection;

        public Database()
        {
            if (!Directory.Exists(DatabasePath)) Directory.CreateDirectory(DatabasePath);
            string path = Path.Combine(DatabasePath, DatabaseName);
            Connection ??= new SQLiteAsyncConnection(path);
        }

        public override void InitializeParameter()
        {
            if (!Directory.Exists($"{Environment.CurrentDirectory}\\{DatabasePath}"))
                Directory.CreateDirectory($"{Environment.CurrentDirectory}\\{DatabasePath}");
            string path = Path.Combine($"{Environment.CurrentDirectory}\\{DatabasePath}", DatabaseName);
            Connection = new SQLiteAsyncConnection(path);
        }

        public async void CreateTable<T>() where T : new()
        {
            await Connection.CreateTableAsync<T>();
        }

        public async Task AddDataAsync<T>(T data) where T : new()
        {
            try
            {
                await Connection.InsertAsync(data);
            }
            catch (Exception)
            {
                await Connection.CreateTableAsync<T>();
                await Connection.InsertAsync(data);
            }
        }

        public async Task AddAllDataAsync<T>(IEnumerable objects) where T : new()
        {
            try
            {
                await Connection.InsertAllAsync(objects);
            }
            catch (Exception)
            {
                await Connection.CreateTableAsync<T>();
                await Connection.InsertAllAsync(objects);
            }
        }

        public async Task DeleteDataAsync<T>(T data)
        {
            await Connection.DeleteAsync(data);
        }

        public async Task DeleteAllDataAsync<T>()
        {
            await Connection.DeleteAllAsync<T>();
        }

        public async Task<T> GetDataAsync<T>(object pk) where T : new()
        {
            try
            {
                return await Connection.GetAsync<T>(pk);
            }
            catch (Exception)
            {
                await Connection.CreateTableAsync<T>();
                return await Connection.GetAsync<T>(pk);
            }
        }

        public async Task<IList<T>> GetDataListAsync<T>() where T : new()
        {
            try
            {
                return await Connection.Table<T>().ToListAsync();
            }
            catch (Exception)
            {
                await Connection.CreateTableAsync<T>();
                return await Connection.Table<T>().ToListAsync();
            }
        }

        public async Task UpdateDataAsync<T>(T data)
        {
            await Connection.UpdateAsync(data);
        }


    }
}
