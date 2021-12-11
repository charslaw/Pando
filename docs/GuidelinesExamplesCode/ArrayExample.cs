namespace GuidelinesExamplesCode;

public record Company(     // Company is a branch
	string CompanyName,    // CompanyName is a blob
	string[] EmployeeNames // EmployeeNames is a branch; each employee name is a blob
);
