using Microsoft.VisualBasic.FileIO;
using Sabio.Models.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabio.Models.Requests.Files
{
    public class FileAddRequest
    {
        [Required]
        [StringLength(255, MinimumLength = 2)]
        public string Name { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 2)]
        public string Url { get; set; }
        [Required]
        [StringLength(50)]
        public string FileType { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int CreatedBy { get; set; }
    }
}
