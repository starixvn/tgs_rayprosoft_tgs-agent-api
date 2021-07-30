namespace TGS.Core.DataServices.Tgs.Models
{
    public class PlayerGameSetting
    {
        public bool IsAutoTransferToGame { get; set; }

        public string PlayerCode { get; set; }

        public virtual Player Player { get; set; }
    }
}
