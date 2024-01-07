using Microsoft.AspNetCore.Mvc;
using zad3_apbd.Properties.Models;

namespace zad3_apbd.Controllers;

[ApiController]
[Route("[controller]")]
public class StudentsController : ControllerBase
{
    private readonly ILogger<StudentsController> _logger;

    private readonly string _csvFilePath;
    public StudentsController(ILogger<StudentsController> logger)
    {
        _logger = logger;
        _csvFilePath = Path.Combine(Environment.CurrentDirectory, "db.csv");
    }

    [HttpGet(Name = "GetStudents")]
    public IEnumerable<Student> GetAllStudents()
    {
        return GetAllStudentsFromDB();
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

    private IEnumerable<Student> GetAllStudentsFromDB()
    {
        // Każdy wiersz reprezentuje pojedynczego studenta
        // Wczytywanie danych z pliku csv po linijce
         var lines = System.IO.File.ReadLines(_csvFilePath);
        
        var students = new List<Student>();

        foreach (var line in lines)
        {
            //Każda kolumna jest oddzielona znakiem ","
            var separatedComma = line.Split(",");
            
            if (separatedComma.Length != 9)
            {
                //Zapis logów błędów do pliku
                Console.WriteLine(line);
                continue;
            }
            
            var student = new Student
            {
                FirstName = separatedComma[0],
                LastName = separatedComma[1],
                IndexNumber = separatedComma[2],
                BirthDate = separatedComma[3],
                StudiesName = separatedComma[4],
                StudiesMode = separatedComma[5],
                Email = separatedComma[6],
                MothersName = separatedComma[7],
                FathersName = separatedComma[8],
            };
            //dodać studenta do listy studentów
            students.Add(student);
        }
        return students;
    }
    
}