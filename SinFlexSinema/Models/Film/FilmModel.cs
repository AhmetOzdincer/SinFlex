using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SinFlexSinema.Models.Film
{
    public class FilmModel
    {
        public List<DB.Film> Films { get; set; }
        public DB.FilmTur FilmTur { get; set; }
    }
}