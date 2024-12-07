namespace Dijkstra
{
    public partial class Form1 : Form
    {

        private const int GridSize = 30;
        private const int CellSize = 30;

        private readonly Node[,] grid;
        private Node startNode;
        private Node endNode;
        private bool isRunning = false;

        public Form1()
        {
            InitializeComponent();
            ClientSize = new Size(CellSize * GridSize, CellSize * GridSize + 40);
            grid = new Node[GridSize, GridSize];
            InitializeGrid();

            Button btnStart = new Button()
            {
                Text = "开始寻路",
                Location = new Point(10, ClientSize.Height - 35),
                Width = 120
            };

            btnStart.Click += btnStart_Click;
            Controls.Add(btnStart);

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (isRunning) return;

            isRunning = true;

            var priorityQueue = new SortedDictionary<double, HashSet<Node>>();
            var initialSet = new HashSet<Node> { startNode };
            priorityQueue[0] = initialSet;

            while (priorityQueue.Count > 0)
            {
                var shortestDistance = priorityQueue.Keys.Min();
                var currentNodes = priorityQueue[shortestDistance];
                var currentNode = currentNodes.First();
                currentNodes.Remove(currentNode);

                if (currentNodes.Count == 0)
                {
                    priorityQueue.Remove(shortestDistance);
                }

                if (currentNode == endNode)
                {
                    await ReconstructPath();
                    MessageBox.Show("路径已找到！");
                    isRunning = false;
                    return;
                }

                if (currentNode != startNode)
                {
                    currentNode.State = NodeState.Visited;
                    Invalidate();
                    await Task.Delay(10);
                }

                foreach (var neighbor in GetNeighbors(currentNode))
                {
                    if (neighbor.State == NodeState.Wall || neighbor.State == NodeState.Visited)
                        continue;

                    double newDistance = currentNode.Distance + 1;

                    if (newDistance < neighbor.Distance)
                    {
                        if (neighbor.Distance != double.MaxValue)
                        {
                            priorityQueue[neighbor.Distance].Remove(neighbor);
                        }

                        neighbor.Distance = newDistance;
                        neighbor.Parent = currentNode;

                        if (!priorityQueue.ContainsKey(newDistance))
                        {
                            priorityQueue[newDistance] = new HashSet<Node>();
                        }

                        priorityQueue[newDistance].Add(neighbor);

                        if (neighbor != endNode)
                        {
                            neighbor.State = NodeState.Visited;
                            Invalidate();
                            await Task.Delay(10);
                        }
                    }
                }
            }

            MessageBox.Show("未找到路径");
            isRunning = false;
        }

        private List<Node> GetNeighbors(Node node)
        {
            var neighbors = new List<Node>();
            var directions = new[]
            {
                new Point(0,1),
                new Point(1,0),
                new Point(0,-1),
                new Point(-1,0)
            };

            foreach (var dir in directions)
            {
                int newX = node.Position.X + dir.X;
                int newY = node.Position.Y + dir.Y;

                if (newX >= 0 && newX < GridSize && newY >= 0 && newY < GridSize)
                {
                    neighbors.Add(grid[newX, newY]);
                }
            }

            return neighbors;
        }

        private async Task ReconstructPath()
        {
            var current = endNode;
            while (current != startNode)
            {
                if (current != endNode)
                {
                    current.State = NodeState.Path;
                    Invalidate();
                    await Task.Delay(10);
                }

                current = current.Parent;
            }
        }

        private void InitializeGrid()
        {
            Random random = new Random();
            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize; y++)
                {
                    grid[x, y] = new Node(new Point(x, y));
                    if (random.NextDouble() < 0.2)
                    {
                        grid[x, y].State = NodeState.Wall;
                    }
                }
            }

            startNode = grid[0, 0];
            endNode = grid[GridSize - 1, GridSize - 1];
            startNode.State = NodeState.Start;
            endNode.State = NodeState.End;
            startNode.Distance = 0;
        }

        private readonly Dictionary<NodeState, Color> nodeColor = new Dictionary<NodeState, Color>
        {
            {NodeState.Unvisited,Color.White },
            {NodeState.Visited,Color.LightBlue },
            {NodeState.Path,Color.Yellow },
            {NodeState.Wall,Color.Black },
            {NodeState.Start,Color.Green },
            {NodeState.End,Color.Red },
        };

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            for (int x = 0; x < GridSize; x++)
            {
                for (int y = 0; y < GridSize; y++)
                {
                    var node = grid[x, y];
                    var rect = new Rectangle(x * CellSize, y * CellSize, CellSize, CellSize);
                    g.FillRectangle(new SolidBrush(nodeColor[node.State]), rect);
                    g.DrawRectangle(Pens.Gray, rect);
                }
            }
        }
    }
}
