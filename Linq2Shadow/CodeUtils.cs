using System.Threading;
using System.Threading.Tasks;

namespace Linq2Shadow
{
    internal static class CodeUtils
    {
        internal static Task<T> FromCancelledTask<T>(CancellationToken cancellationToken)
        {
#if NETSTANDARD

            return Task.FromCanceled<T>(cancellationToken);
#else
            var tcs = new TaskCompletionSource<T>();
            CancellationTokenRegistration? ctr = null;
            ctr = cancellationToken.Register(() =>
            {
                tcs.SetCanceled();
                ctr.Value.Dispose();
            });

            return tcs.Task;
#endif
        }

        internal static Task FromCancelledTask(CancellationToken cancellationToken)
        {
            return FromCancelledTask<object>(cancellationToken);
        }
    }
}
