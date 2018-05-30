using Postal;
using static Emeci.Models.MembershipModel;

namespace Emeci.Models
{
    public class EmailMembership : Email
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public MembershipType MembershipT { get; set; }
        public string Name { get; set; }
        public string EMECI { get; set; }
    }
}