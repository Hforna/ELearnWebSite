﻿using Course.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Entitites
{
    [Table("reviews")]
    public class Review : BaseEntity
    {
        public long CustomerId { get; set; }
        public long CourseId { get; set; }
        public string Text { get; set; }
        public CourseRatingEnum Rating { get; set; }
    }
}
