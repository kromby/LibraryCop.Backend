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
        private readonly Mock<ILogger<BookFinderInteractor>> _interactorLog;

        public BookFinderUnitTests()
        {
            _bookFinderDaNotFoundMock = new Mock<IBookFinderDataAccess> { CallBase = true };
            _bookFinderDaMock = new Mock<IBookFinderDataAccess>() { CallBase = true };
            _interactorLog = new Mock<ILogger<BookFinderInteractor>>();
        }

        [Fact]
        public async void GetBookHappyTest()
        {
            // ARRANGE
            string isbn = "547946546";
            _bookFinderDaNotFoundMock.Setup(f => f.GetBook(It.IsAny<string>()));
            _bookFinderDaNotFoundMock.Setup(f => f.Priority).Returns(1);
            _bookFinderDaMock.Setup(f => f.GetBook(It.IsAny<string>())).ReturnsAsync(new Book() { ISBN = isbn});
            _bookFinderDaMock.Setup(f => f.Priority).Returns(3);
            var list = new List<IBookFinderDataAccess>
            {
                _bookFinderDaNotFoundMock.Object,
                _bookFinderDaMock.Object
            };
            BookFinderInteractor interactor = new(list, _interactorLog.Object);            

            // ACT
            var result = await interactor.GetBook(isbn);

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal(isbn, result.ISBN);
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
            var result = new BookFinderInteractor(list, _interactorLog.Object);

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
                () => new BookFinderInteractor(null, null));

            _ = Assert.Throws<ArgumentNullException>(
                () => new BookFinderInteractor(list, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }
    }
}