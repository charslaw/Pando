using System;

namespace GuidelinesExamplesCode;

public record Person(
	string Name,
	string Email,
	DateTime DateOfBirth,
	Gender Gender,
	EyeColor EyeColor,
	short NumberOfFingers
);

public enum Gender { Male, Female, Nonbinary, Unspecified }

public enum EyeColor { Blue, Green, Brown, Hazel, Black, Other }

public record PersonFixed(
	string Name,
	string Email,
	PersonDetails Details
);

public record PersonDetails(
	DateTime DateOfBirth,
	Gender Gender,
	EyeColor EyeColor,
	short NumberOfFingers
);
