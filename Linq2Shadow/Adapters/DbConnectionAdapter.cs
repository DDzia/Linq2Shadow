using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Linq2Shadow.Adapters
{
    internal sealed class DbConnectionAdapter: DbConnection
    {
        internal static DbConnection Adapte(IDbConnection sourceConnection)
        {
            if(sourceConnection is null)
                throw new ArgumentNullException(nameof(sourceConnection));

            return sourceConnection is DbConnection adaptable
                ? adaptable
                : new DbConnectionAdapter(sourceConnection);
        }

        private readonly IDbConnection _conn;
        private DbConnectionAdapter(IDbConnection connection)
        {
            _conn = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) =>
            new DbTransactionAdapter(_conn.BeginTransaction(isolationLevel), this);

        public override void ChangeDatabase(string databaseName) =>_conn.ChangeDatabase(databaseName);

        public override void Close() => _conn.Close();

        public override void Open() => _conn.Open();

        public override string ConnectionString
        {
            get => _conn.ConnectionString;
            set => _conn.ConnectionString = value;
        }

        public override string Database => _conn.Database;

        public override ConnectionState State => _conn.State;

        public override string DataSource => _conn.Database;

        public override string ServerVersion => string.Empty;

        protected override DbCommand CreateDbCommand() => new DbCommandAdapter(_conn.CreateCommand());

        #region async

        // All async call is synchronous really.
        // For more details see DbConnection implementation: https://referencesource.microsoft.com/#System.Data/System/Data/Common/DBConnection.cs,5f329ec84852e62f

        public override Task OpenAsync(CancellationToken cancellationToken) =>
            Task.Run(() => Open(), cancellationToken);

        #endregion async

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _conn.Dispose();
            }

            base.Dispose(disposing);
        }

        ~DbConnectionAdapter()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }
    }
}
