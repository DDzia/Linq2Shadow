# Result Model and ShadowRow features

The main question is a "what model should be for non-typed queries?". When Linq2Shadow was starts develop, frist of all was been this question. Fortunately, .NET has DLR and dynamic typing. ShadowRow accept this feature as primary form for each row received from database, i.e. each row is an object of ShadowRow type.

Inheritance chain of Shadow row is short, as you can see:
```
System.Object -> DynamicObject, IReadOnlyDictionary<System.String, System.Object> -> ShadowRow
```

