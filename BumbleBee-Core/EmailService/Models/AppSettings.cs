using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailSuitConsole.Models
{
    public class AppSettings
    {
        public string? BaseURL { get; set; }
        public string? FeedSectionURL { get; set; }
        public bool SavingOneYearBlog { get; set; }
        public string? ImagePhysicalPath { get; set; }
        public string? ImageFolderPath { get; set; }
    }
}
