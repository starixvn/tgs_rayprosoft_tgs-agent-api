using TGS.Services.GameOperations.Calculators.Enums;

namespace TGS.Services.GameOperations.Calculators
{
    internal class BetValuesCalculatorFactory : IBetValuesCalculatorFactory
    {
        public IBetValuesCalculator Get(BetValuesType type)
            => type switch
            {
                BetValuesType.Custom => new CustomBetValuesCalculator(),
                _                    => new DefaultBetValuesCalculator()
            };
    }
}