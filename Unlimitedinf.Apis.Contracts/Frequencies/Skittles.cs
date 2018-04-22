using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
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
    /// PartitionKey will be frequency type then username. e.g. skittles_unlimitedinf
    /// RowKey will just be some guid
    /// </remarks>
    public class Skittles
    {
        /// <summary>
        /// Maximum count for a color of skittles. Why would there be more than this in one object?
        /// </summary>
        public const int MaximumColorCount = 1000;

        /// <summary>
        /// <see cref="Auth.Account.username"/>.
        /// </summary>
        [Required, StringLength(100), CustomValidation(typeof(AccountValidator), nameof(AccountValidator.UsernameValidation))]
        public string username { get; set; }

        /// <summary>
        /// The unique identifier for this bag of skittles. Set by the server.
        /// </summary>
        public string id { get; internal set; }

        /// <summary>
        /// The type of the bag of skittles.
        /// </summary>
        [Required, JsonConverter(typeof(StringEnumConverter))]
        public SkittleType type { get; set; }

        /// <summary>
        /// The size of the bag of skittles.
        /// </summary>
        [Required, JsonConverter(typeof(StringEnumConverter))]
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
            /// <remarks>
            /// AFAIK, there isn't a type of skittles with more than 5 colors in it.
            /// </remarks>
            [Required, Range(0, Skittles.MaximumColorCount * 5)]
            public int total { get; set; }
        }

        /// <summary>
        /// The classic skittle colors.
        /// </summary>
        public class SkittleColorClassic : SkittleColors
        {
            /// <summary>
            /// Purple (grape) skittle count.
            /// </summary>
            [Required, Range(0, Skittles.MaximumColorCount)]
            public int purple { get; set; }
            /// <summary>
            /// Red (strawberry) skittle count.
            /// </summary>
            [Required, Range(0, Skittles.MaximumColorCount)]
            public int red { get; set; }
            /// <summary>
            /// Yellow (lemon) skittle count.
            /// </summary>
            [Required, Range(0, Skittles.MaximumColorCount)]
            public int yellow { get; set; }
            /// <summary>
            /// Orange (orange) skittle count.
            /// </summary>
            [Required, Range(0, Skittles.MaximumColorCount)]
            public int orange { get; set; }
            /// <summary>
            /// Green (apple) skittle count.
            /// </summary>
            [Required, Range(0, Skittles.MaximumColorCount)]
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

            sb.AppendLine($"Distribution of all {this.skittles.colors.total} skittles.");
            sb.AppendLine("COLOR  EXP#  ACT#  EXP%  ACT%  VISUALIZATION");
            switch (this.skittles.colors)
            {
                case Skittles.SkittleColorClassic scc:
                    sb.Append($"{nameof(scc.purple).Substring(0, Math.Min(nameof(scc.purple).Length, 6)).PadRight(6)}  ");
                    sb.AppendLine($"{scc.total / 5.0:#0.#}  {scc.purple,4}  {25:0.0}  {100d * scc.purple / scc.total:0.0}  {Visualize(25, 100d * scc.purple / scc.total)}");
                    sb.Append($"{nameof(scc.red).Substring(0, Math.Min(nameof(scc.red).Length, 6)).PadRight(6)}  ");
                    sb.AppendLine($"{scc.total / 5.0:#0.#}  {scc.red,4}  {25:0.0}  {100d * scc.red / scc.total:0.0}  {Visualize(25, 100d * scc.red / scc.total)}");
                    sb.Append($"{nameof(scc.yellow).Substring(0, Math.Min(nameof(scc.yellow).Length, 6)).PadRight(6)}  ");
                    sb.AppendLine($"{scc.total / 5.0:#0.#}  {scc.yellow,4}  {25:0.0}  {100d * scc.yellow / scc.total:0.0}  {Visualize(25, 100d * scc.yellow / scc.total)}");
                    sb.Append($"{nameof(scc.orange).Substring(0, Math.Min(nameof(scc.orange).Length, 6)).PadRight(6)}  ");
                    sb.AppendLine($"{scc.total / 5.0:#0.#}  {scc.orange,4}  {25:0.0}  {100d * scc.orange / scc.total:0.0}  {Visualize(25, 100d * scc.orange / scc.total)}");
                    sb.Append($"{nameof(scc.green).Substring(0, Math.Min(nameof(scc.green).Length, 6)).PadRight(6)}  ");
                    sb.AppendLine($"{scc.total / 5.0:#0.#}  {scc.green,4}  {25:0.0}  {100d * scc.green / scc.total:0.0}  {Visualize(25, 100d * scc.green / scc.total)}");
                    break;

                default:
                    throw new NotImplementedException();
            }

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

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public sealed class SkittlesConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Skittles);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var skit = jo.ToObject<Skittles>();
            switch (skit.type)
            {
                case Skittles.SkittleType.classic:
                    skit.colors = jo[nameof(Skittles.colors)].ToObject<Skittles.SkittleColorClassic>();
                    break;
                default:
                    throw new NotImplementedException();
            }
            return skit;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
