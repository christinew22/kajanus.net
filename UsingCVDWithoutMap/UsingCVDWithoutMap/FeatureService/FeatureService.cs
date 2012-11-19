namespace UsingCVDWithoutMap.FeatureService
{
    using System;
    using System.Linq;

    using Caliburn.Micro;

    using ESRI.ArcGIS.Client;
    using ESRI.ArcGIS.Client.FeatureService;
    using ESRI.ArcGIS.Client.Tasks;

    /// <summary>
    /// Service for FeatureLayer.
    /// </summary>
    /// <remarks>
    /// Responsibility : Wrapper class to encapsulate <see cref="FeatureLayer"/> functionality. 
    /// Functionality  : Load and provide <see cref="CodedValueDomain"/>s from the target service and methods to access those easily. 
    ///                  Also provide methods to execute searches.
    /// </remarks>
    public class FeatureService : PropertyChangedBase
    {
        #region Variables

        private readonly FeatureLayer targetService;
        private BindableCollection<CodedValueDomain> domains;
        private bool isDomainsLoaded;

        #endregion // Variables

        #region Events

        /// <summary>
        /// Event that is raised when the domains are loaded after the <see cref="Initialize"/>.
        /// </summary>
        public event EventHandler<CodedValueDomainsLoadedEventArgs> LoadingDomainsCompleted;

        /// <summary>
        /// Event that is raised when the initialization and loading domains fails.
        /// </summary>
        public event EventHandler<FailedEventArgs> LoadingDomainsFaield;

        /// <summary>
        /// Event that is raised when the features are got from the service after the <see cref="SearchFeatures"/>.
        /// </summary>
        public event EventHandler<GraphicsEventArgs> LoadingFeaturesCompleted;

        /// <summary>
        /// Event that is raised when the search failed.
        /// </summary>
        public event EventHandler<FailedEventArgs> LoadingFeaturesFailed;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureService"/>
        /// </summary>
        /// <param name="serviceUrl">The url to the target Feature service</param>
        public FeatureService(string serviceUrl)
        {
            this.Domains = new BindableCollection<CodedValueDomain>();

            // Create feature layer that is used to internally to work with the service
            // Note the Mode definition. This is needed to work with the FeatureLayer without giving it a Map instance!
            this.targetService = new FeatureLayer
            {
                Url = serviceUrl,
                Mode = FeatureLayer.QueryMode.Snapshot
            };

            // Set initialization events
            this.targetService.Initialized += this.InitializedCompleted;
            this.targetService.InitializationFailed += this.InitializedFailed;

            // Set update events
            this.targetService.UpdateCompleted += this.UpdateCompleted;
            this.targetService.UpdateFailed += this.UpdateFailed;
        }

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the domains are loaded to the <see cref="FeatureService"/>.
        /// </summary>
        public bool IsDomainsLoaded
        {
            get
            {
                return this.isDomainsLoaded;
            }

            private set
            {
                if (value.Equals(this.isDomainsLoaded))
                {
                    return;
                }
                this.isDomainsLoaded = value;
                this.NotifyOfPropertyChange(() => this.IsDomainsLoaded);
            }
        }

        /// <summary>
        /// Gets loaded <see cref="CodedValueDomain"/>s. 
        /// </summary>
        /// <remarks>
        /// This is provided, if you want to use this directly in some source definition.
        /// </remarks>
        public BindableCollection<CodedValueDomain> Domains
        {
            get
            {
                return this.domains;
            }

            private set
            {
                if (Equals(value, this.domains))
                {
                    return;
                }
                this.domains = value;
                this.NotifyOfPropertyChange(() => this.Domains);
            }
        }

        #endregion

        /// <summary>
        /// Initialize the service and load <see cref="CodedValueDomain"/>s.
        /// </summary>
        public void Initialize()
        {
            // Service is already initialized
            if (this.targetService.IsInitialized)
            {
                if (this.IsDomainsLoaded)
                {
                    this.OnLoadingDomainsCompleted(new CodedValueDomainsLoadedEventArgs { Domains = this.Domains });                   
                }

                return;
            }

            this.targetService.Initialize();            
        }

        /// <summary>
        /// Searches features from the service
        /// </summary>
        /// <param name="whereClause"></param>
        public void SearchFeatures(string whereClause)
        {
            if (!this.targetService.IsInitialized)
            {
                throw new Exception("Layer in not initialized");
            }

            // Set query to include all fields and set Where clause
            this.targetService.OutFields = new OutFields { "*" };
            this.targetService.Where = whereClause;

            // Force update for the layer to actually execute the search
            this.targetService.Update();
        }

        /// <summary>
        /// Gets <see cref="CodedValueDomain"/> by domain name.
        /// </summary>
        /// <param name="domainName">The name of the domain</param>
        /// <returns>Return <see cref="CodedValueDomain"/> that exists in the <see cref="FeatureService.Domains"/> list. Null if doesn't.</returns>
        public CodedValueDomain GetDomainByName(string domainName)
        {
            if (this.targetService.IsInitialized == false)
            {
                throw new Exception("Layer is not initialized");
            }

            return this.Domains.FirstOrDefault(x => x.Name == domainName);
        }

        /// <summary>
        ///  Gets alias name from the <see cref="Field"/> by fields name.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        /// <returns>The alias name.</returns>
        public string GetFieldAliasName(string fieldName)
        {
            if (this.targetService.IsInitialized == false)
            {
                throw new Exception("Layer is not initialized");
            }
            
            return this.targetService.LayerInfo.Fields.FirstOrDefault(x => x.FieldName == fieldName).Alias;
        }

        #region On[Stuff] 

        protected virtual void OnLoadingDomainsCompleted(CodedValueDomainsLoadedEventArgs e)
        {
            EventHandler<CodedValueDomainsLoadedEventArgs> handler = this.LoadingDomainsCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnLoadingDomainsFaield(FailedEventArgs e)
        {
            EventHandler<FailedEventArgs> handler = this.LoadingDomainsFaield;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnLoadingFeaturesCompleted(GraphicsEventArgs e)
        {
            EventHandler<GraphicsEventArgs> handler = this.LoadingFeaturesCompleted;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnLoadingFeaturesFailed(FailedEventArgs e)
        {
            EventHandler<FailedEventArgs> handler = this.LoadingFeaturesFailed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion // On[Stuff]

        #region Private handlers

        /// <summary>
        /// Executes completed event when the Update is returned.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateCompleted(object sender, EventArgs e)
        {
            this.OnLoadingFeaturesCompleted(new GraphicsEventArgs { Graphics = this.targetService.Graphics });
        }

        /// <summary>
        /// Executes failed event when the Update didn't succeed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateFailed(object sender, TaskFailedEventArgs e)
        {
            this.OnLoadingFeaturesFailed(new FailedEventArgs{ Error = e.Error });
        }

        /// <summary>
        ///  <see cref="FeatureLayer"/> initialization completed. Also also raises domains loaded event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitializedCompleted(object sender, EventArgs e)
        {
            // Get all fields that contains CVD's
            var fields = this.targetService.LayerInfo.Fields.Where(x => x.Domain != null);

            // Load all CVD's to domains list
            foreach (var domain in fields.Select(field => field.Domain).OfType<CodedValueDomain>())
            {
                this.domains.Add(domain);
            }

            this.IsDomainsLoaded = true;

            // Raise domains loaded event
            this.OnLoadingDomainsCompleted(new CodedValueDomainsLoadedEventArgs { Domains = this.Domains });
        }

        /// <summary>
        /// Executes failed event when the <see cref="FeatureLayer"/> initializagion failed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitializedFailed(object sender, EventArgs e)
        {
            // Getting initialization error and delivering it forward
            var layer = sender as FeatureLayer;

            // Raise failed load domains event
            this.OnLoadingDomainsFaield(new FailedEventArgs { Error = layer.InitializationFailure });
        }

        #endregion // Private handlers
    }
}
