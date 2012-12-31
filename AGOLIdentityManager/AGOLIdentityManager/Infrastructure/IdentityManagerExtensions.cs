namespace AGOLIdentityManager.Infrastructure
{
    using System.Threading.Tasks;

    using ESRI.ArcGIS.Client;

    public static class IdentityManagerExtensions
    {
        /// <summary>
        /// Default url to ArcGIS Online REST API endpoint.
        /// </summary>
        public const string ArcGisOnlineRestUrl = "http://www.arcgis.com/sharing/rest/";

        /// <summary>
        /// Generates credentials for the user to the ArcGIS Online.
        /// </summary>
        /// <param name="identityManager">IdentityManager used.</param>
        /// <param name="username">The Username.</param>
        /// <param name="password">The password.</param>
        /// <returns>Returns <see cref="IdentityManager.Credential"/> as a <see cref="Task{T}"/></returns>
        public static Task<IdentityManager.Credential> GetCredentialsAsync(this IdentityManager identityManager, string username, string password)
        {
            var tcs = new TaskCompletionSource<IdentityManager.Credential>();

            IdentityManager.Current.GenerateCredentialAsync(
                ArcGisOnlineRestUrl,
                username,
                password,
                (credential, e) =>
                    {
                        if (e != null)
                        {
                            tcs.SetException(e);
                            return;
                        }

                        tcs.SetResult(credential);
                    });

            return tcs.Task;
        }
    }
}

