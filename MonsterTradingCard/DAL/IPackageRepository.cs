using MonsterTradingCard.Models.Package;

namespace MonsterTradingCard.DAL.IPackageRepository
{
    public interface IPackageRepository
    {
        void InsertPackage(Package package);
        Package SelectRandomPackage();
        void UpdatePackageOwner(int packageId, string authToken);
    }
}
