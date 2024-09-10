# StreamRegex
Adds stream and text reader functionality to Regex

# Usage
```csharp
var stream=...;
var regex=new Regex(...);
var match = regex.Match(stream);
while (match.Success) {
  ...
  match=match.NextMatch();
}
```

You may specify the Encoding to be used as well as a maxMatchSize and a bufferSize:
- bufferSize is the size of the memory buffer used to find matches. A smaller value will save memory, but lose performance. Default value: 65536 bytes
- maxMatchSize is important for performance. It represents how possibly large a match could be. For example if you know your search will not have more than 1000 bytes, use that so that the code will not load data for no reason.

You can use regex.Match with Stream or with TextReader (the latter has the advantage that it already has an Encoding baked in).

Enjoy! 
