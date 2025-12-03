using Xunit;
using CharacomMaui.Application.ImageProcessing;
using System.Threading.Tasks;

public class ThinningProcessorTests
{
  [Fact]
  public async Task LineVertical_ShouldBeThinnedCorrectly()
  {
    byte[,] src =
    {
            {0,1,0},
            {0,1,0},
            {0,1,0},
        };

    var processor = new ThinningProcessor();
    var result = await processor.ThinBinaryImageAsync(src);
    PrintArray(result);
    // 細線化後も 1px ラインが残る
    Assert.Equal(1, result[0, 1]);
    Assert.Equal(1, result[1, 1]);
    Assert.Equal(1, result[2, 1]);
  }

  [Fact]
  public async Task IsolatedPoint_ShouldRemain()
  {
    byte[,] src =
    {
            {0,0,0},
            {0,1,0},
            {0,0,0},
        };

    var processor = new ThinningProcessor();
    var result = await processor.ThinBinaryImageAsync(src);

    Assert.Equal(1, result[1, 1]);
  }

  [Fact]
  public async Task ThickLine_ShouldBeThinned()
  {
    byte[,] src =
    {
              {0,1,1,0},
              {0,1,1,0},
              {0,1,1,0},
          };

    var processor = new ThinningProcessor();
    var result = await processor.ThinBinaryImageAsync(src);
    byte[,] exception =
    {
      {0,0,0,0},
      {0,0,0,0},
      {0,0,1,0},
    };
    // ▼ デバッグ出力
    PrintArray(result);
    // 中央のみ残る
    Assert.Equal(result, exception);
    //Assert.Equal(0, result[0, 2]);
  }
  private void PrintArray(byte[,] arr)
  {
    int rows = arr.GetLength(0);
    int cols = arr.GetLength(1);

    Console.WriteLine("=== Thin Result ===");
    for (int y = 0; y < rows; y++)
    {
      for (int x = 0; x < cols; x++)
      {
        Console.Write(arr[y, x]);
      }
      Console.WriteLine();
    }
    Console.WriteLine("====================");
  }
}
