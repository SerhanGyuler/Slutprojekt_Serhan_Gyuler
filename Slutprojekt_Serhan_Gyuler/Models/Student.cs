using System;
using System.Collections.Generic;

namespace Slutprojekt_Serhan_Gyuler.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public int? ClassId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public int? SocialSecurityNumber { get; set; }

    public virtual Class? Class { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
