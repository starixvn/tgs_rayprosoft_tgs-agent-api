namespace TGS.Services.Promotion
{
    public interface IPromotionCheckerFactory
    {
        IPromotionChecker GetPromotionChecker(string promotionType);
    }
}