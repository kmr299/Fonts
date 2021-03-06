---
layout: default
title: Font Collections
---

## Working with Font Collections

The `FontCollection` is the root object that manages font styles and families.

### System fonts

We have a pre-configured `FontCollection` at `FontCollection.SystemFonts`. `FontCollection.SystemFonts` is a read only collection with access to all the supported fonts installed in your operating system that the process can find.

### Installing fonts
 
Installing a new font will return a `Font` object representing the newly installed font at a default point size.

#### Installing from a file

```c#
var collection = new FontCollection();
var font = collection.Install(@"c:\path\to\font.ttf");
```

#### Installing from a stream

```c#
var collection = new FontCollection();
var font = collection.Install(stream);
``` 
Assumes you have a seekable stream set at the start of the font, this could be a memory stream, a file stream or any other type.


###  `FontCollection` properties

#### `Families`

A  `FontCollection`  exposes a `Families` property which is a readonly collection of all the `FontFamily`s installed into the collection.

These `FontFamily` objects can be used later to create [`Font`s]({{ site.baseurl }}{% link docs/fonts.md %})


###  `FontCollection` methods


#### `Install(Stream stream)`
Installs a new font from a stream  into the collection and returns a `Font` that represents the contents of the font data.

> Stream must be seekable

#### `Install(string path)`
Installs a new font from a file path into the collection and returns a `Font` that represents the contents of the font data.

#### `Find(string familyName)`

Searchs the collectino for a named `FontFamily` returning null if not found.
