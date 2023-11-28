using LabAssignment6.DataAccess;

namespace LabAssignment6.Models
{
    public class EmployeeEditViewModel
    {
        public Employee Employee { get; set; }
        public List<int> SelectedRoleIds { get; set; }
        public List<Role> AllRoles { get; set; }
    }
}
