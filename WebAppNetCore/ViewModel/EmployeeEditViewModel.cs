using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAppNetCore.ViewModel
{
    public class EmployeeEditViewModel:EmployeeCreateViewModel
    {
        public int Id { get; set; }
        public string  ExistingPhotoPath { get; set; }

    }
}
