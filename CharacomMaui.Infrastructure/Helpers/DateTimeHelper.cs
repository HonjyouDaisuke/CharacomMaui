using System;
using System.Text.Json;

public static class DateTimeHelper
{
  public static DateTime ParseDateTime(JsonElement element)
  {
    var dateString = element.GetString();
    if (DateTime.TryParse(dateString, out var result))
    {
      return result;
    }
    return DateTime.MinValue; // パース失敗時はデフォルト値を返す
  }
}