using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LabAssignment6.DataAccess;
using LabAssignment6.Models;

namespace LabAssignment6.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly StudentrecordContext _context;

        public EmployeesController(StudentrecordContext context)
        {
            _context = context;
        }

        public List<Employee> employees { get; set; } = new List<Employee>();


        // GET: Employees
        public async Task<IActionResult> Index()
        {
            if (_context.Employees != null)
            {
                employees = _context.Employees.Include(e => e.Roles).ToList();
            }

            return View(employees);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,UserName,Password")] Employee employee, List<int> SelectedRoleIds)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                int newEmployeeId = employee.Id;

                if (SelectedRoleIds != null && SelectedRoleIds.Any())
                {
                    foreach (var roleId in SelectedRoleIds)
                    {
                       _context.Database.ExecuteSqlRaw(
                            $"INSERT INTO employee_role (Employee_Id, Role_id) VALUES ({newEmployeeId}, {roleId});");
                        await _context.SaveChangesAsync();
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.Roles) // Include roles
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            var viewModel = new EmployeeEditViewModel
            {
                Employee = employee,
                SelectedRoleIds = employee.Roles.Select(r => r.Id).ToList(), // Populate selected role IDs
                AllRoles = await _context.Roles.ToListAsync() // Retrieve all roles
            };

            return View(viewModel);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeEditViewModel employeeEditViewModel, List<int> selectedRoleIds)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Update employee information
                    _context.Update(employeeEditViewModel.Employee);
                    await _context.SaveChangesAsync();

                    // Execute SQL to remove existing roles
                    await _context.Database.ExecuteSqlRawAsync(
                        $"DELETE FROM employee_role WHERE Employee_Id = {employeeEditViewModel.Employee.Id}");

                    // Execute SQL to add new roles
                    foreach (var roleId in selectedRoleIds)
                    {
                        await _context.Database.ExecuteSqlRawAsync(
                            $"INSERT INTO employee_role (Employee_Id, Role_Id) VALUES ({employeeEditViewModel.Employee.Id}, {roleId})");
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Handle concurrency conflicts if necessary
                    return NotFound();
                }
            }

            return View(employeeEditViewModel);
        }







        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Employees == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Employees == null)
            {
                return Problem("Entity set 'StudentrecordContext.Employees'  is null.");
            }
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return (_context.Employees?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
