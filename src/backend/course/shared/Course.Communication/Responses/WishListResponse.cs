﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Communication.Responses
{
    public class WishListResponse
    {
        public string UserId { get; set; }
        public IList<CourseWishListResponse> Courses { get; set; }
    }
}
