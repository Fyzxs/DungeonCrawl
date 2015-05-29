namespace GrowingTree.Features
{
    abstract class Tile : Feature
    {
        protected Tile(Point point) : base(new Boundary(point, 1, 1))
        {
        }
    }
}
