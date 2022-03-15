using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
