using System;

namespace TGS.Apis.OperatorApi.Controllers.Messages.Players
{
    public class PlayerOperationModel
    {
        public DateTimeOffset CreatedOn { get; set; }
        public string Operation { get; set; }
        public string CreatedByUsername { get; set; }
    }
}
