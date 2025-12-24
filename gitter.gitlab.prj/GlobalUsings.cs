#if SYSTEM_TEXT_JSON
global using JsonPropertyNameAttribute = System.Text.Json.Serialization.JsonPropertyNameAttribute;
#elif NEWTONSOFT_JSON
global using JsonPropertyNameAttribute = Newtonsoft.Json.JsonPropertyAttribute;
#endif
