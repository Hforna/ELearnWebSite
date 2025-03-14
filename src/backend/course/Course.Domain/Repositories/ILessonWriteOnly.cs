﻿using Course.Domain.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Repositories
{
    public interface ILessonWriteOnly
    {
        public void AddLesson(Lesson lesson);
        public void DeleteLessonRange(IList<Lesson> lessons);
        public void DeleteLesson(Lesson lesson);
        public void UpdateLesson(Lesson lesson);
    }
}
