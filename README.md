# Disposable

 \
Primarily implemented to release unmanaged resources


# What is IDisposable

The IDisposable interface is a part of the .NET Framework and is used in C# and other .NET languages. 

It provides a mechanism for releasing unmanaged resources such as file handles, database connections, and network connections. 

The primary purpose of the IDisposable interface is to allow objects to release these resources explicitly when they are no longer needed, 

rather than relying on the automatic garbage collection process.

# When & Where to use IDisposable | .NET Core , EF Core

## Dealing with Files: 

If your class is reading from or writing to a file, it should implement IDisposable. This is because file handles are a finite system resource. If they're not released, the system may run out of file handles.


```
using (FileStream fileStream = File.Open("myfile.txt", FileMode.Open))
{
    // Use the file stream here
    // ...
} // Dispose() method is automatically called here, releasing the file handle
```



## Dealing with Database Connections: 

Connections to databases are also limited resources. If these are not released, it may not be possible to open new connections.


```
using (DbConnection connection = new DbConnection(connectionString))
{
    connection.Open();
    // Use the connection for database operations
} // Dispose() method is automatically called here, closing the connection and releasing resources
```



## Dealing with Network Connections: 

Similar to database connections, network connections should also be released when they are no longer needed.


```
using (TcpClient tcpClient = new TcpClient())
{
    tcpClient.Connect("example.com", 8080);
    // Use the TCP client for network operations
    // ...
} // Dispose() method is automatically called here, closing the network connection and releasing resources..
```



# Ways of implementing Disposable



# 1. Implementing IDisposable Interface:

```
public class MyClass : IDisposable
{
    private bool disposed = false;
    // Dispose method to release resources
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Release managed resources here
            }
            // Release unmanaged resources here
            disposed = true;
        }
    }
    // Public Dispose method
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }}
```

# 2. Using Statement:

The using statement ensures that the Dispose method is called automatically when the using block is exited, even if an exception occurs.  
\
_<span style="text-decoration:underline;">Assuming that MyClass implemented IDisposable</span>_


```
using (MyClass myObject = new MyClass())
{
    // Use myObject within the using block
}
```



# 3. Finalizer (destructor):


 The finalizer is a backup mechanism that ensures resources are released even if Dispose is not called explicitly. However, it is recommended to call Dispose explicitly to release resources in a timely manner.


```
public class MyClass : IDisposable
{
    private bool disposed = false;

    ~MyClass()
    {
        Dispose(false);
    }

    // Dispose method to release resources
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Release managed resources here
            }

            // Release unmanaged resources here

            disposed = true;
        }
    }

    // Public Dispose method
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
```



# WHATs? WHYs? HOWs?



# 1. **What is a Finalizer**

A finalizer is a special method or function that is executed when an object is about to be destroyed or garbage collected by the system.

The purpose of a finalizer is to provide a mechanism for releasing resources that are not automatically managed by the language's memory management system. \
 \
**Is it recommended?** \
 \
finalizers are not always recommended. This is because finalizers can introduce performance overhead and are not guaranteed to be executed promptly or at all 



# 2. **Why we use** `GC.SuppressFinalize(this);`

This line is called after the `Dispose(true)` method is invoked. It instructs the garbage collector (GC) to suppress the finalization of the object. 

In other words, it tells the GC that the object's finalizer does not need to be called because all necessary cleanup has already been performed explicitly in the Dispose method.

By calling `GC.SuppressFinalize(this)`, you're informing the GC that the object has been properly disposed of and its finalizer is no longer required. This allows the GC to skip the finalization step and immediately reclaim the object's memory, leading to more efficient memory management.

**Is this necessary?**

A finalizer isn't necessary if your object implements IDisposable, but if you have a finalizer you should implement IDisposable to allow deterministic cleanup of your class.

For objects that don't hold unmanaged resources, implementing a finalizer and using GC.SuppressFinalize(this) may not be necessary.



# 3. **What is Dispose() and Dispose(bool) ?**

**Dispose() method:** **non-virtual (NotOverridable)**

This method is the public interface of the IDisposable pattern. It allows clients of your class to explicitly release any resources it holds. The Dispose() method is typically called when the client is finished using the object and wants to release the resources immediately.

**Dispose(bool) method: protected virtual void**

This method is a protected virtual method used for actual cleanup operations. 

**It is called by both the Dispose() method (when disposing is true) and the finalizer (when disposing is false). \**

 ‘protected’ limit its visibility. By doing so, it ensures that only derived classes and classes within the same inheritance hierarchy can directly call this method. It prevents external code from accidentally invoking the Dispose(bool disposing) method directly, which could lead to unexpected behavior.

 ‘virtual’, it allows derived classes to override the implementation if necessary. This is useful when a derived class needs to add additional cleanup logic specific to its own resources. By providing a virtual method, you enable polymorphism and customization of the cleanup behavior in derived classes.




# 4. **How to use lambda operations for Dispose**

**Using lambda**


```
public class Foo : IDisposable
{
    private Bar _bar = new Bar();

    public void Dispose() => _bar.Dispose();
}
```


**Without using lambda**


```
public class Foo : IDisposable
{
    private Bar _bar = new Bar();

    public void Dispose()
    {
        _bar.Dispose();
    }
}
```



# **Lambda operation with finalizer (Dispose(bool))**


```
public void Dispose()
    {   Dispose(true);
        GC.SuppressFinalize(this);
    }
```



```
public void Dispose() => Dispose(true);

~ClassName() => Dispose(false);

private void Dispose(bool disposing)
{
    if (disposing)
    {
        // Dispose managed resources here
    }
    // Dispose unmanaged resources here
    GC.SuppressFinalize(this);
}
```



# Best practices



### 1. **Implement the IDisposable Interface** 
 
Classes that need to manage unmanaged resources or implement cleanup logic should implement the IDisposable interface. This ensures that resources are properly released when they are no longer needed.

### 2. **Follow the Dispose Pattern** 

The Dispose pattern consists of implementing a Dispose method to release resources and a finalizer (destructor) as a backup mechanism. The Dispose method should be responsible for releasing both managed and unmanaged resources, while the finalizer is used to release unmanaged resources if Dispose is not explicitly called.

### 3. **Use a Virtual Dispose Method (optional)** 

If you anticipate derived classes that may need to provide their specific cleanup logic, consider using a virtual Dispose method in the base class. This allows derived classes to override the method and customize the cleanup behavior as required 
Ex: 


**Base Class Implementation**: 



```
public class MyBaseClass : IDisposable
{
    private bool disposed = false;

    // Dispose method to release resources
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Release managed resources
            }

            // Release unmanaged resources

            disposed = true;
        }
    }

    // Public Dispose method
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Finalizer (destructor)
    ~MyBaseClass()
    {
        Dispose(false);
    }
}
```



**Derived Class Implementation**:



```
public class MyDerivedClass : MyBaseClass
{
    private bool disposed = false;
    private SomeOtherManagedResource derivedManagedResource; // Additional managed resource in derived class

    protected override void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Release managed resources specific to MyDerivedClass
                derivedManagedResource.Dispose();
            }

            // Release unmanaged resources specific to MyDerivedClass

            disposed = true;
        }

        base.Dispose(disposing);
    }
}
```




### 4. **Call Dispose Explicitly or Use the Using Statement**:

Call the Dispose method explicitly when they are done using the object. Alternatively, the using statement can be used, ensuring that Dispose is called automatically when the object is no longer needed.

### 5. **Dispose Managed Resources First**: 
 
When implementing the Dispose method, release managed resources before releasing unmanaged resources. This ensures that any dependencies or references to managed objects are cleaned up before attempting to release unmanaged resources. 

### 6. **Suppress Finalization if Disposed** 
 
In the Dispose method, call GC.SuppressFinalize(this) to indicate that finalization (the finalizer) is unnecessary if Dispose has been called explicitly. This improves performance by avoiding unnecessary finalization when the object is already disposed.


## Additional Notes


**Close vs Dispose**


**Using close with a database connection:** 


```
using System.Data.SqlClient;

string connectionString = "YourConnectionString";
using (SqlConnection connection = new SqlConnection(connectionString))
{
    connection.Open();

    // Perform database operations

    connection.Close(); // Close the connection and release it back to the connection pool
}
```

**Using dispose with a database context (Entity Framework):** 

```
using (var dbContext = new YourDbContext())
{
    // Perform database operations using the dbContext

    // No need to explicitly call Close() for the connection,
    // as Dispose() takes care of releasing the resources.

    // Dispose() is automatically called at the end of the using block.
}
```


**using BeginTransaction() and Dispose() in a transaction:** 

```
using (var dbContext = new YourDbContext())
{
    using (var sqlTransaction = dbContext.Database.BeginTransaction())
    {
        try
        {
            // Perform database operations within the transaction

            dbContext.SaveChanges(); // Commit changes to the database

            sqlTransaction.Commit(); // Commit the transaction
        }
        catch (Exception ex)
        {
            sqlTransaction.Rollback(); // Rollback the transaction in case of an exception
            // Handle or rethrow the exception
        }
    }
}
```