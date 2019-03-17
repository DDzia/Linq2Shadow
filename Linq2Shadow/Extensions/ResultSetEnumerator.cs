using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;

namespace Linq2Shadow.Extensions
{
    internal class ResultSetEnumerator: IEnumerator<ShadowRow>
    {
        private ShadowRow _current = null;
        public ShadowRow Current
        {
            get
            {
                ThrowIfDisposed();
                return _current;
            }
            private set => _current = value;
        }
        object IEnumerator.Current => Current;

        private readonly DbDataReader _reader;
        private readonly Action _disposeCb;

        private bool _disposed;

        internal ResultSetEnumerator(DbDataReader reader, Action disposeCallback = null)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
            _disposeCb = disposeCallback;
        }

        public void Reset() => throw new NotSupportedException("Reset operation is not supported for one-way iteration.");

        public bool MoveNext()
        {
            ThrowIfDisposed();

            var hasAlso = _reader.Read();

            _current = hasAlso
                ? _reader.FillRow()
                : null;

            return hasAlso;
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposeCb?.Invoke();

            _disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException($"'{nameof(ResultSetEnumerator)}' enumerator is disposed.");
        }
    }
}
