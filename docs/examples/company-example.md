# Company Example

A simple example of a node that contains both primitive data
and another node in the form of a collection of `Person` nodes.

```csharp
public record Company(
    string CompanyName, // CompanyName is a primitive value
    Person[] Employees  // Employees is a node; each employee is a leaf node (see the basic example)
);
```

## Memory Layout

### `Company`

Company is a node that contains both a primitive value (`CompanyName`) and another node (`Employees`).
The primitive value is stored inline alongside the hash which refers to the array of employees.

```
 CompanyName Length  CompanyName   Employees Hash
     ┌──(4 bytes)──┐ ┌(N bytes)─┐ ┌─(8 bytes)──┐
    ┌───┬───┬───┬───┬───┬  ┬─────┬─────┬  ┬─────┐
    │ 0 │ 1 │ 2 │ 3 │ 4 │~~│ 4+N │4+N+1│~~│4+N+8│
    └───┴───┴───┴───┴───┴  ┴─────┴─────┴  ┴─────┘
```

*Where `N` is the number of bytes used to serialize the string (depends on text encoding).*

### `Employees`

`Employees`, being an array of nodes, is a node unto itself,
and thus stores hashes for each of the nodes contained within it.

```
        [0] Hash                [1] Hash             [N] Hash        
 ┌──────(8 bytes)──────┐ ┌──────(8 bytes)──────┐    ┌(8 bytes)─┐
┌──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬  ┬───┬  ┬─────┐
│00│01│02│03│04│05│06│07│08│09│10│11│12│13│14│15│~~│N*8│~~│N*8+7│
└──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴  ┴───┴  ┴─────┘
```

*Where `N` is the number of employees in the array.*
