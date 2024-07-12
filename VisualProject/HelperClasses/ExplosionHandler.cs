using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualProject.Extensions;

namespace VisualProject.HelperClasses
{
    public class ExplosionHandler
    {
        private int _explosionFrameRate;
        private int _explosionNumber;
        private TimeSpan _explosionTimer;
        private Bitmap _sprite;

        public bool ExplosionNotComplete { get; private set; }

        public ExplosionHandler()
        {
            _explosionFrameRate = 15;
            _explosionNumber = 1;
            _sprite = new(Image.FromFile(Directory.GetCurrentDirectory().Split("VisualProject").First() + @$"VisualProject\VisualProject\Sprites\Explosion{_explosionNumber}.png"));
        }

        public List<Polygon> GetExplosionSprite(Point explosionLocation, double rotation, int explosionSize = 5)
        {
            List<Polygon> result = ImageReader.GetPolygonsFromImage(explosionLocation, _sprite, explosionSize);

            for (int i = 0; i < result.Count; i++)
                for (int j = 0; j < result[i].Points.Count; j++)
                    result[i].Points[j] = result[i].Points[j].Rotate(explosionLocation, rotation);

            return result;
        }

        public void UpdateExplosionNumber(List<(Keys key, TimeSpan time)> pressedTimers)
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
