public class UnitTests
{
    // ���� �-�� ��� ������������� ������� �������� � ���������� ������
    [Fact]
    public void DistanceOfTrail_Returns_CorrectDistance()
    {
        // Arrange
        int[][] graph = new int[][]
        {
            new int[] { 0, 2, 3, 2, 1, 5 },
            new int[] { 2, 0, 6, 2, 5, 1 },
            new int[] { 3, 6, 0, 3, 2, 7 },
            new int[] { 2, 2, 3, 0, 5, 1 },
            new int[] { 1, 5, 2, 5, 0, 9 },
            new int[] { 5, 1, 7, 1, 9, 0 },
        };

        int[] trail = { 0, 1, 2, 3, 4, 5 }; // ������ ����

        // Act
        double distance = AntColony.DistanceOfTrail(trail, graph);

        // Assert
        // ��������� ���������� ������ ���� ������ ���������� ����� ����������������� ������
        // � ���������� �� ���������� ���� �� ������� ����
        double expectedDistance = graph[0][1] + graph[1][2] + graph[2][3] + graph[3][4] + graph[4][5] + graph[5][0];
        Assert.Equal(expectedDistance, distance);
    }
    [Fact]
    public void InitAnts_ReturnsCorrectNumberOfAnts()
    {
        // Arrange
        int numAnts = 3;
        int numNodes = 6;

        // Act
        int[][] ants = AntColony.InitAnts(numAnts, numNodes);

        // Assert
        Assert.Equal(numAnts, ants.Length);
    }
    // ���� �-��� ������������� ����� � ��������� ������ ����
    [Fact]
    public void InitializeGraph_InitializedCorrectly()
    {
        int[][] expectedGraph = new int[][]
        {
            new int[] { 0, 2, 3, 2, 1, 5 },
            new int[] { 2, 0, 6, 2, 5, 1 },
            new int[] { 3, 6, 0, 3, 2, 7 },
            new int[] { 2, 2, 3, 0, 5, 1 },
            new int[] { 1, 5, 2, 5, 0, 9 },
            new int[] { 5, 1, 7, 1, 9, 0 }
        };

        int[][] graph = AntColony.InitializeGraph();

        Assert.Equal(expectedGraph, graph);
    }
    // �������� �������� ���������� ���� ������� � ���������� ����
    [Fact]
    public void TakeRandomTrail_CreatedCorrectly()
    {
        int start = 1;
        int numNodes = 4;

        int[] trail = AntColony.TakeRandomTrail(start, numNodes);

        Assert.Equal(numNodes, trail.Length);

        // ��������, ��� ������ ���� ����������� � ���� ���� ���
        for (int i = 0; i < numNodes; i++)
        {
            Assert.Contains(i, trail);
        }
    }
    [Fact]
    public void UpdateAnts_UpdatesAntsPaths()
    {
        // Arrange
        int numAnts = 3;
        int numNodes = 6;
        int[][] ants = new int[numAnts][];
        double[][] pheromones = new double[numNodes][];
        int[][] dists = AntColony.InitializeGraph();
        for (int i = 0; i < numNodes; i++) // ������������� ���������
        {
            pheromones[i] = new double[numNodes];
            for (int j = 0; j < numNodes; j++)
            {
                pheromones[i][j] = 0.01; 
            }
        }
        // Act
        AntColony.UpdateAnts(ants, pheromones, dists);

        // Assert
        // �������� ��������� �� ����
        for (int k = 0; k < numAnts; k++)
        {
            Assert.NotNull(ants[k]); // ���� �� ������ = 0
            Assert.Equal(numNodes, ants[k].Length); // ����� ���� ������ ���� = ���-�� �����
            // ��������� �� ������ ���� �� ���� � ��������� �����
            foreach (var node in ants[k])
            {
                Assert.InRange(node, 0, numNodes - 1);
            }
        }
    }
}

