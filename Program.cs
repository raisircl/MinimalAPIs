using MinimalAPIs;
using Microsoft.Data.SqlClient;
using System.Data;  
var builder = WebApplication.CreateBuilder(args);

var conn_str = builder.Configuration.GetConnectionString("Default");

//builder.Services.AddSingleton<Student>();

var app = builder.Build();

app.MapGet("/testdb", () =>
{
    SqlConnection conn=new SqlConnection(conn_str);
    conn.Open();
    if(conn.State==ConnectionState.Open)
    {
        conn.Close();
        return Results.Ok("Database connection successful");
    }
    else
    {
        return Results.Problem("Database connection failed");
    }
});
app.MapGet("/studentsdb", () =>
{
    List<Student> students = new List<Student>();
    
    SqlConnection conn=new SqlConnection(conn_str);
    SqlCommand comm = new SqlCommand();
    comm.CommandText="SELECT Id, Name, Age FROM tblStudents";  
    comm.CommandType=CommandType.Text;  
    comm.Connection=conn;
    conn.Open();
    SqlDataReader reader=comm.ExecuteReader();
    while(reader.Read())
    {
        students.Add(new Student
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Age = reader.GetInt32(2)
        });
    }
    conn.Close();
    return Results.Ok(students);
});
app.MapPost("/studentsdb", (Student student) =>
{
    SqlConnection conn=new SqlConnection(conn_str);
    SqlCommand comm = new SqlCommand();
    comm.CommandText="INSERT INTO tblStudents (Name, Age) VALUES (@Name, @Age)";  
    comm.CommandType=CommandType.Text;  
    comm.Connection=conn;
    comm.Parameters.AddWithValue("@Name", student.Name);
    comm.Parameters.AddWithValue("@Age", student.Age);
    conn.Open();
    int rowsAffected = comm.ExecuteNonQuery();
    conn.Close();
    comm.Parameters.Clear();
    if (rowsAffected > 0)
    {
        return Results.Created($"/studentsdb/{student.Name}", student);
    }
    else
    {
        return Results.Problem("Failed to add student to the database");
    }
});
app.MapPut("/studentsdb/{id}", (int id, Student updatedStudent) =>
{
    SqlConnection conn=new SqlConnection(conn_str);
    SqlCommand comm = new SqlCommand();
    comm.CommandText="UPDATE tblStudents SET Name = @Name, Age = @Age WHERE Id = @Id";  
    comm.CommandType=CommandType.Text;  
    comm.Connection=conn;
    comm.Parameters.AddWithValue("@Name", updatedStudent.Name);
    comm.Parameters.AddWithValue("@Age", updatedStudent.Age);
    comm.Parameters.AddWithValue("@Id", id);
    conn.Open();
    int rowsAffected = comm.ExecuteNonQuery();
    conn.Close();
    comm.Parameters.Clear();
    if (rowsAffected > 0)
    {
        return Results.NoContent();
    }
    else
    {
        return Results.NotFound();
    }
});
app.MapDelete("/studentsdb/{id}", (int id) =>
{
    SqlConnection conn=new SqlConnection(conn_str);
    SqlCommand comm = new SqlCommand();
    comm.CommandText="DELETE FROM tblStudents WHERE Id = @Id";  
    comm.CommandType=CommandType.Text;  
    comm.Connection=conn;
    comm.Parameters.AddWithValue("@Id", id);
    conn.Open();
    int rowsAffected = comm.ExecuteNonQuery();
    conn.Close();
    comm.Parameters.Clear();
    if (rowsAffected > 0)
    {
        return Results.NoContent();
    }
    else
    {
        return Results.NotFound();
    }
});
List<Student> students = new List<Student>
{
    new Student { Id = 1, Name = "Alice", Age = 20 },
    new Student { Id = 2, Name = "Bob", Age = 22 },
    new Student { Id = 3, Name = "Charlie", Age = 23 }
};
app.MapGet("/", () => "Welcome to Minimal APIs");

app.MapGet("/students", () => students);
app.MapPost("/students", (Student student) =>
{
    students.Add(student);
    return Results.Created($"/students/{student.Id}", student);
});
app.MapPut("/students/{id}", (int id, Student updatedStudent) =>
{
    var student = students.FirstOrDefault(s => s.Id == id);
    if (student is null) return Results.NotFound();
    student.Name = updatedStudent.Name;
    student.Age = updatedStudent.Age;
    return Results.NoContent();
});
app.MapDelete("/students/{id}", (int id) =>
{
    var student = students.FirstOrDefault(s => s.Id == id);
    if (student is null) return Results.NotFound();
    students.Remove(student);
    return Results.NoContent();
});
app.MapGet("/students/{id}", (int id) =>
{
    var student = students.FirstOrDefault(s => s.Id == id);
    return student is not null ? Results.Ok(student) : Results.NotFound();
});

app.Run();
