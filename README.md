# UL Solutions Coding Challenge
![workflow](https://github.com/gabriel301/ULSolutionsCodingChallenge/actions/workflows/dotnet.yml/badge.svg?brach=main)

### Build With
.Net Core 8

## Outline
<p>Rather than asking lots of coding questions in an interview, we’ve produced this short exercise that is designed to allow you to demonstrate your development and problem-solving skills in a non-stressful environment. </p>
  
<p>When working on the exercise, feel free to research using the resources you would have available in the workplace (Internet, books, etc.), as we’re trying to ascertain how you’d tackle a problem in a day-today work environment. However, please submit your own code as code copied verbatim off the Internet will be rejected. You will be asked to demonstrate a thorough understanding of your code at the interview stage.</p>

<p>The exercise itself is not one we’d expect any potential candidate to have a problem solving, so consider it a chance to show off your range of skills. We’re just as interested in how something is done as we are in seeing a functional solution. This isn’t an exercise to provide the shortest possible solution, providing a purely functional solution in one or two of lines of code won’t give us what we need to properly assess your skillset.</p> 

<p>We're looking for people who can demonstrate they write well thought out, maintainable, production quality code. There are no time constraints, but we are looking forward to seeing your solution as soon as possible. </p>

### The Exercise
<p>Write a C# web-based API application that evaluates a string expression consisting of non-negative integers and the + - / * operators only, considering the normal mathematical rules of operator precedence. Support for brackets is not required. The calculation should be performed in the API not the UI and by your own code, not a thirdparty library. </p>

<p>For example:
<br>an input string of "4+5*2" should output 14
<br>an input string of "4+5/2" should output 6.5
<br>an input string of "4+5/2-1" should output 5.5
</p>

## Solution
### Solution Architecture
Based on the Clean Architecture, the solution contains 4 projects:

1. **UL Domain** - Contains the entities (Expression Evaluators), encapsulating the business logic, following the Domain Driven Design approach.
 2. **UL Application** -  Contains the application rules (use cases) of the system. In this project, it contains expression validators and a reduced implementation of the CQRS pattern (only 2 commands) using MediatR. It also implements event notification when an expression is created or evaluated. For this projects, the events are just logs with event information.
 3.  **UL WebApi** -  Contains one controlller, with 2 API versions.
 4. **UL Shared** - Resources that are shared amongst the other 3 projects, such as string messages (resource files), abstract classes or static classes.

### Testing Projects
Unit testing was implemented using xUnit and NBomber (for WebApi):
 1. **UL Domain Tests** - Contains unit testing for business logic.
 2. **UL Application Tests** - Contains use case testing, such as validations and expression evaluation results.
 3. **UL WebApi Tests** - Functional tests, such as API response codes, expression evaluation results and rate limit tests.

### Github Actions
An action is configured to trigger on push or pull request in the main branch.

#### Domain
Domain implements 2 different algorithms for evaluating expressions:

 1. **Binary Expression Tree** - Using regular expressions, the string is parsed from right to left, looking for operators with less precedence. Thus, the last evaluated operator will be placed on the root of the tree. The algorithm process all the left subtrees first, the the right subtrees. Operators are root of subtrees and operands are leafs. For evaluating, a Depth First Search (DFS) algorithm was implemented. Two major drawbacks of this implementation are excessive memory usage for large trees, and long processing time due several object allocation (tree nodes) and regular expression evaluations.
 2. **Evaluation with stacks**: This algorithms uses a regular expressions to split the string in a list of strings, by arithmetic operators. Iterate through the list, processing multiplicatication and divisions first and then sums and subtrations, evaluating the expression string from left to right. Two stacks are used for storing operators and operands for later processing. This implementation solves those 2 drawbacks from Binary Expression Tree implementation.
