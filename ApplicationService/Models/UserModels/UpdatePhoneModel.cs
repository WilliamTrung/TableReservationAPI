using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validator;

namespace ApplicationService.Models.UserModels
{
    public class UpdatePhoneModel
    {
        [PhoneValidator(ErrorMessage = "Provided phone number is not valid!")]
        public string Phone { get; set; } = null!;
    }
}
