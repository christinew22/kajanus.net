namespace UsingCVDWithoutMap.Framework
{
    using System;
    using System.Linq;

    using Caliburn.Micro;

    using ESRI.ArcGIS.Client;
    using ESRI.ArcGIS.Client.FeatureService;

    using UsingCVDWithoutMap.FeatureService;

    public class SearchViewModelBase : ViewModelBase
    {
        protected readonly FeatureService featureService;

        private CodedValueDomain routeTypes;
        private CodedValueDomain routeLevels;

        private bool isInitialized;

        public SearchViewModelBase(IFeatureServiceManager featureServiceManager)
        {
            this.featureService = featureServiceManager.CreateOrGetFeatureService(@"http://services.arcgis.com/4PuGhqdWG1FwH2Yk/ArcGIS/rest/services/DemoCyclingRoutesFinland/FeatureServer/0");
        }

        public bool IsInitialized
        {
            get
            {
                return this.isInitialized;
            }
            set
            {
                if (value.Equals(this.isInitialized))
                {
                    return;
                }
                this.isInitialized = value;
                this.NotifyOfPropertyChange(() => this.IsInitialized);
            }
        }

        /// <summary>
        /// Gets or sets the RouteTypes that are used.
        /// </summary>
        public CodedValueDomain RouteTypes
        {
            get
            {
                return this.routeTypes;
            }
            set
            {
                if (Equals(value, this.routeTypes))
                {
                    return;
                }
                this.routeTypes = value;
                this.NotifyOfPropertyChange(() => this.RouteTypes);
            }
        }

        /// <summary>
        /// Gets or sets the RouteLevels that are used.
        /// </summary>
        public CodedValueDomain RouteLevels
        {
            get
            {
                return this.routeLevels;
            }
            set
            {
                if (Equals(value, this.routeLevels))
                {
                    return;
                }
                this.routeLevels = value;
                this.NotifyOfPropertyChange(() => this.RouteLevels);
            }
        }

        #region Lifecycle events

        /// <summary>
        /// Called when activating. Handles lazy loading.
        /// </summary>
        protected override void OnActivate()
        {
            if (!this.isInitialized)
            {
                this.Initialize();
            }

            base.OnActivate();
        }

        #endregion // Lifecycle events

        private void Initialize()
        {
            if (this.featureService.IsDomainsLoaded)
            {
                this.RouteTypes = this.featureService.GetDomainByName("RouteType");
                this.RouteLevels= this.featureService.GetDomainByName("RouteDifficulty");
            }
            else
            {
                this.featureService.Initialize();
            }
        }

        protected virtual void LoadDomainsFailed(Exception exception)
        {
            throw new NotImplementedException();
        }

        protected virtual void LoadDomainsCompleted(BindableCollection<CodedValueDomain> codedValueDomains)
        {
            // Set RouteTypes. 
            // Also add "Any" to the collection for the better search experience.
            var types = this.featureService.GetDomainByName("RouteType");
            types.CodedValues.Add(-1, "Any");
            this.RouteTypes = types;

            // Set RouteLevels.
            // Also add "Any" to the collection for the better search experience.
            var levels = this.featureService.GetDomainByName("RouteDifficulty");
            levels.CodedValues.Add(-1, "Any");
            this.RouteLevels = levels;

            this.IsInitialized = true;
        }
    }
}
