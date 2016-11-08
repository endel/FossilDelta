Delta compression algorithm for C#
===

[![Build Status](https://secure.travis-ci.org/endel/FossilDelta.svg?branch=master)](https://travis-ci.org/endel/FossilDelta)

> This is a port from the original C implementation. See references below.

Fossil achieves efficient storage and low-bandwidth synchronization through the
use of delta-compression. Instead of storing or transmitting the complete
content of an artifact, fossil stores or transmits only the changes relative to
a related artifact.

* [Format](http://www.fossil-scm.org/index.html/doc/tip/www/delta_format.wiki)
* [Algorithm](http://www.fossil-scm.org/index.html/doc/tip/www/delta_encoder_algorithm.wiki)
* [Original implementation](http://www.fossil-scm.org/index.html/artifact/f3002e96cc35f37b)

Other implementations:

- [JavaScript](https://github.com/dchest/fossil-delta-js) ([Online demo](https://dchest.github.io/fossil-delta-js/))

Installation
---

### NuGet Gallery

FossilDelta is available on the [NuGet Gallery](https://www.nuget.org/packages).

- [NuGet Gallery: FossilDelta](https://www.nuget.org/packages/FossilDelta)

You can add FossilDelta to your project with the **NuGet Package Manager**, by using the following command in the **Package Manager Console**.

    PM> Install-Package FossilDelta

Usage
---

### Fossil.Delta.Create(byte[] origin, byte[] target)

Returns the difference between `origin` and `target` as a byte array (`byte[]`)

### Fossil.Delta.Apply(byte[] origin, byte[] delta)

Apply the `delta` patch on `origin`, returning the final value as byte array
(`byte[]`).

Throws an error if it fails to apply the delta
(e.g. if it was corrupted).

### Fossil.Delta.OutputSize(byte[] delta)

Returns a size of target for this delta.

Throws an error if it can't read the size from delta.

Benchmark
---

[See the inputs used for benchmarking](Tests/data). Run the benchmarks
locally using the `make benchmark` in your commandline.

**Results:**

```
       Method |           Mean |        StdErr |         StdDev |         Median |
------------- |--------------- |-------------- |--------------- |--------------- |
 CreateDelta1 |  5,426.4132 ns |   787.5304 ns |  6,201.0206 ns |  4,286.4851 ns |
 CreateDelta2 | 21,837.1107 ns | 1,661.4695 ns | 13,900.8509 ns | 25,942.1491 ns |
 CreateDelta3 | 11,697.2018 ns | 1,213.1634 ns | 12,607.5636 ns |  9,260.4452 ns |
 CreateDelta4 |    253.4085 ns |    25.1048 ns |    214.4952 ns |    252.6454 ns |
 CreateDelta5 |    150.4963 ns |    29.0635 ns |    311.6718 ns |      0.0000 ns |
  ApplyDelta1 |  3,547.0234 ns |   493.4131 ns |  4,357.7065 ns |  3,086.8397 ns |
  ApplyDelta2 | 20,336.7691 ns | 2,488.0257 ns | 27,254.9560 ns |  9,233.5811 ns |
  ApplyDelta3 |  1,441.5354 ns |   209.2071 ns |  1,995.7090 ns |    855.9650 ns |
  ApplyDelta4 |    252.5743 ns |    29.7323 ns |    234.1123 ns |    236.0480 ns |
  ApplyDelta5 |     68.5550 ns |     9.2923 ns |     92.4574 ns |     39.0918 ns |
```

