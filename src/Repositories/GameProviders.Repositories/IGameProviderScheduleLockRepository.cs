using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories.Models.GameProviderScheduleLock;

namespace TGS.GameProviders.Repositories
{
    public interface IGameProviderScheduleLockRepository
    {
        /// <summary>
        /// Determines whether [is lock asynchronous] [the specified game provider code].
        /// </summary>
        /// <param name="gameProviderCode">The game provider code.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public ValueTask<bool> IsLockAsync(string gameProviderCode, string key);
        /// <summary>
        /// Sets the asynchronous.
        /// </summary>
        /// <param name="gameProviderCode">The game provider code.</param>
        /// <param name="key">The key.</param>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public ValueTask<bool> SetAsync(string gameProviderCode, string key, GameProviderScheduleLockData date);
        /// <summary>
        /// Removes the asynchronous.
        /// </summary>
        /// <param name="gameProviderCode">The game provider code.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public ValueTask<bool> RemoveAsync(string gameProviderCode, string key);
        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="gameProviderCode">The game provider code.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public ValueTask<GameProviderScheduleLockData> GetAsync(string gameProviderCode, string key);

        /// <summary>
        /// Removes the game provider code asynchronous.
        /// </summary>
        /// <param name="gameProviderCode">The game provider code.</param>
        /// <returns></returns>
        public ValueTask RemoveGameProviderCodeAsync(string gameProviderCode);
    }
}
