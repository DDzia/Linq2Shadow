using System;
using System.Data;
using System.Data.Common;

namespace Linq2Shadow.Adapters
{
    internal sealed class DbTransactionAdapter: DbTransaction
    {
        private readonly IDbTransaction _transaction;

        internal DbTransactionAdapter(IDbTransaction transaction, DbConnection connection)
        {
            _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
            DbConnection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public override void Commit() => _transaction.Commit();

        public override void Rollback() => _transaction.Rollback();

        protected override DbConnection DbConnection { get; }

        public override IsolationLevel IsolationLevel => _transaction.IsolationLevel;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _transaction.Dispose();
            }

            base.Dispose(disposing);
        }

        ~DbTransactionAdapter()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }
    }
}
