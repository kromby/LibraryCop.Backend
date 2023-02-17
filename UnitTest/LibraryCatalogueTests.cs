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

namespace UnitTest
{
    public class LibraryCatalogueTests
    {
        private readonly Mock<ILibraryCatalogoueDataAccess> _libraryCatalogueDaMock;
        private readonly Mock<ILogger<LibraryCatalogueInteractor>> _interactorLog;

        public LibraryCatalogueTests()
        {
            _libraryCatalogueDaMock = new Mock<ILibraryCatalogoueDataAccess>(MockBehavior.Strict) { CallBase = true };
            _interactorLog = new Mock<ILogger<LibraryCatalogueInteractor>>();
        }

        [Fact] 
        public void ConstructorHappyTest() 
        {
            // ARRANGE
            

            // ACT
            var interactor = new LibraryCatalogueInteractor(_libraryCatalogueDaMock.Object, _interactorLog.Object);

            // ASSERT
            Assert.NotNull(interactor);
        }

        [Fact]
        public void ConstructorExceptionTest()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
           
            // ARRANGE


            // ACT
            Assert.Throws<ArgumentNullException>(() => new LibraryCatalogueInteractor(null, null));
            Assert.Throws<ArgumentNullException>(() => new LibraryCatalogueInteractor(_libraryCatalogueDaMock.Object, null));

            // ASSERT

#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [Theory]
        [InlineData("9789935117540")]
        [InlineData("9979203447")]
        public void AddBookHappyTest(string isbn)
        {
            // ARRANGE
            _libraryCatalogueDaMock.Setup(l => l.SaveBookState(It.IsAny<BookState>())).Verifiable();
            var interactor = new LibraryCatalogueInteractor(_libraryCatalogueDaMock.Object, _interactorLog.Object);

            // ACT
            var entity = interactor.AddBook(isbn, Guid.NewGuid(), Guid.NewGuid());

            // ASSERT
            Assert.NotNull(entity);
        }

        [Fact]
        public void AddBookNullTest()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

            // ARRANGE
            var interactor = new LibraryCatalogueInteractor(_libraryCatalogueDaMock.Object, _interactorLog.Object);

            // ACT
            Assert.ThrowsAsync<ArgumentNullException>(() => interactor.AddBook(null, new Guid(), new Guid()));
            Assert.ThrowsAsync<ArgumentNullException>(() => interactor.AddBook("isbn", new Guid(), new Guid()));
            Assert.ThrowsAsync<ArgumentNullException>(() => interactor.AddBook("isbn", Guid.NewGuid(), new Guid()));

            Assert.ThrowsAsync<ArgumentException>(() => interactor.AddBook("isbn", Guid.NewGuid(), Guid.NewGuid()));

            // ASSERT

#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }
    }
}
