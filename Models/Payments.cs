using System.ComponentModel.DataAnnotations;

namespace Getdata1.Models
{
    public class Payments
    {
        [Key]
        public int Id { get; set;}
        public int OrderId { get; set; }
        public _Order? Order { get; set; }
        public string Method { get; set; }
        public string Status { get; set; }
        public DateTime PaidAt { get; set; }
    }
}
