using TGS.Services.GameOperations.Calculators.Enums;

namespace TGS.Services.GameOperations.Calculators
{
    internal interface IBetValuesCalculatorFactory
    {
        IBetValuesCalculator Get(BetValuesType type);
    }
}