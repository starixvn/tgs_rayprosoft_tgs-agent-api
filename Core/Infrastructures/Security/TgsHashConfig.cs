namespace TGS.Infrastructures.Security
{
    public interface ITgsHashConfig
    {
        byte[] Salt { get; }
        HashAlgorithmKind HashAlgorithm { get; }
    }

    public sealed class TgsHashConfig : ITgsHashConfig
    {
        byte[] ITgsHashConfig.Salt { get; } =
            BinaryUtil.HexStringToBytes("19CD3E1E7ECD7FA7E62EDB75ECBA99D91EFF16231A5CC279FB41206846B83FC1");

        HashAlgorithmKind ITgsHashConfig.HashAlgorithm { get; } = HashAlgorithmKind.Sha512;
    }
}