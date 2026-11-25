using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Project_10.Models;

namespace Project_10.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly string _connectionString;

        public StudentsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // GET: api/students
        [HttpGet]
        public ActionResult<IEnumerable<Student>> GetAllStudents()
        {
            var students = new List<Student>();
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM Students", con);
            con.Open();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                students.Add(new Student
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    EnrollmentNo = reader["EnrollmentNo"].ToString()!,
                    FullName = reader["FullName"].ToString()!,
                    Branch = reader["Branch"].ToString()!,
                    Semester = Convert.ToInt32(reader["Semester"]),
                    Email = reader["Email"].ToString()!
                });
            }
            return Ok(students);
        }

        // GET: api/students/1
        [HttpGet("{id:int}")]
        public ActionResult<Student> GetStudentById(int id)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM Students WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);
            con.Open();

            using var reader = cmd.ExecuteReader();
            if (!reader.Read())
                return NotFound(new { message = $"Student with ID {id} not found." });

            return Ok(new Student
            {
                Id = Convert.ToInt32(reader["Id"]),
                EnrollmentNo = reader["EnrollmentNo"].ToString()!,
                FullName = reader["FullName"].ToString()!,
                Branch = reader["Branch"].ToString()!,
                Semester = Convert.ToInt32(reader["Semester"]),
                Email = reader["Email"].ToString()!
            });
        }

        // POST: api/students
        [HttpPost]
        public ActionResult<Student> CreateStudent([FromBody] Student student)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var con = new SqlConnection(_connectionString);
            string query = @"INSERT INTO Students (EnrollmentNo, FullName, Branch, Semester, Email)
                             OUTPUT INSERTED.Id
                             VALUES (@EnrollmentNo, @FullName, @Branch, @Semester, @Email)";

            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@EnrollmentNo", student.EnrollmentNo);
            cmd.Parameters.AddWithValue("@FullName", student.FullName);
            cmd.Parameters.AddWithValue("@Branch", student.Branch);
            cmd.Parameters.AddWithValue("@Semester", student.Semester);
            cmd.Parameters.AddWithValue("@Email", student.Email);

            con.Open();
            student.Id = (int)cmd.ExecuteScalar();

            return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, student);
        }

        // PUT: api/students/1
        [HttpPut("{id:int}")]
        public IActionResult UpdateStudent(int id, [FromBody] Student student)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            using var con = new SqlConnection(_connectionString);
            string query = @"UPDATE Students 
                             SET EnrollmentNo = @EnrollmentNo, FullName = @FullName, 
                                 Branch = @Branch, Semester = @Semester, Email = @Email
                             WHERE Id = @Id";

            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Id", id);
            cmd.Parameters.AddWithValue("@EnrollmentNo", student.EnrollmentNo);
            cmd.Parameters.AddWithValue("@FullName", student.FullName);
            cmd.Parameters.AddWithValue("@Branch", student.Branch);
            cmd.Parameters.AddWithValue("@Semester", student.Semester);
            cmd.Parameters.AddWithValue("@Email", student.Email);

            con.Open();
            int rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                return NotFound(new { message = $"Student with ID {id} not found." });

            student.Id = id;
            return Ok(new { message = "Student updated successfully.", student });
        }

        // DELETE: api/students/1
        [HttpDelete("{id:int}")]
        public IActionResult DeleteStudent(int id)
        {
            using var con = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("DELETE FROM Students WHERE Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", id);

            con.Open();
            int rows = cmd.ExecuteNonQuery();
            if (rows == 0)
                return NotFound(new { message = $"Student with ID {id} not found." });

            return Ok(new { message = $"Student with ID {id} deleted successfully." });
        }
    }
}