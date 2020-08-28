using System.ComponentModel.DataAnnotations;

namespace BO
{
    public enum x31ReportFormatEnum
    {
        Telerik = 1,
        DOC = 2,
        XLS = 3,
        MSREPORTING = 4
    }
    public class x31Report : BaseBO
    {
        [Key]
        public int x31ID { get; set; }
        
        public string x31Name { get; set; }
        public string x31Description { get; set; }
        public string x31Code { get; set; }
        public string x31Entity { get; set; }

        public x31ReportFormatEnum x31ReportFormat { get; set; }

        public bool x31Is4SingleRecord { get; set; }

        public string x31FileName { get; set; }
    }
}
