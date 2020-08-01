using System.ComponentModel.DataAnnotations;

namespace BO
{
    public class b03StatusGroup:BaseBO
    {
        [Key]
        public int b03ID { get; set; }
        public string b03Name { get; set; }
    }
}
