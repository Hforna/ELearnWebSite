using AutoMapper;
using CommonTestUtilities.Builds.Services;
using Course.Application.AppServices;
using Course.Application.Services;
using Course.Communication.Requests;
using Course.Communication.Responses;
using Course.Domain.Cache;
using Course.Domain.DTOs;
using Course.Domain.Entitites;
using Course.Domain.Enums;
using Course.Domain.Repositories;
using Course.Domain.Services.Azure;
using Course.Domain.Services.RabbitMq;
using Course.Domain.Services.Rest;
using Course.Domain.Sessions;
using Course.Exception;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using SharedMessages.CourseMessages;
using Sqids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UseCases.Course
{
    public class CourseServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly SqidsEncoder<long> _mockSqids;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IStorageService> _mockStorageService;
        private readonly Mock<ILocationService> _mockLocationService;
        private readonly Mock<ICurrencyExchangeService> _mockCurrencyExchange;
        private readonly Mock<FileService> _mockFileService;
        private readonly Mock<IProducerService> _mockProducerService;
        private readonly Mock<EmailService> _mockEmailService;
        private readonly Mock<IDeleteCourseSender> _mockDeleteCourseSender;
        private readonly Mock<ICourseCache> _mockCourseCache;
        private readonly Mock<ICoursesSession> _mockCoursesSession;
        private readonly Mock<ILinkService> _mockLinkService;
        private readonly CourseService _sut; // System Under Test

        public CourseServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockSqids = SqidsBuild.Build();
            _mockUserService = new Mock<IUserService>();
            _mockMapper = new Mock<IMapper>();
            _mockStorageService = new Mock<IStorageService>();
            _mockLocationService = new Mock<ILocationService>();
            _mockCurrencyExchange = new Mock<ICurrencyExchangeService>();
            _mockFileService = new Mock<FileService>();
            _mockProducerService = new Mock<IProducerService>();
            _mockEmailService = new Mock<EmailService>("asdasd", "asdasd", "asdasd");
            _mockDeleteCourseSender = new Mock<IDeleteCourseSender>();
            _mockCourseCache = new Mock<ICourseCache>();
            _mockCoursesSession = new Mock<ICoursesSession>();
            _mockLinkService = new Mock<ILinkService>();

            _sut = new CourseService(
                _mockUnitOfWork.Object,
                _mockSqids,
                _mockUserService.Object,
                _mockMapper.Object,
                _mockStorageService.Object,
                _mockLocationService.Object,
                _mockCurrencyExchange.Object,
                _mockFileService.Object,
                _mockProducerService.Object,
                _mockEmailService.Object,
                _mockDeleteCourseSender.Object,
                _mockCourseCache.Object,
                _mockCoursesSession.Object,
                _mockLinkService.Object
            );
        }

        #region CourseLessonsCount Tests

        [Fact]
        public async Task CourseLessonsCount_ValidCourseId_ReturnsCorrectCount()
        {
            // Arrange
            var courseId = 1L;
            var expectedLessonsCount = 5;
            var expectedQuizzesCount = 3;
            var course = new CourseEntity { Id = courseId };

            _mockUnitOfWork.Setup(x => x.courseRead.CourseById(courseId, It.IsAny<bool>()))
                .ReturnsAsync(course);
            _mockUnitOfWork.Setup(x => x.lessonRead.CountTotalLessons(courseId))
                .ReturnsAsync(expectedLessonsCount);
            _mockUnitOfWork.Setup(x => x.quizRead.CountQuizzes(courseId))
                .ReturnsAsync(expectedQuizzesCount);

            // Act
            var result = await _sut.CourseLessonsCount(courseId);

            // Assert
            result.Should().Be(expectedLessonsCount + expectedQuizzesCount);
            _mockUnitOfWork.Verify(x => x.courseRead.CourseById(courseId, It.IsAny<bool>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.lessonRead.CountTotalLessons(courseId), Times.Once);
            _mockUnitOfWork.Verify(x => x.quizRead.CountQuizzes(courseId), Times.Once);
        }

        [Fact]
        public async Task CourseLessonsCount_CourseNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var courseId = 999L;
            _mockUnitOfWork.Setup(x => x.courseRead.CourseById(courseId, It.IsAny<bool>()))
                .ReturnsAsync((CourseEntity)null);

            // Act
            Func<Task> act = async () => await _sut.CourseLessonsCount(courseId);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage(ResourceExceptMessages.COURSE_DOESNT_EXISTS);
            _mockUnitOfWork.Verify(x => x.lessonRead.CountTotalLessons(It.IsAny<long>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.quizRead.CountQuizzes(It.IsAny<long>()), Times.Never);
        }

        [Fact]
        public async Task CourseLessonsCount_NoLessonsOrQuizzes_ReturnsZero()
        {
            // Arrange
            var courseId = 1L;
            var course = new CourseEntity { Id = courseId };

            _mockUnitOfWork.Setup(x => x.courseRead.CourseById(courseId, It.IsAny<bool>()))
                .ReturnsAsync(course);
            _mockUnitOfWork.Setup(x => x.lessonRead.CountTotalLessons(courseId))
                .ReturnsAsync(0);
            _mockUnitOfWork.Setup(x => x.quizRead.CountQuizzes(courseId))
                .ReturnsAsync(0);

            // Act
            var result = await _sut.CourseLessonsCount(courseId);

            // Assert
            result.Should().Be(0);
        }

        #endregion

        #region CreateCourse Tests

        [Fact]
        public async Task CreateCourse_ValidRequest_ReturnsCreatedCourse()
        {
            // Arrange
            var request = new CreateCourseRequest
            {
                Price = 100.0,
                ThumbnailImage = null
            };
            var userInfos = new UserInfosDto { id = SqidsBuild.GenerateRandomSqid(), email = "test@test.com", userName = "TestUser" };
            var userId = 1L;
            var courseEntity = new CourseEntity { Id = 1L, TeacherId = userId, Price = 100.0 };
            var response = new CourseShortResponse { CourseId = SqidsBuild.GenerateRandomSqid(), TeacherId = SqidsBuild.GenerateRandomSqid(), Price = 100.0 };

            _mockUserService.Setup(x => x.GetUserInfos()).ReturnsAsync(userInfos);
            _mockMapper.Setup(x => x.Map<CourseEntity>(request)).Returns(courseEntity);
            _mockMapper.Setup(x => x.Map<CourseShortResponse>(courseEntity)).Returns(response);
            _mockLocationService.Setup(x => x.GetCurrencyByUserLocation())
                .ReturnsAsync(new CurrencyByLocationDto { Code = "USD" });
            _mockCurrencyExchange.Setup(x => x.GetCurrencyRates(It.IsAny<CurrencyEnum>()))
                .ReturnsAsync(new RateExchangeDto { BRL = 5.0, USD = 1.0, EUR = 0.9 });

            // Act
            var result = await _sut.CreateCourse(request);

            // Assert
            result.Should().NotBeNull();
            result.CourseId.Should().Be(SqidsBuild.GenerateRandomSqid());
            result.TeacherId.Should().Be(SqidsBuild.GenerateRandomSqid());
            _mockUnitOfWork.Verify(x => x.courseWrite.AddCourse(It.IsAny<CourseEntity>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.Commit(), Times.Once);
            _mockProducerService.Verify(x => x.SendCourseCreated(It.IsAny<CourseCreatedMessage>()), Times.Once);
        }

        [Fact]
        public async Task CreateCourse_UserNotAuthenticated_ThrowsNotAuthenticatedException()
        {
            // Arrange
            var request = new CreateCourseRequest();
            _mockUserService.Setup(x => x.GetUserInfos()).ReturnsAsync((UserInfosDto)null);

            // Act
            Func<Task> act = async () => await _sut.CreateCourse(request);

            // Assert
            await act.Should().ThrowAsync<NotAuthenticatedException>()
                .WithMessage(ResourceExceptMessages.USER_INFOS_DOESNT_EXISTS);
            _mockUnitOfWork.Verify(x => x.courseWrite.AddCourse(It.IsAny<CourseEntity>()), Times.Never);
        }

        [Fact]
        public async Task CreateCourse_WithThumbnail_UploadsImageSuccessfully()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var mockStream = new MemoryStream();
            mockFile.Setup(x => x.OpenReadStream()).Returns(mockStream);

            var request = new CreateCourseRequest
            {
                Price = 100.0,
                ThumbnailImage = mockFile.Object
            };
            var userInfos = new UserInfosDto { id = SqidsBuild.GenerateRandomSqid(), email = "test@test.com", userName = "TestUser" };
            var userId = 1L;
            var courseEntity = new CourseEntity { Id = 1L, TeacherId = userId };

            _mockUserService.Setup(x => x.GetUserInfos()).ReturnsAsync(userInfos);
            _mockMapper.Setup(x => x.Map<CourseEntity>(request)).Returns(courseEntity);
            _mockMapper.Setup(x => x.Map<CourseShortResponse>(courseEntity)).Returns(new CourseShortResponse());
            _mockFileService.Setup(x => x.ValidateImage(It.IsAny<Stream>()))
                .Returns((true, ".jpg"));
            _mockLocationService.Setup(x => x.GetCurrencyByUserLocation())
                .ReturnsAsync(new CurrencyByLocationDto { Code = "USD" });
            _mockCurrencyExchange.Setup(x => x.GetCurrencyRates(It.IsAny<CurrencyEnum>()))
                .ReturnsAsync(new RateExchangeDto { BRL = 5.0, USD = 1.0, EUR = 0.9 });

            // Act
            await _sut.CreateCourse(request);

            // Assert
            _mockStorageService.Verify(x => x.UploadCourseImage(
                It.IsAny<Stream>(),
                It.Is<string>(s => s.EndsWith(".jpg")),
                It.IsAny<Guid>()), Times.Once);
        }

        #endregion

        #region DeleteCourse Tests

        [Fact]
        public async Task DeleteCourse_ValidRequest_SoftDeletesCourse()
        {
            // Arrange
            var courseId = 1L;
            var userId = 1L;
            var course = new CourseEntity
            {
                Id = courseId,
                TeacherId = userId,
                Active = true,
                IsPublish = true,
                Thumbnail = "thumbnail.jpg"
            };
            var userInfos = new UserInfosDto { id = "xsRz", email = "test@test.com", userName = "TestUser" };

            _mockUnitOfWork.Setup(x => x.courseRead.CourseById(courseId, It.IsAny<bool>())).ReturnsAsync(course);
            _mockUserService.Setup(x => x.GetUserInfos()).ReturnsAsync(userInfos);

            // Act
            await _sut.DeleteCourse(courseId);

            // Assert
            course.Active.Should().BeFalse();
            course.IsPublish.Should().BeFalse();
            _mockStorageService.Verify(x => x.DeleteCourseImage(course.courseIdentifier, course.Thumbnail), Times.Once);
            _mockUnitOfWork.Verify(x => x.courseWrite.UpdateCourse(course), Times.Once);
            _mockUnitOfWork.Verify(x => x.Commit(), Times.Once);
            _mockDeleteCourseSender.Verify(x => x.SendMessage(courseId), Times.Once);
            _mockEmailService.Verify(x => x.SendEmail(
                userInfos.userName,
                userInfos.email,
                It.IsAny<string>(),
                It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeleteCourse_CourseNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var courseId = 999L;
            _mockUnitOfWork.Setup(x => x.courseRead.CourseById(courseId, It.IsAny<bool>()))
                .ReturnsAsync((CourseEntity)null);

            // Act
            Func<Task> act = async () => await _sut.DeleteCourse(courseId);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage(ResourceExceptMessages.COURSE_DOESNT_EXISTS);
        }

        [Fact]
        public async Task DeleteCourse_UserNotOwner_ThrowsUnauthorizedException()
        {
            // Arrange
            var courseId = 1L;
            var courseOwnerId = 1L;
            var currentUserId = 2L;
            var course = new CourseEntity { Id = courseId, TeacherId = courseOwnerId };
            var userInfos = new UserInfosDto { id = "xSdw", email = "test@test.com", userName = "TestUser" };

            _mockUnitOfWork.Setup(x => x.courseRead.CourseById(courseId, It.IsAny<bool>())).ReturnsAsync(course);
            _mockUserService.Setup(x => x.GetUserInfos()).ReturnsAsync(userInfos);

            // Act
            Func<Task> act = async () => await _sut.DeleteCourse(courseId);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedException>()
                .WithMessage(ResourceExceptMessages.COURSE_NOT_OF_USER);
        }

        [Fact]
        public async Task DeleteCourse_NoThumbnail_SkipsImageDeletion()
        {
            // Arrange
            var courseId = 1L;
            var userId = 1L;
            var course = new CourseEntity
            {
                Id = courseId,
                TeacherId = userId,
                Thumbnail = null
            };
            var userInfos = new UserInfosDto { id = SqidsBuild.GenerateRandomSqid(), email = "test@test.com", userName = "TestUser" };

            _mockUnitOfWork.Setup(x => x.courseRead.CourseById(courseId, It.IsAny<bool>())).ReturnsAsync(course);
            _mockUserService.Setup(x => x.GetUserInfos()).ReturnsAsync(userInfos);

            // Act
            await _sut.DeleteCourse(courseId);

            // Assert
            _mockStorageService.Verify(x => x.DeleteCourseImage(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region GetCourse Tests

        [Fact]
        public async Task GetCourse_ValidId_ReturnsCourseWithConvertedPrice()
        {
            // Arrange
            var courseId = 1L;
            var course = new CourseEntity
            {
                Id = courseId,
                Price = 100.0,
                CurrencyType = CurrencyEnum.USD,
                totalVisits = 10
            };
            var response = new CourseResponse { Price = 0 };

            _mockUnitOfWork.Setup(x => x.courseRead.CourseById(courseId, It.IsAny<bool>())).ReturnsAsync(course);
            _mockCoursesSession.Setup(x => x.GetCoursesVisited()).Returns(new List<long>());
            _mockMapper.Setup(x => x.Map<CourseResponse>(course)).Returns(response);
            _mockLocationService.Setup(x => x.GetCurrencyByUserLocation())
                .ReturnsAsync(new CurrencyByLocationDto { Code = "BRL" });
            _mockCurrencyExchange.Setup(x => x.GetCurrencyRates(CurrencyEnum.USD))
                .ReturnsAsync(new RateExchangeDto { BRL = 5.0, USD = 1.0, EUR = 0.9 });
            _mockLinkService.Setup(x => x.GenerateResourceLink(It.IsAny<string>(), It.IsAny<object>()))
                .Returns("http://test.com/modules");

            // Act
            var result = await _sut.GetCourse(courseId);

            // Assert
            result.Should().NotBeNull();
            result.Price.Should().Be(500.0); // 100 * 5.0
            _mockUnitOfWork.Verify(x => x.courseWrite.UpdateCourse(course), Times.Once);
            _mockUnitOfWork.Verify(x => x.Commit(), Times.Once);
        }

        [Fact]
        public async Task GetCourse_FirstVisit_IncrementsVisitCount()
        {
            // Arrange
            var courseId = 1L;
            var course = new CourseEntity
            {
                Id = courseId,
                totalVisits = 10,
                CurrencyType = CurrencyEnum.USD
            };

            _mockUnitOfWork.Setup(x => x.courseRead.CourseById(courseId, It.IsAny<bool>())).ReturnsAsync(course);
            _mockCoursesSession.Setup(x => x.GetCoursesVisited()).Returns(new List<long>());
            _mockMapper.Setup(x => x.Map<CourseResponse>(course)).Returns(new CourseResponse());
            _mockLocationService.Setup(x => x.GetCurrencyByUserLocation())
                .ReturnsAsync(new CurrencyByLocationDto { Code = "USD" });
            _mockCurrencyExchange.Setup(x => x.GetCurrencyRates(It.IsAny<CurrencyEnum>()))
                .ReturnsAsync(new RateExchangeDto { BRL = 5.0, USD = 1.0, EUR = 0.9 });
            _mockLinkService.Setup(x => x.GenerateResourceLink(It.IsAny<string>(), It.IsAny<object>()))
                .Returns("http://test.com");

            // Act
            await _sut.GetCourse(courseId);

            // Assert
            course.totalVisits.Should().Be(11);
            _mockCourseCache.Verify(x => x.SetCourseOnMostVisited(courseId), Times.Once);
            _mockCoursesSession.Verify(x => x.AddCourseVisited(courseId), Times.Once);
        }

        [Fact]
        public async Task GetCourse_AlreadyVisited_DoesNotIncrementVisitCount()
        {
            // Arrange
            var courseId = 1L;
            var course = new CourseEntity
            {
                Id = courseId,
                totalVisits = 10,
                CurrencyType = CurrencyEnum.USD
            };

            _mockUnitOfWork.Setup(x => x.courseRead.CourseById(courseId, It.IsAny<bool>())).ReturnsAsync(course);
            _mockCoursesSession.Setup(x => x.GetCoursesVisited()).Returns(new List<long> { courseId });
            _mockMapper.Setup(x => x.Map<CourseResponse>(course)).Returns(new CourseResponse());
            _mockLocationService.Setup(x => x.GetCurrencyByUserLocation())
                .ReturnsAsync(new CurrencyByLocationDto { Code = "USD" });
            _mockCurrencyExchange.Setup(x => x.GetCurrencyRates(It.IsAny<CurrencyEnum>()))
                .ReturnsAsync(new RateExchangeDto { BRL = 5.0, USD = 1.0, EUR = 0.9 });
            _mockLinkService.Setup(x => x.GenerateResourceLink(It.IsAny<string>(), It.IsAny<object>()))
                .Returns("http://test.com");

            // Act
            await _sut.GetCourse(courseId);

            // Assert
            course.totalVisits.Should().Be(10);
            _mockCourseCache.Verify(x => x.SetCourseOnMostVisited(courseId), Times.Never);
            _mockCoursesSession.Verify(x => x.AddCourseVisited(courseId), Times.Never);
        }

        #endregion

        #region UpdateCourse Tests

        [Fact]
        public async Task UpdateCourse_ValidRequest_UpdatesCourseSuccessfully()
        {
            // Arrange
            var courseId = 1L;
            var userId = 1L;
            var request = new UpdateCourseRequest { Thumbnail = null };
            var userInfos = new UserInfosDto { id = SqidsBuild.Build().Encode(userId) };
            var course = new CourseEntity { Id = courseId, TeacherId = userId, Thumbnail = "old.jpg" };
            var response = new CourseShortResponse();

            _mockUserService.Setup(x => x.GetUserInfos()).ReturnsAsync(userInfos);
            _mockUnitOfWork.Setup(x => x.courseRead.CourseByTeacherAndId(userId, courseId))
                .ReturnsAsync(course);
            _mockMapper.Setup(x => x.Map(request, course)).Returns(course);
            _mockMapper.Setup(x => x.Map<CourseShortResponse>(course)).Returns(response);
            _mockStorageService.Setup(x => x.GetCourseImage(It.IsAny<Guid>(), It.IsAny<string>()))
                .ReturnsAsync("http://image.url");

            // Act
            var result = await _sut.UpdateCourse(courseId, request);

            // Assert
            result.Should().NotBeNull();
            _mockUnitOfWork.Verify(x => x.courseWrite.UpdateCourse(course), Times.Once);
            _mockUnitOfWork.Verify(x => x.Commit(), Times.Once);
        }

        [Fact]
        public async Task UpdateCourse_UserNotAuthenticated_ThrowsException()
        {
            // Arrange
            var courseId = 1L;
            var request = new UpdateCourseRequest();
            _mockUserService.Setup(x => x.GetUserInfos()).ReturnsAsync((UserInfosDto)null);

            // Act
            Func<Task> act = async () => await _sut.UpdateCourse(courseId, request);

            // Assert
            await act.Should().ThrowAsync<NullReferenceException>();
        }

        [Fact]
        public async Task UpdateCourse_CourseNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var courseId = 1L;
            var userId = 1L;
            var request = new UpdateCourseRequest();
            var userInfos = new UserInfosDto { id = SqidsBuild.GenerateRandomSqid() };

            _mockUserService.Setup(x => x.GetUserInfos()).ReturnsAsync(userInfos);
            _mockUnitOfWork.Setup(x => x.courseRead.CourseByTeacherAndId(userId, courseId))
                .ReturnsAsync((CourseEntity)null);

            // Act
            Func<Task> act = async () => await _sut.UpdateCourse(courseId, request);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage(ResourceExceptMessages.COURSE_NOT_OF_USER);
        }

        #endregion

        #region UserGotCourse Tests

        [Fact]
        public async Task UserGotCourse_UserHasCourse_ReturnsTrue()
        {
            // Arrange
            var courseId = 1L;
            var userId = 1;
            var request = new GetCourseRequest { courseId = SqidsBuild.GenerateRandomSqid() };
            var userInfos = new UserInfosDto { id = SqidsBuild.GenerateRandomSqid() };
            var course = new CourseEntity { Id = courseId };

            _mockUserService.Setup(x => x.GetUserInfos()).ReturnsAsync(userInfos);
            _mockUnitOfWork.Setup(x => x.courseRead.CourseById(It.IsAny<long>(), It.IsAny<bool>())).ReturnsAsync(course);
            _mockUnitOfWork.Setup(x => x.enrollmentRead.UserGotCourse(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(true);

            // Act
            var result = await _sut.UserGotCourse(request);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UserGotCourse_UserDoesNotHaveCourse_ReturnsFalse()
        {
            // Arrange
            var courseId = 1L;
            var userId = 1L;
            var request = new GetCourseRequest { courseId = SqidsBuild.Build().Encode(courseId) };
            var userInfos = new UserInfosDto { id = SqidsBuild.GenerateRandomSqid() };
            var course = new CourseEntity { Id = courseId };

            _mockUserService.Setup(x => x.GetUserInfos()).ReturnsAsync(userInfos);
            _mockUnitOfWork.Setup(x => x.courseRead.CourseById(courseId, It.IsAny<bool>())).ReturnsAsync(course);
            _mockUnitOfWork.Setup(x => x.enrollmentRead.UserGotCourse(It.IsAny<long>(), It.IsAny<long>()))
                .ReturnsAsync(false);

            // Act
            var result = await _sut.UserGotCourse(request);

            // Assert
            result.Should().BeFalse();
        }

        #endregion
    }
}
