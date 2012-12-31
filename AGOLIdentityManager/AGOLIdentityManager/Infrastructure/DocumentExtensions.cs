namespace AGOLIdentityManager.Infrastructure
{
    using System.Threading.Tasks;

    using ESRI.ArcGIS.Client;
    using ESRI.ArcGIS.Client.WebMap;

    public static class DocumentExtensions
    {
        /// <summary>
        /// Loads WebMap by given item Id asynchronously. Set credentials before calling this if target is a secured item.
        /// </summary>
        /// <param name="document">The Document used.</param>
        /// <param name="webmapId">The WebMap id that is loaded.</param>
        /// <returns>Returns <see cref="Map"/> as a <see cref="Task{T}"/></returns>
        public static Task<Map> LoadMapAsync(this Document document, string webmapId)
        {
            var tcs = new TaskCompletionSource<Map>();

            document.GetMapCompleted += (sender, e) =>
            {
                if (e.Error != null)
                {
                    tcs.SetException(e.Error);
                    return;
                }

                tcs.SetResult(e.Map);
            };

            document.GetMapAsync(webmapId);
            return tcs.Task;
        } 
    }
}
