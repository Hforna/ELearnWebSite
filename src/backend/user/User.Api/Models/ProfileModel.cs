﻿using MimeKit.Tnef;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace User.Api.Models
{
    [Table("profiles")]
    public class ProfileModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long UserId { get; set; }
        public UserModel User { get; set; }
        public string ProfilePicture { get; set; }
        public string Country { get; set; }
        public string Bio { get; set; }
        public int? CourseNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal? Note { get; set; }
        public string FullName { get; set; }
    }
}
