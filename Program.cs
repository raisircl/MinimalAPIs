using MinimalAPIs;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddSingleton<Student>();

var app = builder.Build();
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
