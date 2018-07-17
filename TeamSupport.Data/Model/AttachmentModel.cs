using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;

namespace TeamSupport.Data.Model
{
    class AttachmentModel
    {
        public ActionModel Action { get; private set; }
        public int AttachmentID { get; private set; }
        AttachmentProxy _proxy;
        public DataContext _db { get; private set; }

        public AttachmentModel(ActionModel action)
        {
            Action = action;
        }


    }
}
