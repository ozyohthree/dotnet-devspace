﻿using ds_challenge_05.Controllers;
using System;
using System.Data.SqlClient;
using Xunit;

namespace Challenge5Tests;

public class Challenge05_Tests
{

    [Fact(DisplayName = "Test ChallengeMethod with a long enough string")]
    public void ChallengeMethod_WithSufficientLengthString_ReturnsFifthCharacter()
    {
        // Arrange
        var controller = new ChallengeMethodController();
        var input = "OpenShift DevSpaces";
        var expected = $"The Fifth Character in \"{input}\" is [S]\n";

        // Act
        var result = controller.ChallengeMethod(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact(DisplayName = "Test ChallengeMethod with a short string")]
    public void ChallengeMethod_WithShortString_ReturnsLengthErrorMessage()
    {
        // Arrange
        var controller = new ChallengeMethodController();
        var input = "four";
        var expected = "String is shorter than length 5 \n";

        // Act
        var result = controller.ChallengeMethod(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ChallengeMethod_ReturnsFifthCharacter_WhenStringLongEnough()
    {
        var controller = new ChallengeMethodController();
        string result = controller.ChallengeMethod("OpenShift DevSpaces");
        Assert.Contains("The Fifth Character", result);
        Assert.Contains("[S]", result); // "OpenShift DevSpaces"[4] == 'S'
    }

    [Fact]
    public void ChallengeMethod_ReturnsError_WhenStringTooShort()
    {
        var controller = new ChallengeMethodController();
        string result = controller.ChallengeMethod("Test");
        Assert.Equal("String is shorter than length 5 \n", result);
    }

    [Fact]
    public void ChallengeMethod_Overload_ReturnsCharacterAtIndex()
    {
        var controller = new ChallengeMethodController();
        string result = controller.ChallengeMethod("OpenShift", 2);
        Assert.Equal("The Character at index [2] in \"OpenShift\" is [e]\n", result);
    }

    [Fact]
    public void ChallengeMethod_Overload_ReturnsOutOfBounds_WhenIndexInvalid()
    {
        var controller = new ChallengeMethodController();
        string result = controller.ChallengeMethod("OpenShift", 20);
        Assert.Equal("Index [20] is out of bounds for \"OpenShift\"\n", result);
    }

    /* In the mssql-container terminal, run these commands to set up the test DB and table:
    $ /opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P P@ssword1
    > USE TABLE TESTDB;
    > CREATE TABLE dbo.Inventory (id INT,name NVARCHAR (50),quantity INT,PRIMARY KEY (id));
    > INSERT INTO dbo.Inventory VALUES (1, 'banana', 150); INSERT INTO dbo.Inventory VALUES (2, 'orange', 154);
    > GO
    > SELECT * FROM dbo.Inventory;
    > SELECT * FROM dbo.Inventory WHERE quantity > 151;
    > EXIT
    */
    [Fact(DisplayName = "Connects to MSSQL 'test' DB and does SELECT *")]
    public void ConnectToMssqlAndSelectAll()
    {
        // Connection string for local SQL Server
        var connectionString = "Server=localhost,1433;Database=test;User Id=sa;Password=P@ssword1;TrustServerCertificate=True;";
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        // Use INFORMATION_SCHEMA.TABLES for a generic SELECT *
        using var command = new SqlCommand("SELECT * FROM dbo.Inventory", connection);
        using var reader = command.ExecuteReader();

        // Assert that at least one row is returned
        Assert.True(reader.HasRows, "No tables found in the database.");
    }
}