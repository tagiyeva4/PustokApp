using System.ComponentModel.DataAnnotations.Schema;
using TemplatePustokApp.Models;

namespace TemplatePustokApp.ViewModel
{
    public class OrderVm
    {
        public string Country { get; set; }
        public string TownCity { get; set; }
        public string State { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
    }
}
