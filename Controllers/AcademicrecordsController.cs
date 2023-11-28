using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LabAssignment6.DataAccess;
using LabAssignment6.Models;
using Humanizer;
using Microsoft.AspNetCore.Http.HttpResults;

namespace LabAssignment6.Controllers
{
    public class AcademicrecordsController : Controller
    {

        private readonly StudentrecordContext _context;

        public AcademicrecordsController(StudentrecordContext context)
        {
            _context = context;
        }



        public string OrderBy { get; set; }
        public List<Academicrecord> sortedAR { get; set; } = new List<Academicrecord>();

        // GET: Academicrecords
        public async Task<IActionResult> Index(string orderby)
        {
            var academicrecords = _context.Academicrecords
                .Include(a => a.CourseCodeNavigation)
                .Include(a => a.Student);

            if (orderby != null)
            {
                Console.WriteLine(orderby);
                sortedAR = await academicrecords.ToListAsync();
                AcademicrecordModel.AcademicRecordComparer comparer = new AcademicrecordModel.AcademicRecordComparer(orderby);
                sortedAR.Sort(comparer);
            }
            else
            {
                sortedAR = await academicrecords.ToListAsync();
                AcademicrecordModel.AcademicRecordComparer comparer = new AcademicrecordModel.AcademicRecordComparer("course");
                sortedAR.Sort(comparer);
            }

            return View(sortedAR);
        }


        // GET: Academicrecords/Edit/5
        public async Task<IActionResult> Edit(string studentId, string courseId)
        {



            if (studentId == null || courseId == null || _context.Academicrecords == null)
            {
                return NotFound();
            }

            //To address this issue, you need to ensure that the related entities(CourseCodeNavigation and Student) are loaded when querying the Academicrecord from the database.Modify your Edit action in the controller to eagerly load these related entities:
            var academicrecord = await _context.Academicrecords
            .Include(a => a.CourseCodeNavigation)  // Eagerly load CourseCodeNavigation
            .Include(a => a.Student)               // Eagerly load Student
            .FirstOrDefaultAsync(m => m.StudentId == studentId && m.CourseCode == courseId);
            // In this updated code, the Include method is used to specify which related entities should be loaded along with the Academicrecord. This should ensure that the CourseCodeNavigation and Student properties are not null when you access them in your view.


            if (academicrecord == null)
            {
                return NotFound();
            }
            return View(academicrecord);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("CourseCode,StudentId,Grade")] Academicrecord academicrecord)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(academicrecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AcademicrecordExists(academicrecord.StudentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(academicrecord);
        }

        public async Task<IActionResult> EditAll(string orderby)
        {

            var academicrecords = _context.Academicrecords.Include(a => a.CourseCodeNavigation).Include(a => a.Student);

            if (orderby != null)
            {
                Console.WriteLine(orderby);
                sortedAR = await academicrecords.ToListAsync();
                AcademicrecordModel.AcademicRecordComparer comparer = new AcademicrecordModel.AcademicRecordComparer(orderby);
                sortedAR.Sort(comparer);
            }
            else
            {
                sortedAR = await academicrecords.ToListAsync();
                AcademicrecordModel.AcademicRecordComparer comparer = new AcademicrecordModel.AcademicRecordComparer("course");
                sortedAR.Sort(comparer);
            }

            return View(sortedAR);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAll(List<LabAssignment6.DataAccess.Academicrecord> academicrecords)
        {
            if (ModelState.IsValid)
            {
                foreach (var academicrecord in academicrecords)
                {
                    var dbRecord = await _context.Academicrecords.FindAsync(academicrecord.StudentId, academicrecord.CourseCode);
                    if (dbRecord != null)
                    {
                        dbRecord.Grade = academicrecord.Grade;
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(academicrecords);
        }

        // GET: Academicrecords/Create
        public IActionResult Create()
        {
            ViewData["CourseCode"] = new SelectList(_context.Courses, "Code", "Code");
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id");
            return View();
        }

        // POST: Academicrecords/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CourseCode,StudentId,Grade")] Academicrecord academicrecord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(academicrecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseCode"] = new SelectList(_context.Courses, "Code", "Code", academicrecord.CourseCode);
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Id", academicrecord.StudentId);
            return View(academicrecord);
        }

       






        // GET: Academicrecords/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Academicrecords == null)
            {
                return NotFound();
            }

            var academicrecord = await _context.Academicrecords
                .Include(a => a.CourseCodeNavigation)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (academicrecord == null)
            {
                return NotFound();
            }

            return View(academicrecord);
        }

        // POST: Academicrecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Academicrecords == null)
            {
                return Problem("Entity set 'StudentrecordContext.Academicrecords'  is null.");
            }
            var academicrecord = await _context.Academicrecords.FindAsync(id);
            if (academicrecord != null)
            {
                _context.Academicrecords.Remove(academicrecord);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Academicrecords/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Academicrecords == null)
            {
                return NotFound();
            }

            var academicrecord = await _context.Academicrecords
                .Include(a => a.CourseCodeNavigation)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(m => m.StudentId == id);
            if (academicrecord == null)
            {
                return NotFound();
            }

            return View(academicrecord);
        }

        private bool AcademicrecordExists(string id)
        {
            return (_context.Academicrecords?.Any(e => e.StudentId == id)).GetValueOrDefault();
        }
    }
}
