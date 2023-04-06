using System.Text.Json;
using System.Text.Unicode;

namespace ReservaYA_Backend.Services
{
    public static class EncodingHelper
    {
        public static string JsonSpanishSerializer(object objeto, Type type)
        {
            try
            {
                string value = JsonSerializer.Serialize(objeto, type, new JsonSerializerOptions()
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Latin1Supplement)
                });
                return value;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        public static string JsonSpanishSerializerWithReferences(Object objeto, Type type)
        {
            try
            {
                string value = JsonSerializer.Serialize(objeto, type, new JsonSerializerOptions()
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Latin1Supplement),
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
                });
                return value;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}
