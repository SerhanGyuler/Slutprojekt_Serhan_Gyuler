using System;
using System.Collections.Generic;

namespace Slutprojekt_Serhan_Gyuler.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Profession { get; set; }

    public int? YearsWorked { get; set; }

    public decimal? Salary { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
