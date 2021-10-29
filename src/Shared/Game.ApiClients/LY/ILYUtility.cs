using System.Text.Json;
using TGS.Shared.Game.Models.LY.Enums;

namespace TGS.Shared.Game.ApiClients.LY
{
    public interface ILYUtility
    {
        /// <summary>
        /// Aeses the decrypt.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="key">The key.</param>
        /// <param name="isDecodeUrl">if set to <c>true</c> [is decode URL].</param>
        /// <returns></returns>
        string AesDecrypt(string data, string key, bool isDecodeUrl = true);

        /// <summary>
        /// Aeses the encrypt.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="key">The key.</param>
        /// <param name="isEncodeUrl">if set to <c>true</c> [is encode URL].</param>
        /// <returns></returns>
        string AesEncrypt(string data, string key, bool isEncodeUrl = true);

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        int? GetAction(LYAction action);

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <param name="request">The request.</param>
        /// <param name="options">The options.</param>
        /// <returns></returns>
        string GetPath<TRequest>(TRequest request, JsonSerializerOptions options = null);

        /// <summary>
        /// Hashes the m d5.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public string HashMD5(string data);
    }
}
