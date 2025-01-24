using System.Linq;
using TemplatePustokApp.Data;
using TemplatePustokApp.Models;

namespace TemplatePustokApp.Services
{
    public class LayoutServices
    {
        private readonly PustokAppDbContext _context;

        public LayoutServices(PustokAppDbContext context)
        {
            _context = context;
        }

        public List<Genre> GetAllGenres() 
        { 
            return _context.Genres.ToList();
        }
        public Dictionary<string,string> GetSettings()
        { 
             return _context.Settings.ToDictionary(s=>s.Key,s=>s.Value);
        }
    }
}
