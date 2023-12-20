using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTSv9.Models
{
    public class FileOnFileSystemModel
    {
        public string DoctorUserName { get; set; }
        public string PatientUserName { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public List<FileOnFileSystemModel> FilesOnFileSystem { get; set; }
    }
}
