using System;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Linq2Shadow.Adapters
{
    internal sealed class DbCommandAdapter : DbCommand
    {
        private readonly IDbCommand _cmd;
        private DbConnection _conn;

        internal DbCommandAdapter(IDbCommand cmd, DbConnection conn = null)
        {
            _cmd = cmd ?? throw new ArgumentNullException(nameof(cmd));
            _conn = conn;
        }

        public override void Cancel() => _cmd.Cancel();

        public override int ExecuteNonQuery() => _cmd.ExecuteNonQuery();

        public override object ExecuteScalar() => _cmd.ExecuteScalar();

        public override void Prepare() => _cmd.Prepare();

        public override string CommandText
        {
            get => _cmd.CommandText;
            set => _cmd.CommandText = value;
        }

        public override int CommandTimeout
        {
            get => _cmd.CommandTimeout;
            set => _cmd.CommandTimeout = value;
        }

        public override CommandType CommandType
        {
            get => _cmd.CommandType;
            set => _cmd.CommandType = value;
        }

        public override UpdateRowSource UpdatedRowSource { get; set; }

        protected override DbConnection DbConnection
        {
            get => _conn;
            set
            {
                _conn = value;
                _cmd.Connection = value;
            }
        }

        protected override DbParameterCollection DbParameterCollection { get; } = new DbParamsCollectionImpl();

        protected override DbTransaction DbTransaction
        {
            get => new DbTransactionAdapter(_cmd.Transaction, _conn);
            set => _cmd.Transaction = value;
        }

        [Browsable(false)]
        public override bool DesignTimeVisible { get; set; }

        protected override DbParameter CreateDbParameter() => new DbParameterAdapter(_cmd.CreateParameter());

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => new DbDataReaderAdapter(_cmd.ExecuteReader(behavior));

        #region async

        // All async call is synchronous really.
        // For more details see DbComand implementation: https://referencesource.microsoft.com/#System.Data/System/Data/Common/DBCommand.cs

        protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken) =>
            Task.Run(() => ExecuteReader(behavior), cancellationToken);

        public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken) =>
            Task.Run(() => ExecuteNonQuery(), cancellationToken);

        public override Task<object> ExecuteScalarAsync(CancellationToken cancellationToken) =>
            Task.Run(() => ExecuteScalar(), cancellationToken);

        #endregion async

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cmd.Dispose();
            }

            base.Dispose(disposing);
        }

        ~DbCommandAdapter()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }
    }
}
