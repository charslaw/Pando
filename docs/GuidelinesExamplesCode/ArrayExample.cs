using System.Collections.Generic;

namespace GuidelinesExamplesCode;

public record Company(         // Company is a branch
	string CompanyName,        // CompanyName is a blob
	List<string> EmployeeNames // EmployeeNames is a branch; each employee name is a blob
);
