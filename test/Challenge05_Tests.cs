﻿using ds_challenge_05.Controllers;
using System;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
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

    /* 
        In the mssql-container terminal, run these commands to set up the test DB and table:
        $ /opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P P@ssword1 -q "select @@Version"
        > USE TABLE TESTDB;
        > CREATE TABLE dbo.Inventory (id INT,name NVARCHAR (50),quantity INT,PRIMARY KEY (id));
        > INSERT INTO dbo.Inventory VALUES (1, 'banana', 150); INSERT INTO dbo.Inventory VALUES (2, 'orange', 154);
        > GO
        > SELECT * FROM dbo.Inventory;
        > SELECT * FROM dbo.Inventory WHERE quantity > 151;
        > EXIT
    */
    private const string DbName = "TESTDB";
    private const string MasterConnectionString = "Server=localhost,1433;Database=master;User Id=sa;Password=P@ssword1;TrustServerCertificate=True;";
    private const string TestDbConnectionString = $"Server=localhost,1433;Database={DbName};User Id=sa;Password=P@ssword1;TrustServerCertificate=True;";

    private async Task EnsureDatabaseExists()
    {
        using var connection = new SqlConnection(MasterConnectionString);
        await connection.OpenAsync();

        var checkDbCommand = new SqlCommand($"IF DB_ID('{DbName}') IS NULL CREATE DATABASE {DbName}", connection);
        await checkDbCommand.ExecuteNonQueryAsync();
    }

    [Fact(DisplayName = "Test Database Connection")]
    public async Task ConnectToMssqlAndSelectAll()
    {
        await EnsureDatabaseExists();

        using var connection = new SqlConnection(TestDbConnectionString);
        await connection.OpenAsync();

        // Create a table for testing
        using var createTableCmd = new SqlCommand("IF OBJECT_ID('dbo.Inventory', 'U') IS NULL CREATE TABLE dbo.Inventory (id INT, name NVARCHAR(50), quantity INT);", connection);
        await createTableCmd.ExecuteNonQueryAsync();

        // Clear the table before inserting new data
        using var clearTableCmd = new SqlCommand("DELETE FROM dbo.Inventory;", connection);
        await clearTableCmd.ExecuteNonQueryAsync();

        // Insert some data
        using var insertCmd = new SqlCommand("INSERT INTO dbo.Inventory VALUES (1, 'banana', 150); INSERT INTO dbo.Inventory VALUES (2, 'orange', 154);", connection);
        await insertCmd.ExecuteNonQueryAsync();

        // Select and Print data from the table        
        Console.WriteLine("Data in Inventory table:");
        using var readCmd = new SqlCommand("SELECT id, name, quantity FROM dbo.Inventory", connection);
        using var dataReader = await readCmd.ExecuteReaderAsync();
        if (dataReader.HasRows) 
        {
            while (dataReader.Read())
            {
                Console.WriteLine($"Id: {dataReader.GetInt32(0)}, Name: {dataReader.GetString(1)}, Quantity: {dataReader.GetInt32(2)}");
            }
        }

        Assert.True(dataReader.HasRows, "The query should return rows.");
    }

}