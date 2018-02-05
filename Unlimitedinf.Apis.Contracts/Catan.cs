using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Unlimitedinf.Apis.Contracts.CustomValidators;

namespace Unlimitedinf.Apis.Contracts
{
    /// <summary>
    /// Representing a game of Catan's dice rolls.
    /// </summary>
    public class Catan
    {
        /// <summary>
        /// <see cref="Auth.Account.username"/>.
        /// </summary>
        [Required, StringLength(100), CustomValidation(typeof(AccountValidator), nameof(AccountValidator.UsernameValidation))]
        public string username { get; set; }

        /// <summary>
        /// The unique name for this session of Catan.
        /// </summary>
        [Required, StringLength(100), CustomValidation(typeof(KeyFieldValidator), nameof(KeyFieldValidator.KeyFieldValidation))]
        public string name { get; set; }

        /// <summary>
        /// A list of the rolls in the game.
        /// </summary>
        [Required]
        public IList<Roll> rolls { get; set; } = new List<Roll>();

        /// <summary>
        /// Representing a Catan dice roll.
        /// </summary>
        public class Roll
        {
            /// <summary>
            /// Value of the yellow die.
            /// </summary>
            [Required, Range(1, 6)]
            public int y { get; set; }

            /// <summary>
            /// Value of the red die.
            /// </summary>
            [Required, Range(1, 6)]
            public int r { get; set; }

            /// <summary>
            /// When the die were rolled. Usually set on instance creation.
            /// </summary>
            public DateTimeOffset d { get; set; } = DateTimeOffset.Now;
        }
    }

    internal sealed class CatanStats
    {
        private Catan catan;

        /// <summary>
        /// The count of the sum of all rolls.
        /// </summary>
        public int[] Cnt = new int[13];
        /// <summary>
        /// The observed probability of rolling that sum.
        /// </summary>
        public double[] Prb = new double[13];

        // Counts for the individual die
        public int[] YCnt = new int[7];
        public int[] RCnt = new int[7];
        public double[] YPrb = new double[7];
        public double[] RPrb = new double[7];

        /// <summary>
        /// Constructor. Put all the logic to calculate probabilities right in here because why not!
        /// </summary>
        /// <param name="catan"></param>
        public CatanStats(Catan catan)
        {
            this.catan = catan;
            // Things we want to calculate:
            //  - Die roll distribution compared to normal
            //  - "Fairness" of each die individually

            // First sum up all the rolls
            foreach (var roll in this.catan.rolls)
            {
                this.Cnt[roll.y + roll.r]++;
                this.YCnt[roll.y]++;
                this.RCnt[roll.r]++;
            }

            // Next, probability out of 100 for rolls
            for (int i = 2; i < this.Cnt.Length; i++)
                this.Prb[i] = this.Cnt[i] * 100d / this.catan.rolls.Count;
            // Probability out of 100 for each die
            for (int i = 1; i < this.YCnt.Length; i++)
            {
                this.YPrb[i] = this.YCnt[i] * 100d / this.catan.rolls.Count;
                this.RPrb[i] = this.RCnt[i] * 100d / this.catan.rolls.Count;
            }
        }

        /// <summary>
        /// Turn this into a nice, browser-friendly string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"User:  {this.catan.username}");
            sb.AppendLine($"Catan: {this.catan.name}");
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine($"Distribution of all {this.catan.rolls.Count} rolls.");
            sb.AppendLine("RL  EXP#  ACT#  EXP%  ACT%  VISUALIZATION");
            for (int i = 2; i < 13; i++)
                sb.AppendLine($"{i,2}  {(int)(PRB[i] / 100 * this.catan.rolls.Count),4}  {this.Cnt[i],4}  {PRB[i],4:0.0}  {this.Prb[i],4:0.0}  {Visualize(PRB[i], this.Prb[i], 1)}");
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine("Fairness of YELLOW die.");
            sb.AppendLine($"Expected {1 / 6d * this.catan.rolls.Count:0.#} ({1 / 6d:P1}) rolls of each number.");
            sb.AppendLine("RL  ACT#  ACT%  VISUALIZATION");
            for (int i = 1; i < 7; i++)
                sb.AppendLine($"{i,2}  {this.YCnt[i],4}  {this.YPrb[i],4:0.0}  {Visualize(100 / 6d, this.YPrb[i])}");
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine("Fairness of RED die.");
            sb.AppendLine($"Expected {1 / 6d * this.catan.rolls.Count:0.#} ({1 / 6d:P1}) rolls of each number.");
            sb.AppendLine("RL  ACT#  ACT%  VISUALIZATION");
            for (int i = 1; i < 7; i++)
                sb.AppendLine($"{i,2}  {this.RCnt[i],4}  {this.RPrb[i],4:0.0}  {Visualize(100 / 6d, this.RPrb[i])}");

            return sb.ToString();
        }

        /// <summary>
        /// When given two doubles between 0 and 100 representing a percentage, print one character for every
        /// percentage point with some extra information.
        /// </summary>
        /// <param name="expected">The expected percentage.</param>
        /// <param name="actual">The actual percentage.</param>
        /// <param name="scaleFactor">Used to scale the result set.</param>
        /// <param name="ex">Used to print up to the expected or actual percentage, whichever is lower.</param>
        /// <param name="shrt">Used to fill the space after an actual percentage up to the expected percentage.</param>
        /// <param name="lng">Used to fill the space after an expected percentage up to the actual percentage.</param>
        private static string Visualize(double expected, double actual, double scaleFactor = 0.5, char ex = 'O', char shrt = '-', char lng = 'X')
        {
            if (expected < actual)
            {
                return new string(ex, (int)Math.Round(expected * scaleFactor)) + new string(lng, (int)Math.Round((actual - expected) / 2));
            }
            else if (Math.Abs(expected - actual) < 1 / scaleFactor)
            {
                return new string(ex, (int)Math.Round(expected * scaleFactor));
            }
            else // (expected > actual)
            {
                return new string(ex, (int)Math.Round(actual * scaleFactor)) + new string(shrt, (int)Math.Round((expected - actual) / 2));
            }
        }

        /// <summary>
        /// The probability of the sum of two six-sided dice, out of 100.
        /// </summary>
        private static readonly double[] PRB = new double[]
        {
            0,      // 0
            0,      // 1
            100/36d,// 2
            200/36d,// 3
            300/36d,// 4
            400/36d,// 5
            500/36d,// 6
            600/36d,// 7
            500/36d,// 8
            400/36d,// 9
            300/36d,// 10
            200/36d,// 11
            100/36d // 12
        };
    }
}
