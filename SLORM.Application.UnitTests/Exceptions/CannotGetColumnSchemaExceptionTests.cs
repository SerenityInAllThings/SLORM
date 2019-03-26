using SLORM.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SLORM.Application.UnitTests.Exceptions
{
    public class CannotGetColumnSchemaExceptionTests
    {
        [Fact]
        public void Constructor_WhenParameterless_ShouldReturnInstance()
        {
            // Act
            var exception = new CannotGetColumnSchemaException();
            // Assert
            Assert.NotNull(exception);
        }

        [Fact]
        public void Constructor_WhenInstanciated_ShouldReturnExceptionDeviredClass()
        {
            // Act
            var exception = new CannotGetColumnSchemaException();
            // Assert
            Assert.True(exception is Exception);
        }
    }
}
