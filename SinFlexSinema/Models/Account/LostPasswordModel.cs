using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SinFlexSinema.Models.Account
{
    public class LostPasswordModel
    {
        [Required(ErrorMessage = "Kullanıcı adınızı giriniz.")]
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; }
    }
}