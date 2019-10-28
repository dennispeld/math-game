using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MathGame.Hubs
{
    /// <summary>
    /// The class that is responsible for the client-server dialog
    /// </summary>
    public class GameHub: Hub
    {
        // a static variable to store the state of a current question round
        private static QuestionRound round = new QuestionRound();

        // a static variable to store the number of current connections
        private static int currentConnections = 0;

        // a static variables to store the number of players, who answered the current question
        // this is required to check if ALL the players answered wrong
        private static int playersSubmittedAnswer = 0;

        // the list of current connection ids
        private static List<string> connectionIDs = new List<string>();

        // a maximum number of allowed concurrent connections
        const int maxConnections = 10;
        
        /// <summary>
        /// The method gets the answer from a player to the server
        /// </summary>
        /// <param name="question"></param>
        /// <param name="answerCorrect"></param>
        /// /// <param name="currentScore"></param>
        /// /// <param name="username"></param>
        /// <returns></returns>
        public async Task SendAnswer(string question, bool answerCorrect, int currentScore, string username)
        {
            string notification = string.Empty;

            playersSubmittedAnswer++;


            if (answerCorrect)
            {
                // if a player answered correct, but not the very first one
                if (round.HasWinner)
                {
                    notification = username + " has answered correct, but was too late.";
                }
                else // add one point if the player is the winner
                {
                    currentScore++;

                    round.HasWinner = true;

                    notification = username + " has won the round (" + question + "). ";
                }
            }
            else // substract one point if the player answered wrong
            {
                if (currentScore > 0)
                    currentScore--;

                notification = username + " has answered wrong.";
            }

            // send to all players a notification
            if (!string.IsNullOrEmpty(notification))
                await Clients.All.SendAsync("updateNotifications", notification);

            // start a countdown for all players
            if (round.HasWinner)
                await Clients.All.SendAsync("startCountDown");

            // send to a player their score
            await Clients.Caller.SendAsync("updateScore", currentScore);

            // if everybody have answered wrong, generate a new question
            if (playersSubmittedAnswer == currentConnections && !round.HasWinner)
            {
                notification = "Everybody has answered wrong. Wait for the next round.";
                round.HasWinner = true; // we need to set HasWinner attribute to generate a new question

                await Clients.All.SendAsync("updateNotifications", notification);
                
                await Clients.All.SendAsync("startCountDown");
            }
        }
        
        /// <summary>
        /// Get a current question of generate a new one
        /// </summary>
        /// <returns></returns>
        public async Task GetQuestion()
        {
            playersSubmittedAnswer = 0;

            // generate a new round of question if the current round has already a winner
            if (round.HasWinner)
                round = new QuestionRound();

            // send a new question to all players
            await Clients.All.SendAsync("getQuestion", round.Question, round.Answer);
        }
        
        /// <summary>
        /// A player joins the game
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task JoinGame(string username)
        {
            if (currentConnections < maxConnections)
            {
                currentConnections++;

                string playerID = Context.ConnectionId;

                connectionIDs.Add(playerID);

                string action = username + " joined the game.";

                // send to the player his username and id (id is for test purpose)
                await Clients.Caller.SendAsync("acceptEntry", username, playerID);

                // send to all players a notification
                await Clients.All.SendAsync("updateNotifications", action);

                // send to all players a number of players online
                await Clients.All.SendAsync("showPlayersOnline", currentConnections);
            }
            else
            {
                await Clients.Caller.SendAsync("rejectEntry");
            }
        }

        /// <summary>
        /// The method decrease the number of connections on disconnect
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (connectionIDs.IndexOf(Context.ConnectionId) != -1)
            {
                currentConnections--;

                connectionIDs.Remove(Context.ConnectionId);

                // send to all players a new number of players online
                await Clients.All.SendAsync("showPlayersOnline", currentConnections);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}