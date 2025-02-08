using Microsoft.Data.SqlClient;
using Slutprojekt_Serhan_Gyuler.Models;
using System.Data;
using System.Diagnostics;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Slutprojekt_Serhan_Gyuler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // MENU
            var loop = true;

            while (loop)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("Choose one of the options below by entering a number:");
                Console.WriteLine("========================================");
                Console.WriteLine("1. View which teacher is teaching what subject");
                Console.WriteLine("2. View all students information");
                Console.WriteLine("3. Show all active courses");
                Console.WriteLine("4. View information on employees");
                Console.WriteLine("5. Add a new employee");
                Console.WriteLine("6. View what grades has been set for a specific student");
                Console.WriteLine("7. View salary overview");
                Console.WriteLine("8. View average salary");
                Console.WriteLine("9. View important information on student");
                Console.WriteLine("10. Set grade to student");
                Console.WriteLine("11. Exit"); 
                Console.WriteLine("========================================");
                Console.Write("Enter your choice: ");

                var userChoice = Console.ReadLine();

                // CASES
                switch (userChoice)
                {
                    case "1":
                        TeachersInDifferentDepartments();
                        break;
                    case "2":
                        InformationOnStudents();
                        break;
                    case "3":
                        ActiveCourses();
                        break;
                    case "4":
                        EmployeeOverview();
                        break;
                    case "5":
                        AddNewEmployee();
                        break;
                    case "6":
                        GetGradesForStudents();
                        break;
                    case "7":
                        SalaryOverview();
                        break;
                    case "8":
                        AverageSalary();
                        break;
                    case "9":
                        StoreProcedure();
                        break;
                    case "10":
                        AddGradeToStudent();
                        break;
                    case "11":
                        loop = false;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please enter a valid option.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // Case 1
        public static void TeachersInDifferentDepartments()
        {
            using (var dBManager = new SchoolDbContext())
            {
                var subjectsWithTeachers = dBManager.Subjects
                    .Where(s => s.Employee.Profession == "Teacher")
                    .Select(s => new
                    {
                        s.SubjectName,
                        TeacherName = s.Employee.FirstName + " " + s.Employee.LastName
                    })
                    .ToList();

                foreach (var item in subjectsWithTeachers)
                {
                    Console.WriteLine($"Subject: {item.SubjectName}, Teacher: {item.TeacherName}");
                }
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            Console.Clear();
        }

        // Case 2
        public static void InformationOnStudents()
        {
            using (var dBManager = new SchoolDbContext())
            {
                var studentsInformations = dBManager.Students
                    .Select(s => new
                    {
                        s.FirstName,
                        s.LastName,
                        ClassName = s.Class.ClassName
                    })
                    .ToList();

                foreach (var student in studentsInformations)
                {
                    Console.WriteLine($"{student.FirstName} {student.LastName} ~ Class: {student.ClassName}");
                }
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            Console.Clear();
        }

        // Case 3
        public static void ActiveCourses()
        {
            using (var dBManager = new SchoolDbContext())
            {
                var subjectsActive = dBManager.Subjects
                    .Where(s => (bool)s.IsActive)
                    .ToList();
                Console.WriteLine("Active Subjects:");
                foreach (var subject in subjectsActive)
                {
                    Console.WriteLine($"{subject.SubjectName}");
                }
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            Console.Clear();
        }

        // Connection String
        private static string connectionString = @"Data Source = localhost;Database = SchoolDBLastProject;Integrated Security = True;Trust Server Certificate = True;";

        // Case 4
        public static void EmployeeOverview()
        {
            string query = "SELECT FirstName, LastName, Profession, YearsWorked FROM Employee";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string firstName = reader["FirstName"].ToString();
                    string lastName = reader["LastName"].ToString();
                    string profession = reader["Profession"].ToString();
                    int yearsWorked = Convert.ToInt32(reader["YearsWorked"]);

                    Console.WriteLine($"{firstName} {lastName} - {profession} - {yearsWorked} years in work");
                }

                reader.Close();
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        // Case 5
        public static void AddNewEmployee()
        {
            try 
            {
                Console.WriteLine("Enter First Name: ");
                string firstName = Console.ReadLine();

                Console.Write("Enter Last Name: ");
                string lastName = Console.ReadLine();

                Console.Write("Enter Profession: ");
                string profession = Console.ReadLine();

                Console.Write("Enter Years Worked: ");
                int yearsWorked = Convert.ToInt32(Console.ReadLine());

                Console.Write("Enter Salary: ");
                decimal salary = Convert.ToDecimal(Console.ReadLine());

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Employee (FirstName, LastName, Profession, YearsWorked, Salary)
                                     VALUES (@FirstName, @LastName, @Profession, @YearsWorked, @Salary)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", firstName);
                        command.Parameters.AddWithValue("@LastName", lastName);
                        command.Parameters.AddWithValue("@Profession", profession);
                        command.Parameters.AddWithValue("@YearsWorked", yearsWorked);
                        command.Parameters.AddWithValue("@Salary", salary);

                        connection.Open();
                        var rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine(rowsAffected);
                    }
                    Console.WriteLine("\nPress any key to return to the menu...");
                    Console.ReadKey();
                    Console.Clear();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        // Case 6
        public static void GetGradesForStudents()
        {
            try
            {
                Console.WriteLine("Enter student ID to see set grades: ");
                int studentId = Convert.ToInt32(Console.ReadLine());

                string query = @"SELECT 
                        s.FirstName AS StudentFirstName, 
                        s.LastName AS StudentLastName, 
                        sub.SubjectName, 
                        e.FirstName AS TeacherFirstName, 
                        e.LastName AS TeacherLastName, 
                        g.Grades, 
                        g.DateAssigned
                        FROM Grade g
                        JOIN Student s ON g.StudentId = s.StudentId
                        JOIN Subject sub ON g.SubjectId = sub.SubjectId
                        JOIN Employee e ON g.EmployeeId = e.EmployeeId
                        WHERE s.StudentId = @StudentId";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@StudentId", studentId);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string studentName = reader["StudentFirstName"] + " " + reader["StudentLastName"].ToString();
                        string subject = reader["SubjectName"].ToString();
                        string teacherName = reader["TeacherFirstName"] + reader["TeacherLastName"].ToString();
                        string grade = reader["Grades"].ToString();
                        DateTime dateAssigned = (DateTime)reader["DateAssigned"];

                        Console.WriteLine($"Student: {studentName}, Subject: {subject}, Teacher: {teacherName}, Grade: {grade}, Date: {dateAssigned}");
                    }

                    reader.Close();
                    Console.WriteLine("\nPress any key to return to the menu...");
                    Console.ReadKey();
                    Console.Clear();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                Console.Clear();
            }

        }

        // Case 7
        public static void SalaryOverview()
        {
            string query = "SELECT Profession, SUM(Salary) AS TotalSalary FROM Employee GROUP BY Profession";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string profession = reader["profession"].ToString();
                    decimal totalSalary = Convert.ToDecimal(reader["TotalSalary"]);

                    Console.WriteLine($"{profession}: {totalSalary}kr");
                }

                reader.Close();
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        // Case 8
        public static void AverageSalary()
        {
            string query = "SELECT Profession, SUM(Salary) AS TotalSalary, COUNT(*) AS EmployeeCount FROM Employee GROUP BY Profession";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    string profession = reader["Profession"].ToString();
                    int employeeCount = Convert.ToInt32(reader["EmployeeCount"]);
                    int totalSalary = Convert.ToInt32(reader["TotalSalary"]);
                    int averageSalary = totalSalary / employeeCount;


                    Console.WriteLine($"Average Salary for {profession}: {averageSalary}");
                }

                reader.Close();
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                Console.Clear();
            }
        }

        // Case 9
        public static void StoreProcedure()
        {
            try
            {
                Console.WriteLine("Enter student ID to view important information");
                int studentId = Convert.ToInt32(Console.ReadLine());
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("SelectStudentById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@StudentId", studentId);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string firstName = reader["FirstName"].ToString();
                                string lastName = reader["LastName"].ToString();
                                string ssn = reader["SocialSecurityNumber"].ToString();

                                Console.WriteLine($"Student: {firstName} {lastName} ~ SSN:{ssn} ");
                            }

                            reader.Close();
                            Console.WriteLine("\nPress any key to return to the menu...");
                            Console.ReadKey();
                            Console.Clear();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                Console.Clear();
            }

        }

        // Case 10
        public static void AddGradeToStudent()
        {
            using (SqlConnection connection = new(connectionString))
            {
                connection.Open();

                // Start a local transaction.
                SqlTransaction sqlTran = connection.BeginTransaction();

                try
                {
                    SqlCommand command = connection.CreateCommand();
                    command.Transaction = sqlTran;

                    // Subject
                    Console.WriteLine("Subjects:");
                    command.CommandText = "SELECT SubjectName, SubjectId FROM Subject";
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["SubjectId"]}: {reader["SubjectName"]}");
                        }
                    }
                    Console.Write("Enter the Subject ID: ");
                    int subjectId = Convert.ToInt32(Console.ReadLine());


                    // Student
                    Console.WriteLine("Students:");
                    command.CommandText = "SELECT FirstName, LastName, StudentId FROM Student";
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["StudentId"]}: {reader["FirstName"]} {reader["LastName"]}");
                        }
                    }
                    Console.Write("Enter the student's ID: ");
                    int studentId = Convert.ToInt32(Console.ReadLine());

                    // Grade
                    Console.WriteLine("Choose the grade:\nA\nB\nC\nD\nE\nF");
                    Console.Write("Enter the grade: ");
                    string grade = Console.ReadLine().ToUpper();


                    // Date
                    Console.Write("Enter date (yyyy-mm-dd): ");
                    DateTime dateAssigned = DateTime.Parse(Console.ReadLine());

                    // Employees
                    Console.WriteLine("Available Teachers:");
                    command.CommandText = "SELECT EmployeeId, FirstName, LastName FROM Employee WHERE Profession = 'Teacher'";
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["EmployeeId"]}: {reader["FirstName"]} {reader["LastName"]}");
                        }
                    }
                    Console.Write("Enter the teacher's ID: ");
                    int teacherId = Convert.ToInt32(Console.ReadLine());

                    command.CommandText = @"INSERT INTO Grade (StudentId, SubjectId, EmployeeId, Grades, DateAssigned) 
                                            VALUES (@StudentId, @SubjectId, @TeacherId, @Grades, @DateAssigned)";
                    command.Parameters.AddWithValue("@StudentId", studentId);
                    command.Parameters.AddWithValue("@SubjectId", subjectId);
                    command.Parameters.AddWithValue("@TeacherId", teacherId);
                    command.Parameters.AddWithValue("@Grades", grade);
                    command.Parameters.AddWithValue("@DateAssigned", dateAssigned);

                    int rowsAffected = command.ExecuteNonQuery();

                    // If successful finilaze transaction
                    sqlTran.Commit();
                    Console.WriteLine("Grade successfully added!");

                }
                catch (Exception ex)
                {
                    // Rollback if error
                    sqlTran.Rollback();
                    Console.WriteLine($"Error: {ex.Message}");
                }

                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}
