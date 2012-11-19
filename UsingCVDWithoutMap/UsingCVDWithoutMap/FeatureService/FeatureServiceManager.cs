namespace UsingCVDWithoutMap.FeatureService
{
    using System.Collections.Generic;

    public class FeatureServiceManager : IFeatureServiceManager
    {
        private readonly Dictionary<string, FeatureService> featureServices;

        public FeatureServiceManager()
        {
            this.featureServices = new Dictionary<string, FeatureService>();
        }

        public FeatureService CreateOrGetFeatureService(string url)
        {
            if (this.featureServices.ContainsKey(url))
            {
                return this.featureServices[url];
            }

            var service = new FeatureService(url);
            this.featureServices.Add(url, service);

            return service;
        }

        public void RemoveExistingFeatureService(string url)
        {
            if (this.featureServices.ContainsKey(url))
            {
                this.featureServices.Remove(url);
            }
        }
    }
}
