using VisualProject.Extensions;

namespace VisualProject.HelperClasses
{
    public class ExplosionHandler
    {
        private int _explosionFrameRate;
        private int _explosionNumber;
        private TimeSpan _explosionTimer;
        private Bitmap _sprite;

        /// <summary>
        /// Is true until the explosion animation is finished.
        /// </summary>
        public bool ExplosionNotComplete { get; private set; }

        public ExplosionHandler()
        {
            _explosionFrameRate = 15;
            _explosionNumber = 1;
            _sprite = new(Image.FromFile(Directory.GetCurrentDirectory().Split("VisualProject").First() + @$"VisualProject\VisualProject\Sprites\Explosion{_explosionNumber}.png"));
        }

        /// <summary>
        /// Gets the sprite for the explosion.
        /// </summary>
        /// <param name="explosionLocation"><see cref="Point"/> at the center of the explosion.</param>
        /// <param name="rotation">The radians to rotate the explosion sprite.</param>
        /// <param name="explosionSize">How big to make each pixel of the sprite.</param>
        /// <returns></returns>
        public List<Polygon> GetExplosionSprite(Point explosionLocation, double rotation, int explosionSize = 5)
        {
            List<Polygon> result = ImageReader.GetPolygonsFromImage(explosionLocation, _sprite, explosionSize);

            for (int i = 0; i < result.Count; i++)
                for (int j = 0; j < result[i].Points.Count; j++)
                    result[i].Points[j] = result[i].Points[j].Rotate(explosionLocation, rotation);

            return result;
        }

        /// <summary>
        /// Updates which image that's shown based on how long has passed.
        /// </summary>
        /// <param name="pressedTimers"></param>
        public void UpdateExplosionSprite(List<(Keys key, TimeSpan time)> pressedTimers)
        {
            _explosionTimer += pressedTimers.First(x => x.key == Keys.F20).time;

            while (_explosionTimer > TimeSpan.FromMilliseconds(1000 / _explosionFrameRate))
            {
                _explosionTimer -= TimeSpan.FromMilliseconds(1000 / _explosionFrameRate);
                _explosionNumber++;
            }

            ExplosionNotComplete = _explosionNumber <= 7;

            // Make sure _explosionNumber is within bounds.
            _explosionNumber = Math.Max(_explosionNumber, 1);
            _sprite = new(Image.FromFile(Directory.GetCurrentDirectory().Split("VisualProject").First() + @$"VisualProject\VisualProject\Sprites\Explosion{(_explosionNumber <= 7 ? _explosionNumber : 7)}.png"));
        }
    }
}
