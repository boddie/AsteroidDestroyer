using System;
using System.Collections.Generic;
using Vector3 = BEPUutilities.Vector3;

namespace AsteroidDestroyer
{
    public class AsteroidSpawner
    {
        private List<Vector3> _locations;

        public Vector3 LevelSize { get; private set; }

        public AsteroidSpawner(int size, int count)
        {
            _locations = new List<Vector3>();
            BuildSize(size);
            BuildLocations(count);
        }

        public List<Vector3> GetAsteroidLocations()
        {
            return _locations;
        }

        private void BuildSize(int size)
        {
            switch (size)
            {
                case 1:
                    LevelSize = new Vector3(300, 300, 300);
                    return;
                case 2:
                    LevelSize = new Vector3(450, 450, 450);
                    return;
                case 3:
                    LevelSize = new Vector3(600, 600, 600);
                    return;
            }
        }

        private void BuildLocations(int count)
        {
            while(_locations.Count < count)
            {
                int x = GameInformation.Instance.Rand.Next(0, (int)LevelSize.X * 20) - (int)LevelSize.X * 10;
                int y = GameInformation.Instance.Rand.Next(0, (int)LevelSize.X * 20) - (int)LevelSize.X * 10;
                int z = GameInformation.Instance.Rand.Next(0, (int)LevelSize.X * 20) - (int)LevelSize.X * 10;

                Vector3 pos = new Vector3(x, y, z);

                if ((x * x + y * y + z * z) < (int)LevelSize.X * 20 && !asteroidClose(pos))
                    _locations.Add(new Vector3(x, y, z));
            }
        }

        private bool asteroidClose(Vector3 pos)
        {
            foreach (Vector3 v in _locations)
            {
                if (Vector3.Distance(v, pos) < 15)
                    return true;
            }
            return false;
        }
    }
}
