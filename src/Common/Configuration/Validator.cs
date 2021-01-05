using System;
using DA = System.ComponentModel.DataAnnotations;
using YamlDotNet.Core;
using YamlDotNet.Serialization;


namespace Modmail.Configuration
{
  internal class Validator : INodeDeserializer
  {
    private INodeDeserializer deserializer;
    public Validator(INodeDeserializer des)
    {
      this.deserializer = des;
    }

    public bool Deserialize(
      IParser reader,
      Type expectedType,
      Func<IParser, Type, object> nestedObjectDeserializer,
      out object value
    )
    {
      if (this.deserializer.Deserialize(reader, expectedType, nestedObjectDeserializer, out value))
      {
        var context = new DA.ValidationContext(value, null, null);
        DA.Validator.ValidateObject(value, context, true);
        return true;
      }
      return false;
    }
  }
}