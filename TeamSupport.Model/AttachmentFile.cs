﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Web;

namespace TeamSupport.Model
{
    public class AttachmentFile
    {
        public string FileName { get; private set; }
        public string FilePath { get; private set; }
        public int ContentLength { get; private set; }
        public string ContentType { get; private set; }

        HttpPostedFile _postedFile;

        public AttachmentFile(string attachmentPath, HttpPostedFile postedFile)
        {
            FileName = VerifyFileName(attachmentPath, postedFile.FileName);
            FilePath = Path.Combine(attachmentPath, FileName);
            ContentType = postedFile.ContentType;
            ContentLength = postedFile.ContentLength;
            _postedFile = postedFile;    // write file to disk
        }

        public void Save()
        {
            _postedFile.SaveAs(FilePath);    // write file to disk
        }

        static string VerifyFileName(string directory, string text)
        {
            string fileName = Path.GetFileName(text);
            fileName = VerifyFileName_Unique(directory, fileName);
            return VerifyFileName_SpecialCharacters(fileName);
        }

        static string VerifyFileName_Unique(string directory, string fileName)
        {
            string path = Path.Combine(directory, fileName);
            string result = fileName;
            int i = 0;
            while (File.Exists(path))
            {
                i++;
                if (i > 20) break;
                string name = Path.GetFileNameWithoutExtension(fileName);
                string ext = Path.GetExtension(fileName);
                result = name + "_" + i.ToString() + ext;
                path = Path.Combine(directory, result);
            }

            return result;
        }

        static string VerifyFileName_SpecialCharacters(string text)
        {
            return Path.GetInvalidFileNameChars().Aggregate(text, (current, c) => current.Replace(c.ToString(), "_"));
        }
    }
}
