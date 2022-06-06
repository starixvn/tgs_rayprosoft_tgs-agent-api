using System;

namespace TGS.Apis.OperatorApi.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class IgnoreUserBannedAttribute : Attribute
{
}
