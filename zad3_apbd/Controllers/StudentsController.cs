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
    public IActionResult GetStudent(string indexNumber)
    {
        var student = GetAllStudentsFromDB().FirstOrDefault(student => 
            student.IndexNumber == indexNumber);
        return student == null ? NotFound($"Nie znaleziono studenta o podanym indeksie: {indexNumber}") : Ok(student);
    }

    [HttpPost(Name = "CreateStudent")]
    public async Task<IActionResult> CreateStudent(Student student)
    {
        //czy taki student istnieje
        if (IsStudentValid(student) == false)
        {
            return BadRequest("Nieprawidłowe zapytanie do api");
        }
        
        //czy nr indekus się nie powtarza z jakimś istniejącym
        if (GetAllStudentsFromDB().Any(s => s.IndexNumber == student.IndexNumber))
        {
            return BadRequest($"Student o podanym indeksie już istnieje: {student.IndexNumber}");
        }
        
        await AddNewStudentToDB(student);
        
        return Ok();
    }
    
    private bool IsStudentValid(Student student)
    {
        if (student == null)
        {
            return false;
        }

        if ((student.FirstName == null || student.FirstName.Length == 0)
            || (student.LastName == null || student.LastName.Length == 0)
            || (student.IndexNumber == null || student.IndexNumber.Length == 0)
            || (student.BirthDate == null || student.BirthDate.Length == 0)
            || (student.StudiesName == null || student.StudiesName.Length == 0)
            || (student.StudiesMode == null || student.StudiesMode.Length == 0)
            || (student.Email == null || student.Email.Length == 0)
            || (student.FathersName == null || student.FathersName.Length == 0)
            || (student.MothersName == null || student.MothersName.Length == 0))
        {
            return false;
        }
        return true;
    }

    [HttpPut("{indexNumber}", Name = "UpdateStudent")]
    public IActionResult UpdateStudent(string indexNumber, Student student)
    {
        //czy taki student istnieje
        if (GetAllStudentsFromDB().Any(s => s.IndexNumber == indexNumber) == false)
        {
            return NotFound($"Nie znaleziono studenta o podanym indeksie: {indexNumber}");
        }
        
        //usuwanie studenta z bazy danych
        DeleteStudentFromDB(indexNumber);
        
        //dodanie studenta do bazy danych
        student.IndexNumber = indexNumber; //biorę domyślny indexNumber, nie modyfukuję go zgodnie z treścią zadania
        AddNewStudentToDB(student);
        
        return Ok();
    }

    [HttpDelete("{indexNumber}", Name = "DeleteStudent")]
    public async Task<IActionResult> DeleteStudent(string indexNumber)
    {
        if (GetAllStudentsFromDB().Any(s => s.IndexNumber == indexNumber) == false)
        {
            return NotFound($"Nie znaleziono studenta o podanym indeksie: {indexNumber}");
        }
        
        //usunięcie studenta z bazy danych
        await DeleteStudentFromDB(indexNumber);
        
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
    
    private async Task AddNewStudentToDB(Student student)
    {
        var studentString = $"{student.FirstName},{student.LastName},{student.IndexNumber},{student.BirthDate},{student.StudiesName},{student.StudiesMode},{student.Email},{student.FathersName},{student.MothersName}";
        if (GetAllStudents().Count() == 0)
        {
            await System.IO.File.AppendAllTextAsync(_csvFilePath, studentString);
        }
        else
        {
             await System.IO.File.AppendAllTextAsync(_csvFilePath, Environment.NewLine + studentString);
        }
    }
    
    private async Task DeleteStudentFromDB(string indexNumber)
    {
        var lines = System.IO.File.ReadAllLines(_csvFilePath);
        System.IO.File.Delete(_csvFilePath);
        var newLines = lines.Where(line => line.Split(",")[2] != indexNumber);
        using var stream = System.IO.File.OpenWrite(_csvFilePath);
        using var writer = new StreamWriter(stream);
        for(int i = 0; i < (newLines.Count()) - 1; i++)
        {
            await writer.WriteLineAsync(newLines.ElementAt(i));
        }

        if (newLines.Count() > 0)
        {
            await writer.WriteAsync(newLines.ElementAt(newLines.Count() - 1));
        }
    }
}