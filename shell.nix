{ pkgs ? import <nixpkgs> {} }:

with pkgs;
let
  future = import (builtins.fetchTarball {
        url = "https://github.com/NixOS/nixpkgs/archive/bcd464ccd2a1a7cd09aa2f8d4ffba83b761b1d0e.tar.gz";
    }) {};

  dotnet-10 = future.dotnetCorePackages.dotnet_10.sdk;
in
mkShell {
  packages = [ dotnet-10 ];
  shellHook = ''
    export DOTNET_ROOT="${dotnet-10}";
    export EVENT_PLANNER_HOME=".";
  '';
}
