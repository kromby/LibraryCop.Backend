using BusinessLogic;
using LibraryCop.BusinessLogic;
using LibraryCop.BusinessLogic.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace UnitTest
{
    public class BookManagementUnitTests
    {
        private readonly Mock<ILogger<BookManagementInteractor>> _interactorLog;
        private readonly Mock<IBookManagementDataAccess> _bookMgmtDataAccess;

        public BookManagementUnitTests()
        {
            _interactorLog = new Mock<ILogger<BookManagementInteractor>>();
            _bookMgmtDataAccess = new Mock<IBookManagementDataAccess>();
        }

        [Fact]
        public async void SaveBookOkTest()
        {
            // ARRANGE
            string publisher = "Publisher";
            _bookMgmtDataAccess.Setup(x => x.SaveBook(It.Is<Book>(b => b.Publisher == publisher))).Verifiable();
            BookManagementInteractor interactor = new(_bookMgmtDataAccess.Object, _interactorLog.Object);

            // ACT
            await interactor.SaveBook("Title", "Author", publisher, 1900);

            // ASSERT
            _bookMgmtDataAccess.Verify();
        }

        [Theory]
        [InlineData("", "", "")]
        [InlineData("  ", "  ", "  ")]
        [InlineData("Title", "", "")]
        [InlineData("Title", "Author", "")]
        public void SaveBookArgumentNullExceptionTest(string title, string author, string publisher)
        {
            // ARRANGE
            BookManagementInteractor interactor = new(_bookMgmtDataAccess.Object, _interactorLog.Object);

            // ACT & ASSERT
            Assert.ThrowsAsync<ArgumentNullException>(async () => await interactor.SaveBook(title, author, publisher, 0));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1500)]
        [InlineData(2030)]
        public void SaveBookArgumentExceptionTest(int year)
        {
            // ARRANGE
            BookManagementInteractor interactor = new(_bookMgmtDataAccess.Object, _interactorLog.Object);

            // ACT & ASSERT
            Assert.ThrowsAsync<ArgumentException>(async () => await interactor.SaveBook("Tit", "Aut", "Pub", year));
        }
    }
}
