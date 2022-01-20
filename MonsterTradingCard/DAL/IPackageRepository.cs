using MonsterTradingCard.Models.Package;

namespace MonsterTradingCard.DAL.IPackageRepository
{
    public interface IPackageRepository
    {
        void InsertPackage(Package package);
        Package SelectFirstPackage();
        void UpdatePackageOwner(int packageId, string authToken);
    }
}
