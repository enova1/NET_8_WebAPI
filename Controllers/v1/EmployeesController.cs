using ExampleLibrary;
using Models.Employee;
using System.Linq;


namespace WebApi.Controllers.v1;

/// <summary>
/// Employee Controller for the API.
/// </summary>
[Produces("application/json")]
[ApiController]
[Route("v1/[controller]")]
public class EmployeesController : Controller
{
    private readonly IEmployee _employeeService;

    /// <inheritdoc />
    public EmployeesController(IEmployee employee)
    {
        _employeeService = employee;
    }

    /// <summary>
    /// Get all employees from the database. 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetEmployeesTask()
    {
        var employees = await _employeeService.GetEmployeesTask();
        return employees.Count == 0 ? StatusCode(200, "No employees found.") : StatusCode(200, employees);
    }

    /// <summary>
    /// Filter the employees by phone number and zip code and display the
    /// results in the view Index view.
    /// </summary>
    /// <param name="phone"></param>
    /// <param name="zipCode"></param>
    /// <returns></returns>
    [HttpGet("filterBy/")]
    public async Task<IActionResult> FilterEmployeesTask(string phone, string zipCode)
    {
        var employees = await _employeeService.FilterEmployeesTask(phone, zipCode);
        return employees.Count == 0 ? StatusCode(200, "No employees found.") : StatusCode(200, employees);
    }

    /// <summary>
    /// Display the list of employees with their full name, earliest hire date,
    /// latest hire date, and average length of employment in years. 
    /// </summary>
    /// <returns></returns>
    [HttpGet("averageEmployment/")]
    public async Task<IActionResult> EmployeesAverageLengthTask()
    {
        var data = await _employeeService.GetEmployeesTask();
        var employees = data
            .Select(e => new
            {
                FullName = $"{e.FirstName} {e.LastName}",
                EarliestHireDate = e.HireDate,
                LatestHireDate = e.HireDate,
                AverageLengthOfEmployment = (DateTime.Now - (DateTime.TryParse(e.HireDate, out var dt) ? dt : DateTime.MinValue)).TotalDays / 365
           }).ToList();
        return employees.Count == 0 ? StatusCode(200, "No employees found.") : StatusCode(200, employees);
    } 

    /// <summary>
    /// Get an employee by their employeeId from the database.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("id")]
    public async Task<IActionResult> GetEmployeeDetailsTask(int id)
    {
        if (id == 0) return BadRequest(id);
        var data = await _employeeService.GetEmployeesTask();
        var employees = data.Find(x => x.EmployeeId == id);
        return employees == null ? StatusCode(200, "No employees found.") : StatusCode(200, employees);
    }
    
    /// <summary>
    /// Create a new employee in the database.
    /// </summary>
    /// <param name="employees"></param>
    /// <returns></returns>
    [HttpPost()]
    public async Task<IActionResult> CreateEmployee([FromBody] Employees employees)
    {
        try
        {
            var result = await _employeeService.CreateEmployee(employees);
            return result == null ? StatusCode(200, "Employee was NOT created") : StatusCode(200, result);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    /// <summary>
    /// save the edited employee in the database.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    [HttpPut, HttpPatch]
    public async Task<IActionResult> EditEmployee(Employees data)
    {
        try
        {
            var result = await _employeeService.EditEmployee(data);
            return result == null ? StatusCode(200, "No employees found.") : StatusCode(200, result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }

    }


    /// <summary>
    /// Delete an employee from the database.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("id")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        try
        {
            var result = await _employeeService.DeleteEmployee(id);
            return result == false ? StatusCode(200, "No employees found.") : StatusCode(200, $"Employee id {id} has been deleted.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

}