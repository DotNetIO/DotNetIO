namespace DotNetIO.Functional

open DotNetIO

module File =

  let copy (f : File) t =
    f.CopyTo t

// etc