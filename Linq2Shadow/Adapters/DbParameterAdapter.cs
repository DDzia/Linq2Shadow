using System;
using System.Data;
using System.Data.Common;

namespace Linq2Shadow.Adapters
{
    internal sealed class DbParameterAdapter : DbParameter
    {
        private readonly IDbDataParameter _p;

        internal DbParameterAdapter(IDbDataParameter parameter)
        {
            _p = parameter ?? throw new ArgumentNullException(nameof(parameter));
        }

        public override void ResetDbType() => throw new NotSupportedException(nameof(ResetDbType));

        public override DbType DbType
        {
            get => _p.DbType;
            set => _p.DbType = value;
        }

        public override ParameterDirection Direction
        {
            get => throw new NotSupportedException(nameof(Direction));
            set => throw new NotSupportedException(nameof(Direction));
        }

        public override bool IsNullable
        {
            get => _p.IsNullable;
            set => throw new NotSupportedException(nameof(IsNullable) + " is readonly.");
        }

        public override string ParameterName
        {
            get => _p.ParameterName;
            set => _p.ParameterName = value;
        }

        public override string SourceColumn
        {
            get => _p.SourceColumn;
            set => _p.SourceColumn = value;
        }

        public override DataRowVersion SourceVersion
        {
            get => _p.SourceVersion;
            set => _p.SourceVersion = value;
        }

        public override object Value
        {
            get => _p.Value;
            set => _p.Value = value;
        }

        public override bool SourceColumnNullMapping
        {
            get => throw new NotSupportedException(nameof(SourceColumnNullMapping));
            set => throw new NotSupportedException(nameof(SourceColumnNullMapping));
        }

        public override int Size
        {
            get => _p.Size;
            set => _p.Size = value;
        }
    }
}
