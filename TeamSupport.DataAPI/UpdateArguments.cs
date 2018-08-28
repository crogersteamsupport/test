using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamSupport.DataAPI
{
    public class UpdateArguments
    {
        StringBuilder _builder;
        public UpdateArguments()
        {
            _builder = new StringBuilder();
        }

        public UpdateArguments(string name, int value) : this()
        {
            Insert(name, value);
        }

        public void Insert(string name, int value)
        {
            if (_builder.Length == 0)
                _builder.Append($"{name}={value}");
            else
                _builder.Append($", {name}={value}");
        }
        public override string ToString()
        {
            return _builder.ToString();
        }
    }


}
