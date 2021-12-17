using System;

namespace GuidelinesExamplesCode;

internal record Person(
	string Name,
	string Email,
	DateTime DateOfBirth,
	Gender Gender,
	EyeColor EyeColor,
	short NumberOfFingers
);

internal enum Gender { Male, Female, Nonbinary, Unspecified }

internal enum EyeColor { Blue, Green, Brown, Hazel, Black, Other }

internal record PersonFixed(
	string Name,
	string Email,
	PersonDetails Details
);

internal record PersonDetails(
	DateTime DateOfBirth,
	Gender Gender,
	EyeColor EyeColor,
	short NumberOfFingers
);
