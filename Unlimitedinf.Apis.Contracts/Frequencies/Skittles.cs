using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Unlimitedinf.Apis.Contracts.CustomValidators;

namespace Unlimitedinf.Apis.Contracts.Frequencies
{
    /// <summary>
    /// Representing a bag of skittles.
    /// </summary>
    /// <remarks>
    /// Partition Key will be frequency type, subtype, and size. E.g. skittles_classic_fun
    /// RowKey will just be some guid
    /// </remarks>
    public class Skittles
    {
        /// <summary>
        /// <see cref="Auth.Account.username"/>.
        /// </summary>
        [Required, StringLength(100), CustomValidation(typeof(AccountValidator), nameof(AccountValidator.UsernameValidation))]
        public string username { get; set; }

        /// <summary>
        /// The unique identifier for this bag of skittles. Set by the server.
        /// </summary>
        public string id { get; protected set; }

        /// <summary>
        /// The type of bag of skittles.
        /// </summary>
        [Required]
        public SkittleType type { get; set; }

        /// <summary>
        /// The size of the bag of skittles.
        /// </summary>
        [Required]
        public SkittleSize size { get; set; }

        /// <summary>
        /// The count of the colors of the skittles.
        /// </summary>
        [Required]
        public SkittleColors colors { get; set; }

        /// <summary>
        /// The type of bag of skittles.
        /// </summary>
        public enum SkittleType
        {
            /// <summary>
            /// The classic bag of skittles.
            /// </summary>
            classic
        }

        /// <summary>
        /// The size of the bag of skittles.
        /// </summary>
        public enum SkittleSize
        {
            /// <summary>
            /// Fun size
            /// </summary>
            fun,
            /// <summary>
            /// Regular size
            /// </summary>
            regular,
            /// <summary>
            /// King, aka share, size
            /// </summary>
            king
        }

        /// <summary>
        /// Base for skittle colors.
        /// </summary>
        public abstract class SkittleColors
        {
            /// <summary>
            /// Total count of the skittles.
            /// </summary>
            public int total { get; protected set; }
        }

        /// <summary>
        /// The classic skittle colors.
        /// </summary>
        public class SkittleColorClassic : SkittleColors
        {
            /// <summary>
            /// Purple (grape) skittle count.
            /// </summary>
            public int purple { get; set; }
            /// <summary>
            /// Red (strawberry) skittle count.
            /// </summary>
            public int red { get; set; }
            /// <summary>
            /// Yellow (lemon) skittle count.
            /// </summary>
            public int yellow { get; set; }
            /// <summary>
            /// Orange (orange) skittle count.
            /// </summary>
            public int orange { get; set; }
            /// <summary>
            /// Green (apple) skittle count.
            /// </summary>
            public int green { get; set; }
        }
    }

    internal sealed class SkittleStats
    {
        private Skittles skittles;

        /// <summary>
        /// Constructor. Put all the logic to calculate probabilities right in here because why not!
        /// </summary>
        public SkittleStats(Skittles skittles)
        {
            this.skittles = skittles;
            // Things we want to calculate:
            //  - Color distribution compared to normal
        }

        /// <summary>
        /// Turn this into a nice, browser-friendly string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"User:  {this.skittles.username}");
            sb.AppendLine($"Type: {this.skittles.type.ToString()}");
            sb.AppendLine($"Size: {this.skittles.size.ToString()}");
            sb.AppendLine();
            sb.AppendLine();



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
    }
}
