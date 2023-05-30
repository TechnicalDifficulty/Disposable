
using Microsoft.EntityFrameworkCore;

public class MyDbContext : DbContext,IDisposable
{
    private bool disposed = false;

    public DbSet<Student> Students { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=tcp:azure-disposable-server.database.windows.net,1433;Initial Catalog=app-disposable-db;Persist Security Info=False;User ID=azuresql;Password=yasa@azure*12;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
    }

    public void DoSomethingWithEntities()
    {
        if (disposed)
            throw new ObjectDisposedException("MyDbContext");

        // Use the DbContext and its entities
        foreach (var entity in Students)
        {
            Console.WriteLine(entity.Name);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Release any managed resources here
                Console.WriteLine("Disposing of managed resources.");

                // Dispose the DbContext
                base.Dispose();
            }

            disposed = true;
        }
    }
}

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}

public class Program
{
    public static void Main()
    {

        // #region Program1
        // Typical way of executing disposable
        //Program1();
        // #endregion

        // #region Program2
        // Using "using" statement to automatically execute disposable
        //Program2();
        // #endregion

        // #region Program3
        // Using finalizer
        Program3();
        // #endregion

        Console.WriteLine("Program completed.");

    }

    #region Program1
    private static void Program1()
    {
        var dbContext = new MyDbContext();
        try
        {
            dbContext.Database.EnsureCreated();
            var students = new List<Student>
        {
            new Student { Name = "Pa", Age = 23 },
            new Student { Name = "Ta", Age = 25 }
        };

            dbContext.Students.AddRange(students);

            dbContext.SaveChanges();

            dbContext.DoSomethingWithEntities();
        }
        finally
        {
            if (dbContext != null)
            {
                dbContext.Dispose();
            }
        }

    }
    #endregion

    #region Program2
    private static void Program2()
    {
        using (var dbContext = new MyDbContext())
        {
            dbContext.Database.EnsureCreated();

            var students = dbContext.Students.Where(s => s.Age >= 20).ToList();

            foreach (var student in students)
            {
                Console.WriteLine(student.Name);
            }
        }
    }
    #endregion

    #region Program3
    private static void Program3()
    {
        MyClass myObject = new MyClass();
        myObject.DoSomething();

        // Uncomment the line below to test finalizer
        myObject = null;
        GC.WaitForPendingFinalizers();
        Thread.Sleep(10000);
    }

    public class MyClass:IDisposable
    {
        public MyClass()
        {
            Console.WriteLine("MyClass object created.");
        }

        public void DoSomething()
        {
            Console.WriteLine("Doing something...");
        }

        ~MyClass()
        {
            // Finalizer code
            Console.WriteLine("MyClass object finalized.");
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Release any managed resources here
                Console.WriteLine("Disposing of managed resources.");
            }

            // Release any unmanaged resources here
            Console.WriteLine("Disposing of unmanaged resources.");
        }
    }
    #endregion
}
