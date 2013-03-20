using System;
using System.Runtime.Serialization;

namespace RaceAlly.BusinessLogic.Membership
{
    [Serializable]
    public class MembershipException : Exception
    {
        public MembershipStatus StatusCode { get; set; }

        public MembershipException()
        {
        }

        public MembershipException(string message)
            : base(message)
        {
        }

        public MembershipException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public MembershipException(MembershipStatus statusCode)
        {
            this.StatusCode = statusCode;
        }

        protected MembershipException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
