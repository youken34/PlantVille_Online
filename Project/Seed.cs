using System;
namespace Plantvill
{
    public class Seed
    {
        private static int nextIndex = 0;
        public int index { get; }
        public string name;
        public int price;
        public int reward;
        public DateTime timePurchased;
        public TimeSpan timeToHarvest;
        public TimeSpan timeBeforeSpoiled;

        public Seed()
        {
            index = nextIndex;
        }
        public Seed(string name, int price, int reward, int timetoharvest, int timebeforespoiled)
        {
            this.name = name;
            this.price = price;
            this.reward = reward;
            this.timeToHarvest = TimeSpan.FromSeconds(timetoharvest);
            this.timeBeforeSpoiled = TimeSpan.FromSeconds(timebeforespoiled);
            this.timePurchased = DateTime.UtcNow;
            index = nextIndex;
            nextIndex++;
        }

    }
    public class DefaultSeeds
    {
        public Seed StrawberrySeed = new Seed("Strawberry", 2, 8, 30, 900);
        public Seed SpinachSeed = new Seed("Spinach", 5, 21, 60, 900);
        public Seed PearSeed = new Seed("Pears", 3, 20, 180, 900);

    }
}