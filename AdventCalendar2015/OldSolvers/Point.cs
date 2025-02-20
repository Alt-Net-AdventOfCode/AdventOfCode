namespace AdventCalendar2015
{
    public class Point
    {
        private int X;
        private int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        private bool Equals(Point other)
        {
            return X == other.X && Y == other.Y;
        }

        public Point Move(char dir)
        {
            switch (dir)
            {
                case '>':
                    return new Point(X+1, Y);
                case '<':
                    return new Point(X-1, Y);
                case '^':
                    return new Point(X, Y-1);
                case 'v':
                    return new Point(X, Y+1);
            }

            return this;
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Point) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (X * 397) ^ Y;
            }
        }
    }
}