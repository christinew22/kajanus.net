namespace AGOLWebMapInClient
{
    using ESRI.ArcGIS.Client;
    using ESRI.ArcGIS.Client.WebMap;

    using Knet.Core;

    public class MainViewModel : NotificationBase 
    {
        private LayerCollection layers;

        public MainViewModel()
        {
            // WebMap used is the same that we created in the Esri Code Camp.
            this.LoadWebMap("1e7d9301457943b08034cb89abea905b");
        }

        /// <summary>
        /// Gets or sets the layers for the Map.
        /// </summary>
        public LayerCollection Layers
        {
            get
            {
                return this.layers;
            }
            set
            {
                if (this.layers == value)
                {
                    return;
                }
                this.layers = value;
                this.NotifyPropertyChanged(() => this.Layers);
            }
        }

        private void LoadWebMap(string id)
        {
            var webMap = new Document();
            webMap.GetMapCompleted += (s, e) =>
            {
                //MyMap.Extent = e.Map.Extent;

                var layerCollection = new LayerCollection();
                foreach (var layer in e.Map.Layers)
                {
                    layerCollection.Add(layer);
                }

                // Remove reference from webmap layers collection
                e.Map.Layers.Clear();

                // Set layers for map in the UI.
                this.Layers = layerCollection;
            };

            webMap.GetMapAsync(id);

        }
    }
}
