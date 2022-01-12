# Array Example

A simple example of an array in a pando state tree.

```csharp
public record Company(      // Company is a branch
    string CompanyName,     // CompanyName is a primitive value
    Person[] Employees      // Employees is a branch; each employee is a blob (see the basic example)
);
```

## Memory Layout

### `Company`

Company is a branch that contains both a primitive value (`CompanyName`) and another branch (`Employees`). The primitive
value is stored inline alongside the hash which refers to the array of employees.

```
 ┌─ CompanyName ─┐ ┌─────────┬─ Employees Hash
┌──┬──┬   ┬───┬───┬───┬   ┬───┐
│00│01│...│N-1│ N │N+1│...│N+8│
└──┴──┴   ┴───┴───┴───┴   ┴───┘
```

*Where `N` is the number of bytes used to serialize the string (depends on text encoding).*

### `Employees`

`Employees`, being an array of blobs, is a branch, and thus stores hashes for each of the blobs contained within it.

```
 ┌──── [0] Hash ───────┐ ┌──── [1] Hash ───────┐     ┌ [N] Hash ─┐
┌──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬   ┬───┬   ┬─────┐
│00│01│02│03│04│05│06│07│08│09│10│11│12│13│14│15│...│N*8│...│N*8+7│
└──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴   ┴───┴   ┴─────┘
```

*Where `N` is the number of employees in the array.*
