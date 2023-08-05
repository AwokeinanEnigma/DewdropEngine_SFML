using SFML.Graphics;
using SFML.System;

namespace DewDrop.Collision;

        /// <summary>
    /// Spatial hashing class for handling collision.
    /// </summary>
    internal class SpatialHash
    {
        internal const int CellSize = 256;
        internal const int InitialBucketSize = 4;
        internal const int MaxBucketSize = 512;
        private readonly ICollidable[][] _buckets;
        private VertexArray _debugGridVerts;
        private readonly int _heightInCells;
        private readonly bool[] _touches;
        private readonly int _widthInCells;

        /// <summary>
        /// Creates a new spatial hash
        /// </summary>
        /// <param name="width">The width of the space that this spatial hash handles collision in</param>
        /// <param name="height">The height of the space that this spatial hash handles collision in</param>
        public SpatialHash(int width, int height)
        {
            _widthInCells = (width - 1) / CellSize + 1;
            _heightInCells = (height - 1) / CellSize + 1;
            int num = _widthInCells * _heightInCells;
            _buckets = new ICollidable[num][];
            _touches = new bool[num];
            InitializeDebugGrid();
        }

        private void InitializeDebugGrid()
        {
            uint vertexCount = (uint)((_widthInCells + _heightInCells + 2) * 2);
            _debugGridVerts = new VertexArray(PrimitiveType.Lines, vertexCount);
            int num = _widthInCells * CellSize;
            int num2 = _heightInCells * CellSize;
            uint num3 = 0U;
            while (num3 <= (ulong)_widthInCells)
            {
                _debugGridVerts[num3 * 2U] = new Vertex(new Vector2f(num3 * CellSize, 0f), Color.Blue);
                _debugGridVerts[num3 * 2U + 1U] = new Vertex(new Vector2f(num3 * CellSize, num2), Color.Blue);
                num3 += 1U;
            }

            uint num4 = (uint)((_widthInCells + 1) * 2);
            uint num5 = 0U;
            while (num5 <= (ulong)_heightInCells)
            {
                _debugGridVerts[num4 + num5 * 2U] = new Vertex(new Vector2f(0f, num5 * CellSize), Color.Blue);
                _debugGridVerts[num4 + num5 * 2U + 1U] = new Vertex(new Vector2f(num, num5 * CellSize), Color.Blue);
                num5 += 1U;
            }
        }

        private void ClearTouches()
        {
            Array.Clear(_touches, 0, _touches.Length);
        }
        private int GetPositionHash(int x, int y)
        {
            int num = x / CellSize;
            int num2 = y / CellSize;
            return num + num2 * _widthInCells;
        }
        private void BucketInsert(int hash, ICollidable collidable)
        {
            int num = -1;
            ICollidable[] array = _buckets[hash];
            if (array == null)
            {
                _buckets[hash] = new ICollidable[4];
                array = _buckets[hash];
            }

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == collidable)
                {
                    return;
                }

                if (num < 0 && array[i] == null)
                {
                    num = i;
                }
            }

            if (num >= 0)
            {
                array[num] = collidable;
                return;
            }

            int num2 = array.Length;
            if (num2 * 2 <= MaxBucketSize)
            {
                Array.Resize(ref array, num2 * 2);
                array[num2] = collidable;
                _buckets[hash] = array;
                return;
            }

            string message = string.Format("Cannot to insert more than {0} collidables into a single bucket.", MaxBucketSize);
            throw new InvalidOperationException(message);
        }
        private void BucketRemove(int hash, ICollidable collidable, bool log)
        {
            ICollidable[] array = _buckets[hash];
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i] == collidable)
                    {
                        if (log)
                        {
                            Console.WriteLine($"{collidable} removed!");
                        }
                        array[i] = null;

                        return;
                    }
                }
            }

        }

        public void Insert(ICollidable collidable)
        {
            ClearTouches();
            AABB aabb = collidable.AABB;
            int num = ((int)aabb.Size.X - 1) / CellSize + 1;
            int num2 = ((int)aabb.Size.Y - 1) / CellSize + 1;
            for (int i = 0; i <= num2; i++)
            {
                int y = i == num2
                    ? (int)(collidable.Position.Y + aabb.Position.Y) + (int)aabb.Size.Y
                    : (int)(collidable.Position.Y + aabb.Position.Y) + CellSize * i;
                for (int j = 0; j <= num; j++)
                {
                    int x = j == num
                        ? (int)(collidable.Position.X + aabb.Position.X) + (int)aabb.Size.X
                        : (int)(collidable.Position.X + aabb.Position.X) + CellSize * j;
                    int positionHash = GetPositionHash(x, y);
                    if (positionHash >= 0 && positionHash < _buckets.Length && !_touches[positionHash])
                    {
                        _touches[positionHash] = true;
                        BucketInsert(positionHash, collidable);
                    }
                }
            }
        }


        public void Update(ICollidable collidable, Vector2f oldPosition, Vector2f newPosition)
        {
            ClearTouches();
            AABB aabb = collidable.AABB;
            int num = ((int)aabb.Size.X - 1) / CellSize + 1;
            int num2 = ((int)aabb.Size.Y - 1) / CellSize + 1;



            for (int i = 0; i <= num2; i++)
            {
                int y = i == num2
                    ? (int)(oldPosition.Y + aabb.Position.Y) + (int)aabb.Size.Y
                    : (int)(oldPosition.Y + aabb.Position.Y) + CellSize * i;

                int y2 = i == num2
                    ? (int)(newPosition.Y + aabb.Position.Y) + (int)aabb.Size.Y
                    : (int)(newPosition.Y + aabb.Position.Y) + CellSize * i;

                for (int j = 0; j <= num; j++)
                {
                    int x = j == num
                        ? (int)(oldPosition.X + aabb.Position.X) + (int)aabb.Size.X
                        : (int)(oldPosition.X + aabb.Position.X) + CellSize * j;

                    int x2 = j == num
                        ? (int)(newPosition.X + aabb.Position.X) + (int)aabb.Size.X
                        : (int)(newPosition.X + aabb.Position.X) + CellSize * j;

                    int positionHash = GetPositionHash(x, y);
                    int positionHash2 = GetPositionHash(x2, y2);
                    bool flag = positionHash >= 0 && positionHash < _buckets.Length;
                    bool flag2 = positionHash2 >= 0 && positionHash2 < _buckets.Length;
                    if (flag && !_touches[positionHash] || flag2 && !_touches[positionHash2])
                    {
                        if (flag && positionHash != positionHash2)
                        {
                            BucketRemove(positionHash, collidable, false);
                        }

                        if (flag2 && positionHash != positionHash2)
                        {
                            BucketInsert(positionHash2, collidable);
                        }

                        if (flag)
                        {
                            _touches[positionHash] = true;
                        }

                        if (flag2)
                        {
                            _touches[positionHash2] = true;
                        }
                    }
                }
            }
        }

        public bool CheckPosition(Vector2f position)
        {
            foreach (ICollidable[] collider0 in _buckets)
            {
                foreach (ICollidable collidable in collider0)
                {
                    if (position.X >= collidable.Position.X + collidable.AABB.Position.X && position.X < collidable.Position.X + collidable.AABB.Position.X + collidable.AABB.Size.X && position.Y >= collidable.Position.Y + collidable.AABB.Position.Y && position.Y < collidable.Position.Y + collidable.AABB.Position.Y + collidable.AABB.Size.Y)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return true;

        }

        public void Remove(ICollidable collidable)
        {
            ClearTouches();
            AABB aabb = collidable.AABB;
            int num = ((int)aabb.Size.X - 1) / CellSize + 1;
            int num2 = ((int)aabb.Size.Y - 1) / CellSize + 1;
            //Console.WriteLine($"removing collider {collidable}");
            for (int i = 0; i <= num2; i++)
            {
                int y = i == num2
                    ? (int)(collidable.Position.Y + aabb.Position.Y) + (int)aabb.Size.Y
                    : (int)(collidable.Position.Y + aabb.Position.Y) + CellSize * i;
                for (int j = 0; j <= num; j++)
                {
                    int x = j == num
                        ? (int)(collidable.Position.X + aabb.Position.X) + (int)aabb.Size.X
                        : (int)(collidable.Position.X + aabb.Position.X) + CellSize * j;
                    int positionHash = GetPositionHash(x, y);
                    if (positionHash >= 0 && positionHash < _buckets.Length && !_touches[positionHash])
                    {
                        _touches[positionHash] = true;
                        BucketRemove(positionHash, collidable, false);
                    }
                }
            }
        }

        public void Query(Vector2f point, Stack<ICollidable> resultStack)
        {
            int positionHash = GetPositionHash((int)point.X, (int)point.Y);
            if (positionHash < 0 || positionHash >= _buckets.Length || _touches[positionHash])
            {
                return;
            }

            ICollidable[] array = _buckets[positionHash];
            if (array != null)
            {
                foreach (ICollidable t in array)
                {
                    if (t != null)
                    {
                        resultStack.Push(t);
                    }
                }
            }
        }

        public void Query(ICollidable collidable, Stack<ICollidable> resultStack)
        {
            Query(collidable, new Vector2f(0f, 0f), resultStack);
        }

        public void Query(ICollidable collidable, Vector2f offset, Stack<ICollidable> resultStack)
        {
            this.ClearTouches();
            AABB aabb = collidable.AABB;
            int num = ((int)aabb.Size.X - 1) / CellSize + 1;
            int num2 = ((int)aabb.Size.Y - 1) / CellSize + 1;
            for (int i = 0; i <= num2; i++)
            {
                int y = (i == num2) ? ((int)(collidable.Position.Y + aabb.Position.Y) + (int)aabb.Size.Y) : ((int)(collidable.Position.Y + aabb.Position.Y) + CellSize * i);
                for (int j = 0; j <= num; j++)
                {
                    int x = (j == num) ? ((int)(collidable.Position.X + aabb.Position.X) + (int)aabb.Size.X) : ((int)(collidable.Position.X + aabb.Position.X) + CellSize * j);
                    int positionHash = this.GetPositionHash(x, y);
                    if (positionHash >= 0 && positionHash < this._buckets.Length && !this._touches[positionHash])
                    {
                        this._touches[positionHash] = true;
                        ICollidable[] array = this._buckets[positionHash];
                        if (array != null)
                        {
                            foreach (ICollidable t in array)
                            {
                                if (t != null && t != collidable)
                                {
                                    resultStack.Push(t);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Clear()
        {
            foreach (ICollidable[] array in _buckets)
            {
                if (array != null)
                {
                    for (int j = 0; j < array.Length; j++)
                    {
                        array[j] = null;
                    }
                }
            }
        }

        public void DebugDraw(RenderTarget target)
        {
            RenderStates states = new(BlendMode.Alpha, Transform.Identity, null, null);
            foreach (ICollidable[] array in _buckets)
            {
                if (array != null)
                {
                    foreach (ICollidable collidable in array)
                    {
                        if (collidable != null && collidable.DebugVerts != null)
                        {

                            states.Transform = Transform.Identity;
                            states.Transform.Translate(collidable.Position);
                            target.Draw(collidable.DebugVerts, states);
                        }
                    }
                }
            

                target.Draw(_debugGridVerts);
            }
        }
}