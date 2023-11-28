using LabAssignment6.DataAccess;

namespace LabAssignment6.Models
{
    public partial class AcademicrecordModel
    {
        public class AcademicRecordComparer : IComparer<Academicrecord>
        {
            private string CompareBy { get; set; }

            public AcademicRecordComparer(string compareBy)
            {
                if (compareBy == "course" || compareBy == "student" || compareBy == "grade")
                    CompareBy = compareBy;
                else
                    throw new Exception("Unsupported comparing criteria!");
            }

            public int Compare(Academicrecord rd1, Academicrecord rd2)
            {
                if (rd1.Grade == null && rd2.Grade == null) return 0; // Both have null grades, consider them equal
                else if (rd1.Grade == null) return -1; // Only rd1 has a null grade, so it comes first
                else if (rd2.Grade == null) return 1; // Only rd2 has a null grade, so it comes first

                // At this point, both rd1 and rd2 have non-null grades

                if (CompareBy == "course")
                {
                    return rd1.CourseCodeNavigation.Title.CompareTo(rd2.CourseCodeNavigation.Title);
                }
                else if (CompareBy == "student")
                {
                    return rd1.Student.Name.CompareTo(rd2.Student.Name);
                }
                else if (CompareBy == "grade")
                {
                    return rd1.Grade.Value.CompareTo(rd2.Grade.Value);
                }
                else
                {
                    throw new Exception("Unsupported comparing criteria!");
                }
            }
        }

    }
}
