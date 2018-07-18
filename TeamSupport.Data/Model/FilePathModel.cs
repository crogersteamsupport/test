using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq.Mapping;
using System.Data.Linq;

namespace TeamSupport.Data.Model
{
    [Table(Name = "FilePaths")]
    public class FilePathModel
    {
#pragma warning disable CS0649  // Field is never assigned to, and will always have its default value null
        private Int64 _ID;
        [Column(Storage = "_ActionToAnalyzeID", IsPrimaryKey = true, IsDbGenerated = true)]
        public Int64 ID { get { return _ID; } }

        [Column]
        public string Value;
#pragma warning restore CS0649

        static FilePathModel[] _filePaths;
        public static FilePathModel[] GetFilePaths(DataContext db)
        {
            if (_filePaths != null)
                return _filePaths;
            Table<FilePathModel> table = db.GetTable<FilePathModel>();
            return _filePaths = table.ToArray();
        }

        public static string RootPath(DataContext db)
        {
            FilePathModel[] filePaths = GetFilePaths(db);
            foreach (FilePathModel path in filePaths)
            {
                if (path.ID == 1)
                    return path.Value;
            }
            return String.Empty;
        }
    }
}
