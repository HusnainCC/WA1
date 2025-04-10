using Microsoft.EntityFrameworkCore;
using WA1.Context;
using WA1.Entities;
using WA1.UserInterface;

namespace WA1.Repositories
{
    public class UserRepo : IEmployee
    {
        private readonly WA1dbcontext _context;

        public UserRepo(WA1dbcontext context)
        {
            _context = context;
        }

        public async Task AddEmployeeAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
        }

        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
        {
            return await _context.Employees
                .Include(e => e.Department) 
                .FirstOrDefaultAsync(e => e.Id == employeeId);
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .Include(e => e.Department)
                .ToListAsync();
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }
    }
    }
