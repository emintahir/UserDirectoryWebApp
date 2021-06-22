using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserDirectoryWebApp.Data.Entities;

namespace UserDirectoryWebApp.ViewModels
{
    public class UserVM
    {
        public User User { get; set; }
        public string DeleteImage { get; set; }
    }
}
