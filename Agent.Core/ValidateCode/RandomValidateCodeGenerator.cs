using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace TGS.Services.Agent.ValidateCode;

internal class RandomValidateCodeGenerator : IValidateCodeGenerator
{
    public string GenerateCode() => string.Join(string.Empty, CreateNumberSerial().Take(6));

    public string GenerateSecurityKey() => Guid.NewGuid().ToString();

    private IEnumerable<int> CreateNumberSerial()
    {
        while (true)
            yield return RandomNumberGenerator.GetInt32(10);
    }
}