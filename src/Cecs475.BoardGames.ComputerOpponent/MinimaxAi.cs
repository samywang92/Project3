﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cecs475.BoardGames.ComputerOpponent
{
    /// <summary>
    /// A pair of an IGameMove that was the best move to apply for a given board state,
    /// and the Weight of the board that resulted.
    /// </summary>
    internal struct MinimaxBestMove
    {
        public int Weight { get; set; }
        public IGameMove Move { get; set; }
    }

    /// <summary>
    /// A minimax with alpha-beta pruning implementation of IGameAi.
    /// </summary>
    public class MinimaxAi : IGameAi
    {
        private int mMaxDepth;
        public MinimaxAi(int maxDepth)
        {
            mMaxDepth = maxDepth;
        }

        // The public calls this function, which kicks off the minimax search.
        public IGameMove FindBestMove(IGameBoard b)
        {
            // TODO: call the private FindBestMove with appropriate values for the parameters.
            // mMaxDepth is what the depthLeft should start at.
            // You are maximizing iff the board's current player is 1.

            return FindBestMove(b, mMaxDepth, b.CurrentPlayer == 1,int.MinValue,int.MaxValue).Move;
        }

        private static MinimaxBestMove FindBestMove(IGameBoard b, int depthLeft, bool maximize,int alpha,int beta)
        {
            // Implement the minimax algorithm. 
            // Your first attempt will not use alpha-beta pruning. Once that works, 
            // implement the pruning as discussed in the project notes.
            if (depthLeft == 0 || b.IsFinished)
                return new MinimaxBestMove()
                {
                    Weight = b.Weight,
                    Move = null
                };
            IGameMove bestMove = null;

            foreach (var move in b.GetPossibleMoves()){
                b.ApplyMove(move);
                var w = FindBestMove(b, depthLeft - 1, !maximize, alpha, beta).Weight;
                b.UndoLastMove();
                if (b.CurrentPlayer == 1 && w > alpha){
                    alpha = w;
                    bestMove = move;
                }else if (b.CurrentPlayer == 2 && w < beta){
                    beta = w;
                    bestMove = move;
                }

                if (alpha >= beta) {
                    if (b.CurrentPlayer == 1) {
                        return new MinimaxBestMove() {
                            Weight = beta,
                            Move = bestMove
                        };
                    } else {
                        return new MinimaxBestMove() {
                            Weight = alpha,
                            Move = bestMove
                        };

                    }
                }
            }

            if (b.CurrentPlayer == 1) {
                return new MinimaxBestMove() {
                    Weight = alpha,
                    Move = bestMove
                };
            } else {
                return new MinimaxBestMove() {
                    Weight = beta,
                    Move = bestMove
                };
            }
            
        }

    }
}