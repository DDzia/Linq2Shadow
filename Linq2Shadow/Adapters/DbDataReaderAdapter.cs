using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Linq2Shadow.Adapters
{
    internal class DbDataReaderAdapter : DbDataReader
    {
        private readonly IDataReader _reader;

        public DbDataReaderAdapter(IDataReader dataReader)
        {
            _reader = dataReader ?? throw new ArgumentNullException(nameof(dataReader));
        }

        public override DataTable GetSchemaTable() => _reader.GetSchemaTable();

        public override bool GetBoolean(int ordinal) => _reader.GetBoolean(ordinal);

        public override byte GetByte(int ordinal) => _reader.GetByte(ordinal);

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length) =>
            _reader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);

        public override char GetChar(int ordinal) => _reader.GetChar(ordinal);

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length) =>
            _reader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);

        public override string GetDataTypeName(int ordinal) => _reader.GetDataTypeName(ordinal);

        public override DateTime GetDateTime(int ordinal) => _reader.GetDateTime(ordinal);

        public override decimal GetDecimal(int ordinal) => _reader.GetDecimal(ordinal);

        public override double GetDouble(int ordinal) => _reader.GetDouble(ordinal);

        public override Type GetFieldType(int ordinal) => _reader.GetFieldType(ordinal);

        public override float GetFloat(int ordinal) => _reader.GetFloat(ordinal);

        public override Guid GetGuid(int ordinal) => _reader.GetGuid(ordinal);

        public override short GetInt16(int ordinal) => _reader.GetInt16(ordinal);

        public override int GetInt32(int ordinal) => _reader.GetInt32(ordinal);

        public override long GetInt64(int ordinal) => _reader.GetInt64(ordinal);

        public override string GetName(int ordinal) => _reader.GetName(ordinal);

        public override int GetOrdinal(string name) => _reader.GetOrdinal(name);

        public override string GetString(int ordinal) => _reader.GetString(ordinal);

        public override object GetValue(int ordinal) => _reader.GetValue(ordinal);

        public override int GetValues(object[] values) => _reader.GetValues(values);

        public override bool IsDBNull(int ordinal) => _reader.IsDBNull(ordinal);

        public override int FieldCount => _reader.FieldCount;

        public override object this[int ordinal] => _reader[ordinal];

        public override object this[string name] => _reader[name];

        public override int RecordsAffected => _reader.RecordsAffected;
        public override bool HasRows => true;
        public override bool IsClosed => _reader.IsClosed;

        public override void Close() => _reader.Close();

        public override bool NextResult() => _reader.NextResult();

        public override bool Read() => _reader.Read();

        public override int Depth => _reader.Depth;

        public override IEnumerator GetEnumerator() => new DbReaderEnumerator(this);

        #region async

        // All async call is synchronous really.
        // For more details see DbDataReader implementation: https://referencesource.microsoft.com/#System.Data/System/Data/Common/DbDataReader.cs,f7c2de36229de361

        public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken) =>
            Task.Run(() => GetFieldValue<T>(ordinal), cancellationToken);

        public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken) =>
            Task.Run(() => IsDBNull(ordinal), cancellationToken);

        public override Task<bool> ReadAsync(CancellationToken cancellationToken) =>
            Task.Run(() => Read(), cancellationToken);

        public override Task<bool> NextResultAsync(CancellationToken cancellationToken) =>
            Task.Run(() => NextResult(), cancellationToken);

        #endregion async

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _reader.Dispose();
            }

            base.Dispose(disposing);
        }

        ~DbDataReaderAdapter()
        {
            Dispose(false);
            GC.SuppressFinalize(this);
        }

        private class DbReaderEnumerator : IEnumerator
        {
            private readonly DbDataReaderAdapter _reader;

            internal DbReaderEnumerator(DbDataReaderAdapter reader)
            {
                _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            }

            public bool MoveNext()
            {
                var res = _reader.Read();

                if (res)
                {
                    Current = _reader;
                }

                return res;
            }

            public void Reset() => throw new NotSupportedException();

            public object Current { get; private set; } = null;
        }
    }
}
