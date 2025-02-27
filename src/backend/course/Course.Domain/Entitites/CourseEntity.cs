using Course.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Entitites
{
    [Table("courses")]
    public class CourseEntity : BaseEntity
    {
        public long TeacherId { get; set; }
        [MaxLength(100, ErrorMessage = "Title field length must be less than 100")]
        public string Title { get; set; }
        public string? Thumbnail { get; set; }
        public string Description { get; set; }
        public decimal Note { get; set; }
        public IList<CourseTopicsEntity> TopicsCovered { get; set; }
        public double Price { get; set; }
        public int? Enrollments { get; set; }
        public long totalVisits { get; set; } = 0;
        public LanguagesEnum CourseLanguage { get; set; }
        public TypeCourseEnum CourseType { get; set; }
        public double? Duration { get; set; }
        public int ModulesNumber { get; set; }
        public bool IsPublish { get; set; } = false;
        public Collection<Module> Modules { get; set; }
        public Guid courseIdentifier { get; set; } = Guid.NewGuid();
    }
}
