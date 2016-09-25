Fossil SCM delta compression algorithm
======================================

The cool thing about it is that plain text inputs generate plain text deltas
(binary inputs, of course, may generate binary deltas).

* [Format](http://www.fossil-scm.org/index.html/doc/tip/www/delta_format.wiki)
* [Algorithm](http://www.fossil-scm.org/index.html/doc/tip/www/delta_encoder_algorithm.wiki)
* [Original implementation](http://www.fossil-scm.org/index.html/artifact/f3002e96cc35f37b)

Installation
------------

### NuGet Gallery

FossilDelta is available on the [NuGet Gallery](https://www.nuget.org/packages), as still a **prerelease** version.

- [NuGet Gallery: FossilDelta](https://www.nuget.org/packages/FossilDelta)

You can add FossilDelta to your project with the **NuGet Package Manager**, by using the following command in the **Package Manager Console**.

    PM> Install-Package FossilDelta

Usage
-----

### Fossil.Delta.Create(origin, target)

Returns a delta (as `Array` of bytes) from origin to target (any array-like
object containing bytes, e.g. `Uint8Array`, `Buffer` or plain `Array`).

### Fossil.Delta.Apply(origin, delta)

Returns target (as `Array` of bytes) by applying delta to origin.

Throws an error if it fails to apply the delta
(e.g. if it was corrupted).

### Fossil.Delta.OutputSize(delta)

Returns a size of target for this delta.

Throws an error if it can't read the size from delta.
