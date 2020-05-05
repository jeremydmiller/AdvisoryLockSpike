using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace AdvisoryLockSpike.Election
{
    public static class AdvisoryLockExtensions
    {
        // Global lock with transaction scoping, this would block
        public static Task GetGlobalTxLock(this DbConnection conn, DbTransaction tx, int lockId,
            CancellationToken cancellation = default(CancellationToken))
        {
            return tx.CreateCommand("SELECT pg_advisory_xact_lock(:id);").With("id", lockId)
                .ExecuteNonQueryAsync(cancellation);
        }

        // Try to get a global lock with transaction scoping
        public static async Task<bool> TryGetGlobalTxLock(this DbConnection conn, DbTransaction tx, int lockId,
            CancellationToken cancellation = default(CancellationToken))
        {
            var c = await tx.CreateCommand("SELECT pg_try_advisory_xact_lock(:id);")
                .With("id", lockId)
                .ExecuteScalarAsync(cancellation);

            return (bool) c;
        }

        // Global lock with connection scoping. This would block
        public static Task GetGlobalLock(this DbConnection conn, int lockId, CancellationToken cancellation = default(CancellationToken),
            DbTransaction transaction = null)
        {
            return conn.CreateCommand("SELECT pg_advisory_lock(:id);").With("id", lockId)
                .ExecuteNonQueryAsync(cancellation);
        }

        // Try to get a global lock with connection scoping
        public static async Task<bool> TryGetGlobalLock(this DbConnection conn, int lockId, CancellationToken cancellation = default(CancellationToken))
        {
            var c = await conn.CreateCommand("SELECT pg_try_advisory_lock(:id);")
                .With("id", lockId)
                .ExecuteScalarAsync(cancellation);

            return (bool) c;
        }

        // Release a held global lock
        public static Task ReleaseGlobalLock(this DbConnection conn, int lockId, CancellationToken cancellation = default(CancellationToken),
            DbTransaction tx = null)
        {
            return tx.CreateCommand("SELECT pg_advisory_unlock(:id);").With("id", lockId)
                .ExecuteNonQueryAsync(cancellation);
        }


    }
}