using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TeamSupport.IDTree
{
    public class ProductVersionModel : IDNode, IAttachmentDestination
    {

        public ProductModel Product { get; private set; }
        public int ProductVersionID { get; private set; }

        public ProductVersionModel(ProductModel product, int productVersionID) : base(product)
        {
        }

        public ProductVersionModel(ConnectionContext connection, int productVersionID) : base(connection)
        {
            int productID = ExecuteQuery<int>($"SELECT ProductID FROM ProductVersions WITH (NOLOCK) WHERE ProductVersionID = {productVersionID}").Min();
            Product = new ProductModel(connection, productID);
            ProductVersionID = productVersionID;
        }

        public override void Verify()
        {
        }

        string IAttachmentDestination.AttachmentPath
        {
            get
            {
                string path = Connection.Organization.AttachmentPath;
                path = Path.Combine(path, "Products");   // see AttachmentPath.GetFolderName(AttachmentPath.Folder.Actions);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

    }
}
