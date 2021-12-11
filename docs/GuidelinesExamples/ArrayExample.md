# Array Example

A simple example of an array in a pando state tree.

```csharp
public record Company(         // Company is a branch
    string CompanyName,        // CompanyName is a blob
    List<string> EmployeeNames // EmployeeNames is a branch; each employee name is a blob
);
```

## Memory Layout

### `Company`

```
 ┌─ CompanyName Hash ──┐ ┌ EmployeeNames Hash ─┐
┌──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┐
│00│01│02│03│04│05│06│07│08│09│10│11│12│13│14│15│
└──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┘
```

### `CompanyName` and `EmployeeNames[i]`

Strings can be complicated since in a UTF8 string a character can be 2-4 bytes. For this memory layout example we'll
assume all of the characters are 2 bytes, however in a real-world scenario any UTF8 string should work.

You could also write a custom serializer for a ASCII only string, etc.

```
 ┌[0]┐ ┌[1]┐ ┌[2]┐    ┌────┬─ [N]
┌──┬──┬──┬──┬──┬──┬──┬───┬──┐
│00│01│02│03│04│05│..│M-1│ M│
└──┴──┴──┴──┴──┴──┴──┴───┴──┘
```

### `EmployeeNames`

```
 ┌──── [0] Hash ───────┐ ┌──── [2] Hash ───────┐    ┌───────┬─ [N] Hash
┌──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬──┬───┬─────┐
│00│01│02│03│04│05│06│07│08│09│10│11│12│13│14│15│..│N*8│N*8+7│
└──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴──┴───┴─────┘
```
