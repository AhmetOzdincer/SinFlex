
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace SinFlexSinema.DB
{

using System;
    using System.Collections.Generic;
    
public partial class Film
{

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public Film()
    {

        this.Bilets = new HashSet<Bilet>();

        this.Seans = new HashSet<Sean>();

    }


    public int Film_ID { get; set; }

    public string Adi { get; set; }

    public System.TimeSpan Sure { get; set; }

    public decimal Fiyat { get; set; }

    public bool YerliMi { get; set; }

    public int FilmTur_ID { get; set; }

    public bool SatisDevamEdiyoMu { get; set; }

    public Nullable<int> StokDurumu { get; set; }



    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Bilet> Bilets { get; set; }

    public virtual FilmTur FilmTur { get; set; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

    public virtual ICollection<Sean> Seans { get; set; }

}

}
