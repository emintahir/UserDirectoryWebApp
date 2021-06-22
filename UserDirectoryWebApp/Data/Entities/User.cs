using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace UserDirectoryWebApp.Data.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        public string Email { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime BirthDate { get; set; }

        [Required]
        public string Phone { get; set; }

        [Display(Name = "Image Name")]
        public string UserImageName { get; set; }

        [NotMapped]
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }

    }
}
