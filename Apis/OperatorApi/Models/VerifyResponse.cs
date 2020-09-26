namespace TGS.Apis.OperatorApi.Models
{
    public record VerifyResponse
    {
        public VerifyResponse(
            bool isValid = true,
            string message = "")
            => (IsValid, Message) = (isValid, message);

        public bool IsValid { get; init; } = true;
        public string Message { get; init; } = string.Empty;
    }
}
