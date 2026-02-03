using Npgsql;
using System.Data;
using EduPortal.Models;

namespace EduPortal.Data
{
    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager()
        {
            // Строка подключения к PostgreSQL
            // Измените параметры подключения под ваши настройки
            _connectionString = "Host=localhost;Port=5432;Database=EduPortal;Username=postgres;Password=artyomka";
        }

        public DatabaseManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Тест подключения к базе данных
        public bool TestConnection()
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Teachers
        public List<Teacher> GetAllTeachers()
        {
            var teachers = new List<Teacher>();
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                
                string query = "SELECT teacher_id, first_name, last_name, subject, email, phone FROM Teachers ORDER BY last_name, first_name";
                using var command = new NpgsqlCommand(query, connection);
                using var reader = command.ExecuteReader();
                
                while (reader.Read())
                {
                    teachers.Add(new Teacher
                    {
                        TeacherId = reader.GetInt32("teacher_id"),
                        FirstName = reader.GetString("first_name"),
                        LastName = reader.GetString("last_name"),
                        Subject = reader.GetString("subject"),
                        Email = reader.IsDBNull("email") ? "" : reader.GetString("email"),
                        Phone = reader.IsDBNull("phone") ? "" : reader.GetString("phone")
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении списка преподавателей: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return teachers;
        }

        public bool AddTeacher(Teacher teacher)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                
                string query = "INSERT INTO Teachers (first_name, last_name, subject, email, phone) VALUES (@firstName, @lastName, @subject, @email, @phone)";
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@firstName", teacher.FirstName);
                command.Parameters.AddWithValue("@lastName", teacher.LastName);
                command.Parameters.AddWithValue("@subject", teacher.Subject);
                command.Parameters.AddWithValue("@email", (object?)teacher.Email ?? DBNull.Value);
                command.Parameters.AddWithValue("@phone", (object?)teacher.Phone ?? DBNull.Value);
                
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении преподавателя: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool UpdateTeacher(Teacher teacher)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                
                string query = "UPDATE Teachers SET first_name = @firstName, last_name = @lastName, subject = @subject, email = @email, phone = @phone WHERE teacher_id = @id";
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@firstName", teacher.FirstName);
                command.Parameters.AddWithValue("@lastName", teacher.LastName);
                command.Parameters.AddWithValue("@subject", teacher.Subject);
                command.Parameters.AddWithValue("@email", (object?)teacher.Email ?? DBNull.Value);
                command.Parameters.AddWithValue("@phone", (object?)teacher.Phone ?? DBNull.Value);
                command.Parameters.AddWithValue("@id", teacher.TeacherId);
                
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении преподавателя: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool DeleteTeacher(int teacherId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                
                string query = "DELETE FROM Teachers WHERE teacher_id = @id";
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", teacherId);
                
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении преподавателя: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        #endregion

        #region Courses
        public DataTable GetCoursesDataTable()
        {
            var dataTable = new DataTable();
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                
                string query = @"
                    SELECT c.course_id, c.course_name, c.duration, c.credits, c.semester, c.description,
                           t.first_name || ' ' || t.last_name as teacher_name
                    FROM Courses c
                    JOIN Teachers t ON c.teacher_id = t.teacher_id
                    ORDER BY c.semester, c.course_name";
                
                using var adapter = new NpgsqlDataAdapter(query, connection);
                adapter.Fill(dataTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении курсов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dataTable;
        }

        public List<Course> GetAllCourses()
        {
            var courses = new List<Course>();
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                
                string query = @"
                    SELECT c.course_id, c.course_name, c.duration, c.teacher_id, c.credits, c.semester, c.description,
                           t.first_name || ' ' || t.last_name as teacher_name
                    FROM Courses c
                    JOIN Teachers t ON c.teacher_id = t.teacher_id
                    ORDER BY c.semester, c.course_name";
                
                using var command = new NpgsqlCommand(query, connection);
                using var reader = command.ExecuteReader();
                
                while (reader.Read())
                {
                    courses.Add(new Course
                    {
                        CourseId = reader.GetInt32("course_id"),
                        CourseName = reader.GetString("course_name"),
                        Duration = reader.GetString("duration"),
                        TeacherId = reader.GetInt32("teacher_id"),
                        Credits = reader.GetInt32("credits"),
                        Semester = reader.GetInt32("semester"),
                        Description = reader.IsDBNull("description") ? "" : reader.GetString("description"),
                        TeacherName = reader.GetString("teacher_name")
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении курсов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return courses;
        }

        public bool AddCourse(Course course)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                
                string query = "INSERT INTO Courses (course_name, duration, teacher_id, credits, semester, description) VALUES (@courseName, @duration, @teacherId, @credits, @semester, @description)";
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@courseName", course.CourseName);
                command.Parameters.AddWithValue("@duration", course.Duration);
                command.Parameters.AddWithValue("@teacherId", course.TeacherId);
                command.Parameters.AddWithValue("@credits", course.Credits);
                command.Parameters.AddWithValue("@semester", course.Semester);
                command.Parameters.AddWithValue("@description", (object?)course.Description ?? DBNull.Value);
                
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении курса: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool UpdateCourse(Course course)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                
                string query = "UPDATE Courses SET course_name = @courseName, duration = @duration, teacher_id = @teacherId, credits = @credits, semester = @semester, description = @description WHERE course_id = @id";
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@courseName", course.CourseName);
                command.Parameters.AddWithValue("@duration", course.Duration);
                command.Parameters.AddWithValue("@teacherId", course.TeacherId);
                command.Parameters.AddWithValue("@credits", course.Credits);
                command.Parameters.AddWithValue("@semester", course.Semester);
                command.Parameters.AddWithValue("@description", (object?)course.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("@id", course.CourseId);
                
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении курса: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool DeleteCourse(int courseId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                
                string query = "DELETE FROM Courses WHERE course_id = @id";
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", courseId);
                
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении курса: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public List<Course> SearchCourses(string searchTerm)
        {
            var courses = new List<Course>();
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                
                string query = @"
                    SELECT c.course_id, c.course_name, c.duration, c.teacher_id, c.credits, c.semester, c.description,
                           t.first_name || ' ' || t.last_name as teacher_name
                    FROM Courses c
                    JOIN Teachers t ON c.teacher_id = t.teacher_id
                    WHERE LOWER(c.course_name) LIKE LOWER(@searchTerm)
                    ORDER BY c.semester, c.course_name";
                
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");
                using var reader = command.ExecuteReader();
                
                while (reader.Read())
                {
                    courses.Add(new Course
                    {
                        CourseId = reader.GetInt32("course_id"),
                        CourseName = reader.GetString("course_name"),
                        Duration = reader.GetString("duration"),
                        TeacherId = reader.GetInt32("teacher_id"),
                        Credits = reader.GetInt32("credits"),
                        Semester = reader.GetInt32("semester"),
                        Description = reader.IsDBNull("description") ? "" : reader.GetString("description"),
                        TeacherName = reader.GetString("teacher_name")
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске курсов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return courses;
        }
        #endregion

        #region Students
        public DataTable GetStudentsDataTable()
        {
            var dataTable = new DataTable();
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                
                string query = @"
                    SELECT s.student_id, s.first_name, s.last_name, s.email, s.group_name, s.enrollment_date
                    FROM Students s
                    ORDER BY s.last_name, s.first_name";
                
                using var adapter = new NpgsqlDataAdapter(query, connection);
                adapter.Fill(dataTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении студентов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dataTable;
        }

        public bool AddStudent(Student student)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                
                string query = "INSERT INTO Students (first_name, last_name, email, group_name, enrollment_date) VALUES (@firstName, @lastName, @email, @groupName, @enrollmentDate)";
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@firstName", student.FirstName);
                command.Parameters.AddWithValue("@lastName", student.LastName);
                command.Parameters.AddWithValue("@email", student.Email);
                command.Parameters.AddWithValue("@groupName", student.GroupName);
                command.Parameters.AddWithValue("@enrollmentDate", student.EnrollmentDate);
                
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении студента: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public List<Student> GetAllStudents()
        {
            var students = new List<Student>();
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                
                string query = "SELECT student_id, first_name, last_name, email, group_name, enrollment_date FROM Students ORDER BY last_name, first_name";
                using var command = new NpgsqlCommand(query, connection);
                using var reader = command.ExecuteReader();
                
                while (reader.Read())
                {
                    students.Add(new Student
                    {
                        StudentId = reader.GetInt32("student_id"),
                        FirstName = reader.GetString("first_name"),
                        LastName = reader.GetString("last_name"),
                        Email = reader.IsDBNull("email") ? "" : reader.GetString("email"),
                        GroupName = reader.IsDBNull("group_name") ? "" : reader.GetString("group_name"),
                        EnrollmentDate = reader.GetDateTime("enrollment_date")
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении списка студентов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return students;
        }

        public bool UpdateStudent(Student student)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                
                string query = "UPDATE Students SET first_name = @firstName, last_name = @lastName, email = @email, group_name = @groupName WHERE student_id = @id";
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@firstName", student.FirstName);
                command.Parameters.AddWithValue("@lastName", student.LastName);
                command.Parameters.AddWithValue("@email", (object?)student.Email ?? DBNull.Value);
                command.Parameters.AddWithValue("@groupName", (object?)student.GroupName ?? DBNull.Value);
                command.Parameters.AddWithValue("@id", student.StudentId);
                
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении студента: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool DeleteStudent(int studentId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                
                string query = "DELETE FROM Students WHERE student_id = @id";
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", studentId);
                
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении студента: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public List<StudentCourse> GetAllEnrollments()
        {
            var enrollments = new List<StudentCourse>();
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                
                string query = @"
                    SELECT sc.id as StudentCourseId, sc.student_id, sc.course_id, sc.enrollment_date, sc.grade,
                           s.first_name || ' ' || s.last_name as student_name,
                           c.course_name
                    FROM Student_Courses sc
                    JOIN Students s ON sc.student_id = s.student_id
                    JOIN Courses c ON sc.course_id = c.course_id
                    ORDER BY sc.enrollment_date DESC";
                
                using var command = new NpgsqlCommand(query, connection);
                using var reader = command.ExecuteReader();
                
                while (reader.Read())
                {
                    enrollments.Add(new StudentCourse
                    {
                        StudentCourseId = reader.GetInt32("StudentCourseId"),
                        StudentId = reader.GetInt32("student_id"),
                        CourseId = reader.GetInt32("course_id"),
                        EnrollmentDate = reader.GetDateTime("enrollment_date"),
                        Grade = reader.IsDBNull("grade") ? null : reader.GetDecimal("grade"),
                        StudentName = reader.GetString("student_name"),
                        CourseName = reader.GetString("course_name")
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении записей: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return enrollments;
        }

        public bool EnrollStudent(StudentCourse enrollment)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                
                string query = "INSERT INTO Student_Courses (student_id, course_id, enrollment_date) VALUES (@studentId, @courseId, @enrollmentDate)";
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@studentId", enrollment.StudentId);
                command.Parameters.AddWithValue("@courseId", enrollment.CourseId);
                command.Parameters.AddWithValue("@enrollmentDate", enrollment.EnrollmentDate);
                
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при записи студента: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool UpdateGrade(int enrollmentId, decimal grade)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                
                string query = "UPDATE Student_Courses SET grade = @grade WHERE id = @id";
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@grade", grade);
                command.Parameters.AddWithValue("@id", enrollmentId);
                
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении оценки: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool UnenrollStudent(int enrollmentId)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                
                string query = "DELETE FROM Student_Courses WHERE id = @id";
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", enrollmentId);
                
                return command.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отчислении студента: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public List<StudentCourse> GetStudentsOnCourse(int courseId)
        {
            var studentCourses = new List<StudentCourse>();
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                
                string query = @"
                    SELECT sc.id, sc.student_id, sc.course_id, sc.enrollment_date, sc.grade, sc.grade_type,
                           s.first_name || ' ' || s.last_name as student_name,
                           c.course_name
                    FROM Student_Courses sc
                    JOIN Students s ON sc.student_id = s.student_id
                    JOIN Courses c ON sc.course_id = c.course_id
                    WHERE sc.course_id = @courseId
                    ORDER BY s.last_name, s.first_name";
                
                using var command = new NpgsqlCommand(query, connection);
                command.Parameters.AddWithValue("@courseId", courseId);
                using var reader = command.ExecuteReader();
                
                while (reader.Read())
                {
                    studentCourses.Add(new StudentCourse
                    {
                        Id = reader.GetInt32("id"),
                        StudentId = reader.GetInt32("student_id"),
                        CourseId = reader.GetInt32("course_id"),
                        EnrollmentDate = reader.GetDateTime("enrollment_date"),
                        Grade = reader.IsDBNull("grade") ? null : reader.GetDecimal("grade"),
                        GradeType = reader.IsDBNull("grade_type") ? "" : reader.GetString("grade_type"),
                        StudentName = reader.GetString("student_name"),
                        CourseName = reader.GetString("course_name")
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении студентов курса: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return studentCourses;
        }
        #endregion
    }
}