using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LabAssignment6.DataAccess;

public partial class Academicrecord
{
    public string CourseCode { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public int? Grade { get; set; }

    public virtual Course CourseCodeNavigation { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}


