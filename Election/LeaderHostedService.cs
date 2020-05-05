using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Npgsql;

namespace AdvisoryLockSpike.Election
{
    public class LeaderHostedService<T> : BackgroundService
        where T : IActiveProcess
    {
        private readonly LeaderSettings<T> _settings;
        private readonly T _process;
        private NpgsqlConnection _connection;

        public LeaderHostedService(LeaderSettings<T> settings, T process)
        {
            _settings = settings;
            _process = process;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Don't try to start right off the bat
            await Task.Delay(_settings.FirstPollingTime, stoppingToken);
                
            _connection = new NpgsqlConnection(_settings.ConnectionString);
            await _connection.OpenAsync(stoppingToken);
            
            while (!stoppingToken.IsCancellationRequested)
            {
                var state = await _process.State();
                if (state != ProcessState.Active)
                {
                    // If you can take the global lock, start
                    // the process
                    if (await _connection.TryGetGlobalLock(_settings.LockId, cancellation: stoppingToken))
                    {
                        await _process.Start(_connection);
                    }
                }

                await Task.Delay(_settings.OwnershipPollingTime, stoppingToken);
            }

            if (_connection.State != ConnectionState.Closed)
            {
                await _connection.DisposeAsync();
            }

        }
    }
}