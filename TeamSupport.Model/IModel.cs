using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.Model
{
    public interface IModel
    {
        ConnectionContext Connection { get; }
    }
}
