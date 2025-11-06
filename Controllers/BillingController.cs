using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persol.Marketplace.Data;

namespace Persol.Marketplace.Controllers;

/// <summary>
/// API controller for billing operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication for all endpoints
public class BillingController : ControllerBase
{
    private readonly BillingDbContext _context;
    private readonly ILogger<BillingController> _logger;

    public BillingController(BillingDbContext context, ILogger<BillingController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get health status of billing database connection
    /// </summary>
    /// <returns>Database connection status</returns>
    [HttpGet("health")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHealth()
    {
        try
        {
            // Test database connection
            var canConnect = await _context.Database.CanConnectAsync();
            
            return Ok(new
            {
                status = "healthy",
                database = canConnect ? "connected" : "disconnected",
                schema = "billings",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking billing database health");
            return StatusCode(500, new
            {
                status = "unhealthy",
                error = ex.Message,
                timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Example endpoint - Get all billing records (placeholder)
    /// </summary>
    /// <remarks>
    /// This is a placeholder endpoint. Add your DbSet properties to BillingDbContext
    /// and implement your actual billing logic here.
    /// </remarks>
    /// <returns>List of billing records</returns>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult GetBillingRecords()
    {
        // Example: When you add DbSet<BillingRecord> to BillingDbContext, you can do:
        // var records = await _context.BillingRecords.ToListAsync();
        // return Ok(records);

        return Ok(new
        {
            message = "Billing records endpoint - ready for implementation",
            schema = "billings",
            note = "Add your DbSet properties to BillingDbContext and implement your logic here"
        });
    }

    /// <summary>
    /// Get database schema information
    /// </summary>
    /// <returns>Schema information</returns>
    [HttpGet("schema-info")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult GetSchemaInfo()
    {
        var connectionString = _context.Database.GetConnectionString();
        var maskedConnectionString = connectionString != null 
            ? Regex.Replace(connectionString, @"password=[^;]*", "password=***", RegexOptions.IgnoreCase)
            : "not available";

        return Ok(new
        {
            schemaName = "billings",
            connectionString = maskedConnectionString,
            provider = _context.Database.ProviderName,
            note = "Add your entities and DbSet properties to BillingDbContext"
        });
    }
}

