using Microsoft.AspNetCore.Mvc;
using zad3_apbd.Properties.Models;

namespace zad3_apbd.Controllers;

[ApiController]
[Route("[controller]")]
public class StudentsController : ControllerBase
{

    private readonly ILogger<StudentsController> _logger;

    public StudentsController(ILogger<StudentsController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetStudents")]
    public IEnumerable<Student> GetAllStudents()
    {
        return new List<Student>();
    }
    
    [HttpGet("{indexNumber}", Name = "GetStudent")]
    public Student GetStudent(string indexNumber)
    {
        return new Student();
    }
    
    [HttpPost(Name = "CreateStudent")]
    public IActionResult CreateStudent(Student student)
    {
        return Ok();
    }
    
    [HttpPut("{indexNumber}", Name = "UpdateStudent")]
    public IActionResult UpdateStudent(string indexNumber, Student student)
    {
        return Ok();
    }
    
    [HttpDelete("{indexNumber}", Name = "DeleteStudent")]
    public IActionResult DeleteStudent(string indexNumber)
    {
        return Ok();
    }
    
}