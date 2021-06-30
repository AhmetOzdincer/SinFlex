using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SinFlexSinema.Models
{
    public class BasketModels
    {
        public BasketModels()
        {
            this.Films = new Dictionary<int, int>();
        }
        public Dictionary<int,int> Films { get; set; }
    }
}