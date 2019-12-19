namespace MCWorldConverter
{
    internal struct Point
    {
        internal int x;
        internal int y;
        internal Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static bool operator <(Point a, Point b) => a.x < b.x && a.y < b.y;
        public static bool operator >(Point a, Point b) => a.x > b.x && a.y > b.y;
    }
}
