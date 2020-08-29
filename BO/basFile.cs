using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;


namespace BO
{
    public class BASFILE
    {

        public static void SaveStream2File(String strDestFullPath, Stream inputStream)
        {
            
            using (FileStream outputFileStream = new FileStream(strDestFullPath, FileMode.Create))
            {
                inputStream.CopyTo(outputFileStream);
            }

            
        }

        static List<string> GetFileNamesInDir(string strDir, string strPattern,bool getFullPath)
        {
            DirectoryInfo dir = new DirectoryInfo(strDir);
            var lis = new List<string>();
            foreach (FileInfo file in dir.GetFiles(strPattern))
            {
                if (getFullPath == true)
                {
                    lis.Add(file.FullName);
                }
                else
                {
                    lis.Add(file.Name);
                }
                

            }

            return lis;
        }

        public static List<BO.o27Attachment> GetUploadedFiles(string strSourceTempDir, string strTempGUID)
        {
            var lisO27 = new List<BO.o27Attachment>();
            foreach (string file in System.IO.Directory.EnumerateFiles(strSourceTempDir, strTempGUID + "*.infox", System.IO.SearchOption.TopDirectoryOnly))
            {
                var info = System.IO.File.ReadAllText(file).Split("|");                
                var cO27 = new BO.o27Attachment() { o27ContentType = info[0], o27FileSize = BO.BAS.InInt(info[1]), o27Name = info[2], o27GUID = strTempGUID };
                cO27.o27ArchiveFileName = cO27.o27GUID + "_" + cO27.o27Name;
                cO27.FullPath = strSourceTempDir+"\\"+ cO27.o27ArchiveFileName;
                lisO27.Add(cO27);
            }

            return lisO27;
        }

        public static List<BO.o27Attachment> CopyTempFiles2Upload(string strSourceTempDir, string strTempGUID,string strDestUploadDir)
        {
            var lisO27 = new List<BO.o27Attachment>();
            foreach (string file in System.IO.Directory.EnumerateFiles(strSourceTempDir, strTempGUID + "_*.infox", System.IO.SearchOption.TopDirectoryOnly))
            {
                var info = System.IO.File.ReadAllText(file).Split("|");
                var strGUID = BO.BAS.GetGuid();
                var cO27 = new BO.o27Attachment() { o27ContentType = info[0], o27FileSize = BO.BAS.InInt(info[1]), o27Name = info[2], o27GUID = strGUID };
                cO27.o27ArchiveFileName = strGUID + "_" + cO27.o27Name;
                cO27.o27ArchiveFolder = DateTime.Now.Year.ToString() + "\\" + DateTime.Now.Month.ToString();
                if (!System.IO.Directory.Exists(strDestUploadDir + "\\" + cO27.o27ArchiveFolder))
                {
                    System.IO.Directory.CreateDirectory(strDestUploadDir + "\\" + cO27.o27ArchiveFolder);
                }
                cO27.FullPath = strDestUploadDir + "\\" + cO27.o27ArchiveFolder + "\\" + cO27.o27ArchiveFileName;
                System.IO.File.Copy(strSourceTempDir + "\\" + strTempGUID + "_" + cO27.o27Name, cO27.FullPath, true);
                
                lisO27.Add(cO27);
            }
            return lisO27;
        }
    }
}
