using System.ComponentModel.DataAnnotations;

namespace SSDDemo.Models
{
    public class SSD
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Descricao { get; set; }
       
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Sigla { get; set; }
        
        public string EmailAtendimento { get; set; }
        public string Url { get; set; }
        public string Status { get; set; }
    }
}