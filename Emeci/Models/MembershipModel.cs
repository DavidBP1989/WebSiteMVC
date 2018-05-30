using static Emeci.Models.PayUModel;

namespace Emeci.Models
{
    public class MembershipModel
    {
        public enum MembershipType
        {
            NewFile = 0,
            Renewal = 1
        }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public MembershipType Type { get; set; }
        public StatusT Status { get; set; } = StatusT.Ninguno;
        public string EMECI { get; set; }
        public string Error { get; set; }
    }
}