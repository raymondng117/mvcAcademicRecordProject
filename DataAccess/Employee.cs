using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace LabAssignment6.DataAccess;

public partial class Employee
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

}

public partial class Employee
{
    [NotMapped]
    public List<int> SelectedRoleIds { get; set; }
}
