# Math Game

The *MathGame* app is my try to implement a real-time browser-based math game using ASP.NET Core 2.2 Web Application project with Angular template, as well as SignalR library for WebSocket support.

## Getting Started

To try it out, follow the steps:

Clone the project.

The Client-Side and Server-Side are using different ports, that's why we need to start both before we can play the game.
Open _Visual Studio Developer Command Prompt_ and navigate the Angular project (ClientApp folder)
```
cd [PATH_TO_THE_PROJECT]\MathGame\ClientApp
```
and type for the very first time only
```
npm install
```
This will install the dependencies in the local node_modules folder.

Then type and run this command
```
ng serve
```
This will start the Client-Side **NG Live Development Server** and start listening on _http://localhost:4200_.

Next, open a second _Visual Studio Developer Command Prompt_ and navigate the (root) project
```
cd [PATH_TO_THE_PROJECT]\MathGame
```
Then type and run

```
dotnet run --environment=Development
```
This will start the Server-Side application and start listening on _http://localhost:5000_.

Now, in your browser, navigate to
```
http://localhost:5000
```
this will start the game. 

To test with more players, copy the URL and open it in multiple tabs/windows. 

## Math Game specification

Implement a realtime browser-based math game for up to 10 concurrent users. The game is structured as a continuous series of rounds, where all connected players compete to submit the correct answer first. The number of rounds is not limited, players can connect at any time and start competing. 
 
At the beginning of each round a simple math challenge is sent to all connected players, consisting of a basic operation (+ - * /), two operands in range 1..10 and a potential answer. All players are presented with the challenge and have to answer whether the proposed answer is correct using a simple yes/no choice.
 
The first player to submit a correct answer gets 1 point for the round and completes the round. All incorrect answers subtract a point from the players' score. Correct late answers do not affect the score. After completing the round all remaining players are informed that the round is over. A new round starts in 5 seconds after the end of last one.

## Bugs to fix and improvements to do 
Bugs:
* [FIXED] If you don't enter your name, you would still be shown the question and you can start playing without a name.
* [FIXED] If all participants answer wrong, game will stuck.

Code quality:
* [FIXED] Linter shipped with code, that fails on code quality check. Run _npm run lint_ in ClientApp directory.
* [FIXED] Dead code should not be shipped. Some examples: SampleDataController, unused e2e tests that are failing when executed.

Other concerns:
* There is no server-side validation whether the person actually answered correctly or not. Server-side code trusts anything that comes from client side (browser).

Author: Dennis Peld  
Language: C#, .NET Core 2.2  
Environment: JetBrains Rider