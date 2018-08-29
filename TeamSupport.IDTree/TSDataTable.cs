using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.Linq;

namespace TeamSupport.IDTree
{
    public class TSDataTable : IDisposable
    {
        public DataTable _table { get; private set; }
        Proxy.AuthenticationModel _authentication;
        SqlConnection _connection;
        SqlTransaction _transaction;
        SqlCommand _command;
        SqlDataAdapter _adapter;

        public TSDataTable(string query, params object[] args)
        {
            _authentication = new Proxy.AuthenticationModel();
            _connection = new SqlConnection(_authentication.ConnectionString);
            _connection.Open();
            _transaction = _connection.BeginTransaction(IsolationLevel.ReadUncommitted);
            _command = new SqlCommand()
            {
                Connection = _connection,
                Transaction = _transaction,
                CommandText = query,
                CommandType = CommandType.Text,
            };
            for (int i = 0; i < args.Length; ++i)
                _command.Parameters.AddWithValue($"t{i}", args[0]);

            _table = new DataTable();
            try
            {
                _adapter = new SqlDataAdapter(_command);
                _adapter.FillSchema(_table, SchemaType.Source);
                _adapter.Fill(_table);
                _transaction.Commit();
            }
            catch (Exception e)
            {
                _transaction.Rollback();
            }
            _connection.Close();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (_adapter != null)
                _adapter.Dispose();
            if (_command != null)
                _command.Dispose();
            if (_table != null)
                _table.Dispose();
            if (_transaction != null)
                _transaction.Dispose();
            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
            }
        }
    }

}
