namespace UsingCVDWithoutMap.Services.CodedValueDomainService
{
    using System;
    using System.Linq;

    using Caliburn.Micro;

    using ESRI.ArcGIS.Client;
    using ESRI.ArcGIS.Client.FeatureService;
    
    public class CodedValueDomainService : PropertyChangedBase
    {
        private readonly FeatureLayer targetService;

        private BindableCollection<CodedValueDomain> domains;

        private bool isLoaded;

        public CodedValueDomainService(string serviceUrl)
        {
            this.Domains = new BindableCollection<CodedValueDomain>();

            this.targetService = new FeatureLayer
            {
                Url = serviceUrl,
                Mode = FeatureLayer.QueryMode.Snapshot
            };

            this.targetService.Initialize();
        }

        public void LoadDomains(Action<BindableCollection<CodedValueDomain>> completed, Action<Exception> failed)
        {
            this.targetService.Initialized += (s, e) =>
            {
                var fields = this.targetService.LayerInfo.Fields.Where(x => x.Domain != null);

                foreach (var field in fields)
                {
                    this.domains.Add(field.Domain as CodedValueDomain);
                }

                this.IsLoaded = true;

                completed.Invoke(this.Domains);
            };

            this.targetService.InitializationFailed += (s, e) =>
            {
                
            };

            this.targetService.Initialize();
        }

        public BindableCollection<CodedValueDomain> Domains
        {
            get
            {
                return this.domains;
            }
            set
            {
                if (Equals(value, this.domains))
                {
                    return;
                }
                this.domains = value;
                this.NotifyOfPropertyChange(() => this.Domains);
            }
        }

        public CodedValueDomain GetDomainByName(string domainName)
        {
            return this.Domains.FirstOrDefault(x => x.Name == domainName);
        }

        public bool IsLoaded
        {
            get
            {
                return this.isLoaded;
            }
            private set
            {
                if (value.Equals(this.isLoaded))
                {
                    return;
                }
                this.isLoaded = value;
                this.NotifyOfPropertyChange(() => this.IsLoaded);
            }
        }
    }
}
