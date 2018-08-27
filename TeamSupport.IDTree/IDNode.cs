using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TeamSupport.IDTree
{
    public abstract class IDNode
    {
        public ConnectionContext Connection { get; private set; }

        protected IDNode(ConnectionContext request)
        {
            Connection = request;
        }

        protected IDNode(IDNode node) //: this(node.Request)
        {
            Connection = node.Connection;
        }

        public abstract void Verify();

        /// <summary> Verify helper </summary>
        protected void Verify(string query)
        {
            if (Connection._db.ExecuteQuery<int>(query).Any()) // valid ID found?
                return;

            if (Debugger.IsAttached)
            {
                Debug.WriteLine(query);   // see the failed query in the debug output window
                Debugger.Break();   // ID is wrong - fix the code!
            }
            throw new System.Data.ConstraintException(String.Format($"{query} not found")); // error - a join of the records to authentication just doesn't add up
        }

    }
}
