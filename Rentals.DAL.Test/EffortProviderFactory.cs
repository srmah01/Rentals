using System.Data.Common;
using System.Data.Entity.Infrastructure;

namespace Rentals.DAL.Tests
{
    /// <summary>
    /// Reperesents the Factory to create the Effort DB Provider.
    /// </summary>
    public class EffortProviderFactory : IDbConnectionFactory
    {
        private static DbConnection _connection;
        private readonly static object _lock = new object();

        /// <summary>
        /// Resets the database connection.
        /// </summary>
        public static void ResetDb()
        {
            lock (_lock)
            {
                _connection = null;
            }
        }

        /// <summary>
        /// Create a new connection to the named database.
        /// </summary>
        /// <param name="nameOrConnectionString"></param>
        /// <returns>A new DbConnection object.</returns>
        public DbConnection CreateConnection(string nameOrConnectionString)
        {
            lock (_lock)
            {
                if (_connection == null)
                {
                    _connection = Effort.DbConnectionFactory.CreateTransient();
                }

                return _connection;
            }
        }
    }
}
