# DocCore

This dotnet tool is a simple cross-platform dotnet class api documentation generator.

It will generate a .md file for each .cs file in a solution containing documentation for all public and protected constructors, methods and properties.

The documentation can be written either as simple comments or in yaml syntax.

By using the yaml syntax you can provide extra information for parameters and return types.

Some examples of doc comments:

**yaml syntax**

```
// Summary: Get all items from database filtered by searchParams
// Parameters:
//   searchParams: A dictionary mapping property name to expected value as a string
// Returns: All items of type T from the database
protected abstract Task<IEnumerable<T>> ReadItems(Dictionary<string, string> searchParams);
```

**simple string comment**

```
// Abstract storage class.
// Use this class to implement different types of storages for your resources.
public abstract class Storage<T> where T : AbstractResource
```

## Why?

Because I don't like xml comments.

## Installation

```
dotnet tool install -g DocCore
```

## How to use

Navigate to the folder containing your solution and run `doccore`.

Run 'doccore --help' for more options.
