﻿using SLORM.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SLORM.Application.UnitTests.Exceptions
{
    public class UnknownSQLProviderExceptionTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password = myPassword;")]
        public void Constructor_WhenInstanciated_ShouldReturnInstance(string connectionString)
        {
            // Arrange
            // Act
            var exception = new UnknownSQLProviderException(connectionString);
            // Assert
            Assert.NotNull(exception);
        }
    }
}
