using System;
using System.Collections.Generic;

namespace Slutprojekt_Serhan_Gyuler.Models;

public partial class Subject
{
    public int SubjectId { get; set; }

    public int? ClassId { get; set; }

    public int? EmployeeId { get; set; }

    public string? SubjectName { get; set; }

    public bool? IsActive { get; set; }

    public virtual Class? Class { get; set; }

    public virtual Employee? Employee { get; set; }

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
}
