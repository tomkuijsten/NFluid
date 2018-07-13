# NFluid

A way to use the fluid syntax in .net.

Basic sample:

```cs
string start = "input";
var output = 
  start
    .StartFlow()
    .Call(i => i + " + output")
    .Return();
```

Planned:
 - Catch
 - AddParameters (for dynamic injection)
 - much more...
