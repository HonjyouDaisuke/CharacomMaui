using Xunit;
using CharacomMaui.Application.ImageProcessing;
using CharacomMaui.Tests.TestHelpers;
using System.Threading.Tasks;

public class ThinningProcessorTests
{
  [Fact]
  public void Thinning_EmptyImage_NoChange()
  {
    var input = BitmapTestHelper.CreateBinaryBitmap(
      5, 5,
      (x, y) => false); // 全部白

    var result = ThinningProcess.Thinning(input);

    for (int y = 0; y < 5; y++)
    {
      for (int x = 0; x < 5; x++)
      {
        Assert.False(BitmapTestHelper.IsBlack(result, x, y));
      }
    }
  }

  [Fact]
  public void Thinning_OnePixelWideLine_Remains()
  {
    var input = BitmapTestHelper.CreateBinaryBitmap(
      5, 5,
      (x, y) => x == 2); // 中央縦線 1px

    var result = ThinningProcess.Thinning(input);

    for (int y = 0; y < 5; y++)
    {
      Assert.True(BitmapTestHelper.IsBlack(result, 2, y));
    }
  }
  [Fact]
  public void Thinning_ThreePixelWideLine_BecomesSinglePixel()
  {
    var input = BitmapTestHelper.CreateBinaryBitmap(
      7, 7,
      (x, y) => x >= 2 && x <= 4); // 幅3pxの縦線

    var result = ThinningProcess.Thinning(input);

    for (int y = 0; y < 7; y++)
    {
      Assert.True(BitmapTestHelper.IsBlack(result, 3, y)); // 中央
      Assert.False(BitmapTestHelper.IsBlack(result, 2, y));
      Assert.False(BitmapTestHelper.IsBlack(result, 4, y));
    }
  }

  [Fact]
  public void Thinning_CrossShape_CenterPreserved()
  {
    var input = BitmapTestHelper.CreateBinaryBitmap(
      7, 7,
      (x, y) => x == 3 || y == 3);

    var result = ThinningProcess.Thinning(input);

    // 中心点は必ず残る
    Assert.True(BitmapTestHelper.IsBlack(result, 3, 3));
  }
}
