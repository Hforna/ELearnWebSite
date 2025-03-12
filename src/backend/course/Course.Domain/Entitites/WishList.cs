﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Entitites
{
    [Table("wish-list")]
    public class WishList : BaseEntity
    {
        public long UserId { get; set; }
        public long CourseId { get; set; }
        public CourseEntity Course { get; set; }
    }
}
