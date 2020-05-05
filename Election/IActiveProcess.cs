using System;
using System.Threading.Tasks;
using Npgsql;

namespace AdvisoryLockSpike.Election
{
    public interface IActiveProcess : IDisposable
    {
        Task<ProcessState> State();
        
        // The way I've done this before, the
        // running code does all its work using
        // the currently open connection or at
        // least checks the connection to "know"
        // that it still has the leadership role
        Task Start(NpgsqlConnection conn);
    }
}