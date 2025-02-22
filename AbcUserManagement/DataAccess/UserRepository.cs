﻿using AbcUserManagement.Models;
using Dapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace AbcUserManagement.DataAccess
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(IDbConnection dbConnection, ILogger<UserRepository> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Getting user by ID: {Id}", id);
                var sql = "SELECT Id, Username, Password AS PasswordHash, Role, CompanyId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate FROM Users WHERE Id = @Id";
                return await _dbConnection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetUsersByCompanyIdAsync(int companyId)
        {
            try
            {
                _logger.LogInformation("Getting users by company ID: {CompanyId}", companyId);
                var sql = "SELECT Id, Username, Password AS PasswordHash, Role, CompanyId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate FROM Users WHERE CompanyId = @CompanyId";
                return await _dbConnection.QueryAsync<User>(sql, new { CompanyId = companyId }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users by company ID: {CompanyId}", companyId);
                throw;
            }
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            try
            {
                _logger.LogInformation("Getting user by username: {Username}", username);
                var sql = "SELECT Id, Username, Password AS PasswordHash, Role, CompanyId, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate FROM Users WHERE Username = @Username";
                return await _dbConnection.QuerySingleOrDefaultAsync<User>(sql, new { Username = username }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username: {Username}", username);
                throw;
            }
        }

        public async Task AddUserAsync(User user)
        {
            try
            {
                _logger.LogInformation("Adding user: {Username}", user.Username);
                var sql = @"
                    INSERT INTO Users (Username, Password, Role, CompanyId, CreatedBy, CreatedDate)
                    VALUES (@Username, @PasswordHash, @Role, @CompanyId, @CreatedBy, @CreatedDate);
                    SELECT CAST(SCOPE_IDENTITY() as int)";
                var id = await _dbConnection.QuerySingleAsync<int>(sql, new
                {
                    user.Username,
                    user.PasswordHash,
                    Role = user.Role.ToString(),
                    user.CompanyId,
                    user.CreatedBy,
                    user.CreatedDate
                }).ConfigureAwait(false);
                user.Id = id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user: {Username}", user.Username);
                throw;
            }
        }

        public async Task UpdateUserAsync(User user)
        {
            try
            {
                _logger.LogInformation("Updating user: {Id}", user.Id);
                var sql = @"
                    UPDATE Users
                    SET Username = @Username,
                        Password = @PasswordHash,
                        Role = @Role,
                        CompanyId = @CompanyId,
                        ModifiedBy = @ModifiedBy,
                        ModifiedDate = @ModifiedDate
                    WHERE Id = @Id";
                await _dbConnection.ExecuteAsync(sql, new
                {
                    user.Id,
                    user.Username,
                    user.PasswordHash,
                    Role = user.Role.ToString(),
                    user.CompanyId,
                    user.ModifiedBy,
                    user.ModifiedDate
                }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {Id}", user.Id);
                throw;
            }
        }

        public async Task DeleteUserAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting user: {Id}", id);
                var sql = "DELETE FROM Users WHERE Id = @Id";
                await _dbConnection.ExecuteAsync(sql, new { Id = id }).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user: {Id}", id);
                throw;
            }
        }
    }
}