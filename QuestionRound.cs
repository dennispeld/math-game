using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MathGame
{
    /// <summary>
    /// Tha class QuestionRound that stores the information about the currently run question
    /// </summary>
    public class QuestionRound
    {
        public string Question { get; }
        public bool Answer { get; }
        // to see if the question has already a winner
        public bool HasWinner { get; set; }
        
        // Constructor, that initialises the new question
        public QuestionRound()
        {
            this.Question = GenerateQuestion(out bool generatedAnswer);
            this.Answer = generatedAnswer;
            this.HasWinner = false;
        }

        /// <summary>
        /// The method randomly generates a mathematical challenge
        /// </summary>
        /// <param name="generatedAnswer">Correct or wrong answer</param>
        /// <returns>Math challenge</returns>
        private string GenerateQuestion(out bool generatedAnswer)
        {
            double result = new double();
            string generatedQuestionText = string.Empty;
            
            Random rnd = new Random();
            int operandOne = rnd.Next(1, 11); // number from 1 to 10
            int operandTwo = rnd.Next(1, 11); // number from 1 to 10
            int operation = rnd.Next(1, 5); // 1 - add, 2 - subtract, 3 - multiply, 4 - divide

            // randomly decide, if we want to show a correct or wrong answer
            generatedAnswer = rnd.Next(1, 3) == 1 ? true : false; // 1 - yes, 2 - no
            
            switch (operation) {
                case 1:
                    result = operandOne + operandTwo;
                    break;
                case 2:
                    result = operandOne - operandTwo;
                    break;
                case 3:
                    result = operandOne * operandTwo;
                    break;
                case 4:
                    // on divide, we need to return double with 2 decimals after comma
                    result = Math.Round((double)operandOne / (double)operandTwo, 2); 
                    break;
            }

            if (generatedAnswer)
            {
                generatedQuestionText = operandOne + ConvertOperation(operation) + operandTwo + " = " + result;
            }
            else
            {
                // simply add 1 to the result to make it fake
                double fakeResult = result + 1;

                generatedQuestionText = operandOne + ConvertOperation(operation) + operandTwo + " = " + fakeResult;
            }
            
            return generatedQuestionText;
        }

        /// <summary>
        /// A helper method to convert operation number into a string representation
        /// </summary>
        /// <param name="operationNumber">Number 1 to 4</param>
        /// <returns>Symbol add, substract, multiply or divide</returns>
        private string ConvertOperation(int operationNumber)
        {
            switch (operationNumber)
            {
                case 1:
                    return " + ";
                case 2:
                    return " - ";
                case 3:
                    return " * ";
                case 4:
                    return " / ";
                default:
                    return string.Empty;
            }
        }
    }
}