using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public IndexModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        public string NameSort { get; set; }
        public string DateSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public PaginatedList<Student> Students { get; set; }
    
        public async Task OnGetAsync(string sortOrder, string searchString, string currentFilter, int? pageIndex)
        {
            NameSort = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            DateSort = sortOrder == "Date" ? "date_aesc" : "Date";

            CurrentFilter = searchString;
            CurrentSort = sortOrder;


            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }


            IQueryable<Student> studentsIQ = from s in _context.Students
                                             select s;


            if (string.IsNullOrEmpty(searchString))
            {
                studentsIQ = studentsIQ.Where(x => x.LastName.Contains(searchString) || x.FirstMidName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    studentsIQ = studentsIQ.OrderByDescending(x => x.LastName);
                    break;
                case "Date":
                    studentsIQ = studentsIQ.OrderByDescending(x => x.EnrollmentDate);
                    break;
                case "date_desc":
                    studentsIQ = studentsIQ.OrderByDescending(x => x.EnrollmentDate);
                    break;
                default:
                    studentsIQ = studentsIQ.OrderBy(x => x.LastName);
                    break;
            }

            int pageSize = 3;
            Students = await PaginatedList<Student>.CreateAsync(studentsIQ
                .AsNoTracking(), pageIndex ?? 1, pageSize);
                
        }
    }
}
