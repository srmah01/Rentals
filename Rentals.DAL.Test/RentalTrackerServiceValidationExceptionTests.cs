using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rentals.DAL.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace Rentals.DAL.Tests
{
    /// <summary>
    /// Test class for the RentalsServiceValidationException
    /// </summary>
    [TestClass]
    public class RentalsServiceValidationExceptionTest
    {
        [TestMethod]
        public void CanCreateDefaultException()
        {
            var exception = new RentalsServiceValidationException();

            Assert.IsNotNull(exception);
        }

        [TestMethod]
        public void CanCreateExceptionWithMessage()
        {
            var  message = "Error Message";
            var exception = new RentalsServiceValidationException(message);

            Assert.IsNotNull(exception);
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(0, exception.ValidationResults.Count());
        }

        [TestMethod]
        public void CanCreateExceptionWithMessageAndListOfErrors()
        {
            var message = "Error Message";
            var errors = new List<ValidationResult>()
            {
                new ValidationResult("Message 1", new [] { "Property" }),
                new ValidationResult("Message 2", new [] { "Property1", "Property2" }),
            };
            var exception = new RentalsServiceValidationException(message, errors);

            Assert.IsNotNull(exception);
            Assert.AreEqual(errors.Count, exception.ValidationResults.Count());
        }
    }
}
