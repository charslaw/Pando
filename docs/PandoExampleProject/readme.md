# Chess Example Project

This is a relatively simple toy project that illustrates the concepts of data organization and serializers.
This is not a playable chess game with rules,
rather it serves to show how the data of a chess game might be represented as a Pando state tree.

## How to use this example

1. Start with the [state tree definition](ChessStateTree.cs).
   This shows how the data is organized.
3. Inspect the [serializers](Serializers) written for this state tree.
4. View the tests, which shows usage of a Pando repository with the state tree and serializers.
5. The rest of the code is peripheral to Pando itself and is primarily related to making the tests expressive.
