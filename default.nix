{ fetchFromGitHub
, buildDotnetModule
}:

buildDotnetModule rec {
  pname = "EventPlanner";
  version = "1.0.0";

  src = fetchFromGitHub {
    owner = "MystiriodisLykos";
    repo = "CS-690";
    rev = "v${version}";
    sha256 = "e3eb1ee1967b1205dbf1876e2fc8833414745bec4ce5531c9ac27d4b8f8d63b2";
  };

  projectFile = "event-planner/EventPlanner-cli/EventPlanner-cli.csproj";
}
