using System.Data;

namespace TourPlanner.Server.DAL
{
    public class TransactionObject
    {
        public int ExpectedAffectedRows { get; set; }
        public IDbCommand Command { get; set; }

        public TransactionObject(IDbCommand command, int expectedAffectedRows)
        {
            Command = command;
            ExpectedAffectedRows = expectedAffectedRows;
        }
    }
}
