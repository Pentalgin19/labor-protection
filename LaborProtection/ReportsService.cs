using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using LaborProtection.Entities;

namespace LaborProtection
{
    public class ReportsService
    {
        private readonly AppContext _context;
        public ReportsService()
        {
            _context = new AppContext();
        }

        // SQL-запрос: Получить всех сотрудников определённого отдела
        public List<Employee> GetEmployeesByDepartmentSql(int departmentId)
        {
            return _context.employees
                .FromSqlRaw($"SELECT * FROM employees WHERE department = {departmentId}")
                .ToList();
        }

        // LINQ-запрос: Количество сотрудников по отделам
        public List<(int Department, int Count)> GetEmployeeCountByDepartmentSql()
        {
            return _context.employees
                .GroupBy(e => e.department)
                .Select(g => new { Department = g.Key, Count = g.Count() })
                .AsEnumerable()
                .Select(x => (x.Department, x.Count))
                .ToList();
        }

        // LINQ-запрос: Сотрудники, принятые после даты
        public List<Employee> GetEmployeesHiredAfter(DateOnly date)
        {
            return _context.employees
                .Where(e => e.date_of_employment > date)
                .ToList();
        }

        // LINQ-запрос: Экзамены сотрудника
        public List<Exam> GetExamsByEmployee(int employeeId)
        {
            return _context.exams
                .Where(ex => ex.id_emp == employeeId)
                .ToList();
        }
    }
} 