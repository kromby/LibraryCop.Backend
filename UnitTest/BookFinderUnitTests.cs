using BusinessLogic;
using LibraryCop.BusinessLogic;
using LibraryCop.BusinessLogic.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;

namespace UnitTest
{
    public class BookFinderUnitTests
    {
        private readonly Mock<IBookFinderDataAccess> _bookFinderDaNotFoundMock;
        private readonly Mock<IBookFinderDataAccess> _bookFinderDaMock;
        private readonly Mock<IBookManagementDataAccess> _bookManagementDaMock;
        private readonly Mock<LibraryCatalogueInteractor> _libraryCatalogueInteractorMock;
        private readonly Mock<ILogger<BookFinderInteractor>> _interactorLog;

        public BookFinderUnitTests()
        {
            _bookFinderDaNotFoundMock = new Mock<IBookFinderDataAccess> { CallBase = true };
            _bookFinderDaMock = new Mock<IBookFinderDataAccess>() { CallBase = true };
            _bookManagementDaMock = new Mock<IBookManagementDataAccess>(MockBehavior.Strict) { CallBase = true };
            Mock<ILibraryCatalogueDataAccess> libraryCatalogueDataAccess = new();
            Mock<ILogger<LibraryCatalogueInteractor>> libraryCatalogueLogger = new();
            _libraryCatalogueInteractorMock = new Mock<LibraryCatalogueInteractor>(libraryCatalogueDataAccess.Object, libraryCatalogueLogger.Object) { CallBase = true };
            _interactorLog = new Mock<ILogger<BookFinderInteractor>>();
        }

        [Fact]
        public async void GetBookHappyTest()
        {
            // ARRANGE
            User user = new(Guid.NewGuid(), Guid.NewGuid(), "user");
            Book book = new("547946546", false, user.UserID);
            Guid libraryID = Guid.NewGuid();
            _bookFinderDaNotFoundMock.Setup(f => f.GetBook(It.IsAny<string>()));
            _bookFinderDaNotFoundMock.Setup(f => f.Priority).Returns(1);
            _bookFinderDaMock.Setup(f => f.GetBook(book.ISBN)).ReturnsAsync(book);
            _bookFinderDaMock.Setup(f => f.Priority).Returns(3);
            var list = new List<IBookFinderDataAccess>
            {
                _bookFinderDaNotFoundMock.Object,
                _bookFinderDaMock.Object
            };
            _bookManagementDaMock.Setup(m => m.SaveBook(book)).Returns(Task.Delay(100)).Verifiable();

            BookFinderInteractor interactor = new(list, _bookManagementDaMock.Object, _libraryCatalogueInteractorMock.Object, _interactorLog.Object);            

            // ACT
            var result = await interactor.GetBook(book.ISBN, user);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(book.ISBN, result.ISBN);
            _bookFinderDaNotFoundMock.Verify();
            _bookFinderDaMock.Verify();
            _bookManagementDaMock.Verify();
        }

        [Fact]
        public void ConstructorHappyTest()
        {
            // ARRANGE
            var list = new List<IBookFinderDataAccess>
            {
                _bookFinderDaMock.Object
            };

            // ACT
            var result = new BookFinderInteractor(list, _bookManagementDaMock.Object, _libraryCatalogueInteractorMock.Object, _interactorLog.Object);

            // ASSERT
            Assert.NotNull(result);
        }

            [Fact]
        public void ConstructorArgExTest()
        {
            // ARRANGE
            var list = new List<IBookFinderDataAccess>
            {
                _bookFinderDaMock.Object
            };

            // ACT & ASSERT
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _ = Assert.Throws<ArgumentNullException>(
                () => new BookFinderInteractor(null, null, _libraryCatalogueInteractorMock.Object, null));

            _ = Assert.Throws<ArgumentNullException>(
                () => new BookFinderInteractor(list, null, _libraryCatalogueInteractorMock.Object, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }
    }
}