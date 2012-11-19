namespace UsingCVDWithoutMap.FeatureService
{
    public interface IFeatureServiceManager
    {
        FeatureService CreateOrGetFeatureService(string url);

        void RemoveExistingFeatureService(string url);
    }
}