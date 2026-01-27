using System.Text.Json.Nodes;

namespace Trick;

static class JsonExtensions {
  
  public static T Get<T>(this JsonObject obj, string key) {
    var node = obj[key] ??
      throw new Exception("Cant get json node");

    var val = node.GetValue<T>();

    if(val is not T) // Maybe dont need?
      throw new Exception("");

    return val;
  }

}
