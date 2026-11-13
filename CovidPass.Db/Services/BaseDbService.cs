// © 2020 Partners HealthCare Technology & Innovation Department
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CovidPass.Common.Utils;
using Dapper;

namespace CovidPass.Db.Services
{
    public interface IBaseDbService : IDisposable
    {
        // Transactions
        IDisposable BeginTransaction(IsolationLevel isolationLevel);
        void Commit();
        void Rollback();
        void EndTransaction();
        //
        Task<List<T>> SelectMany<T>(string sql, object param = null);
        Task<T> SelectOne<T>(string sql, object param = null);
        Task<int> Execute(string sql, object param = null);
        Task<T> ExecuteScalar<T>(string sql, object param = null);
    }
    public class BaseDbService : IBaseDbService
    {
        private readonly IDbConnection _connection;
        private IDbTransaction _transaction;

        public BaseDbService(IDbConnection connection)
        {
            _connection = connection;
        }

        private IDbConnection GetConnection() => _transaction?.Connection ?? _connection;

        public IDisposable BeginTransaction(IsolationLevel isolationLevel)
        {
            if (_transaction != null)
                throw new Exception("Transaction already started");
            _transaction = _connection.BeginTransaction(isolationLevel);
            return new Disposable(EndTransaction);
        }

        public void Commit()
        {
            if (_transaction == null)
                throw new Exception("Transaction isn't started");
            _transaction.Commit();
        }

        public void Rollback()
        {
            if (_transaction == null)
                throw new Exception("Transaction isn't started");
            _transaction.Rollback();
        }

        public void EndTransaction()
        {
            if (_transaction == null)
                throw new Exception("Transaction isn't started");
            _transaction.Dispose();
            _transaction.Connection.Dispose();
            _transaction = null;
        }

        public async Task<List<T>> SelectMany<T>(string sql, object param = null) => (await GetConnection().QueryAsync<T>(sql, param)).ToList();

        public async Task<T> SelectOne<T>(string sql, object param = null) => await GetConnection().QueryFirstOrDefaultAsync<T>(sql, param);

        public async Task<int> Execute(string sql, object param = null) => await GetConnection().ExecuteAsync(sql, param);

        public async Task<T> ExecuteScalar<T>(string sql, object param = null) => await GetConnection().ExecuteScalarAsync<T>(sql, param);

        public void Dispose()
        {
            GetConnection().Dispose();
        }
    }
}
