﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SinFlexSinemaEntities1 : DbContext
    {
        public SinFlexSinemaEntities1()
            : base("name=SinFlexSinemaEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Bilet> Bilets { get; set; }
        public virtual DbSet<Departman> Departmen { get; set; }
        public virtual DbSet<Film> Films { get; set; }
        public virtual DbSet<FilmTur> FilmTurs { get; set; }
        public virtual DbSet<Kampanya> Kampanyas { get; set; }
        public virtual DbSet<Rezervasyon> Rezervasyons { get; set; }
        public virtual DbSet<Salon> Salons { get; set; }
        public virtual DbSet<Sati> Satis { get; set; }
        public virtual DbSet<SatisTur> SatisTurs { get; set; }
        public virtual DbSet<Sean> Seans { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserType> UserTypes { get; set; }
    }
}
