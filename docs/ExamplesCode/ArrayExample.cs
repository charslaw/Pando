namespace ExamplesCode;

internal record Company(
	string CompanyName, // CompanyName is a primitive value
	Person[] Employees  // Employees is a node; each employee is a leaf node (see the basic example)
);
