using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Repositories
{
    public interface IUnitOfWork
    {
        public ICourseReadOnly courseRead { get; set; }
        public ICourseWriteOnly courseWrite { get; set; }
        public IModuleWriteOnly moduleWrite { get; set; }
        public IModuleReadOnly moduleRead { get; set; }
        public ILessonReadOnly lessonRead { get; set; }
        public ILessonWriteOnly lessonWrite { get; set; }
        public IVideoReadOnly videoRead { get; set; }
        public IVideoWriteOnly videoWrite { get; set; }
        public IEnrollmentReadOnly enrollmentRead { get; set; }
        public IEnrollmentWriteOnly enrollmentWrite { get; set; }
        public IReviewReadOnly reviewRead { get; set; }
        public IReviewWriteOnly reviewWrite { get; set; }
        public IWishListReadOnly wishListRead { get; set; }
        public IWishListWriteOnly wishListWrite { get; set; }
        public Task Commit();
    }
}
