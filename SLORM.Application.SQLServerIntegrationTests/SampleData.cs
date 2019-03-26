using System;
using System.Collections.Generic;
using System.Text;

namespace SLORM.Application.SQLServerIntegrationTests
{
    internal class SampleData
    {
        private static int lastGeneratedId = 0;
        private static int randomSeed = 1337;
        private static Random rnd = new Random(randomSeed);

        public int Id { get; set; }
        public string Name { get; set; }
        public float Value { get; set; }
        public double Value2 { get; set; }
        public int Value3 { get; set; }
        public bool Valid { get; set; }
        public DateTime DateTime { get; set; }

        internal static IList<SampleData> GenerateSampleData(int numberOfInstances)
        {
            var generatedData = new List<SampleData>();

            for (var i = 0; i < numberOfInstances; i ++)
            {
                var currentSampleData = new SampleData()
                {
                    Id = GenerateId(),
                    Name = GenerateName(),
                    Value = GenerateValue(),
                    Value2 = GenerateValue2(),
                    Value3 = GenerateValue3(),
                    Valid = GenerateValid(),
                    DateTime = GenerateDateTime(),
                };
                generatedData.Add(currentSampleData);
            }

            return generatedData;
        }

        private static int GenerateId()
        {
            return ++lastGeneratedId;
        }

        private static string GenerateName()
        {
            const int minimumValue = 0;
            const int maximumValue = 100;
            return rnd.Next(minimumValue, maximumValue).ToString();
        }

        private static float GenerateValue()
        {
            const int maximumValue = 100;
            return (float)(rnd.NextDouble() * maximumValue);
        }

        private static double GenerateValue2()
        {
            const int maximumValue = 100;
            return rnd.NextDouble() * maximumValue;
        }

        private static int GenerateValue3()
        {
            const int maximumValue = 25;
            return rnd.Next(0, maximumValue);
        }

        private static bool GenerateValid()
        {
            var rndValue = rnd.Next(0, 2);
            return rndValue == 0;
        }

        private static DateTime GenerateDateTime()
        {
            const int minimumNumberOfDaysToIncrement = 0;
            const int maximumNumberOfDaysToIncrement = 15;
            // This is the date when this was first coded. I'm leaving this comment here for nostalgia purposes.
            var initialDate = new DateTime(2019, 03, 19);
            var daysToIncrement = rnd.Next(minimumNumberOfDaysToIncrement, maximumNumberOfDaysToIncrement);
            return initialDate.AddDays(daysToIncrement);
        }
    }
}
