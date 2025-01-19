# Network Self-Service Manager
This is a simple self-service administration utility for segmented home networks based on Mikrotik routers. This is
designed for a specific configuration (although it's a fairly typical multi-segment local network configuration type),
however with a bit of effort, it should be possible to adapt this to your use case.

I originally made this to allow people to edit Wi-Fi ACLs and static DHCP leases on their own. This saves me some time
and enables limited control over their segments, even when I am not available. This project isn't strictly new. I have
previously written [a similar tool][0], using PHP. Back then, I did that to solve the same issue, but learn some Laravel
in the process. This time, I set out to learn integrating gRPC with ASP.NET and Angular, but in the process discovered
some new grounds to tread too.

## The goal
To let users perform some basic administration tasks in their own network segments, without having to go to the proper
administrator, and without giving them full access to the underlying management tools. This is both because they would
offer too much functionality, and they're too complicated. The idea is that people are used to consumer-grade management
tools, and this aims to be close in terms of both UI, UX, and functionality.

## Contents
The project consists of 3 main parts:
- High-level Mikrotik API wrapper
- ASP.NET/gRPC backend
- Angular frontend

### The wrapper
The wrapper uses the Mikrotik API (not the REST API, mind) to query and modify entries. It provides a high-level
asynchronous API, which uses LINQ, to provide an ORM-like experience. Note that only a very limited subset of operations
is actually supported, because the Mikrotik API itself is also very limited. It also doesn't actually implement the
entire API surface, because I did not need it. Honestly, I put in way too much effort into designing and engineering
this wrapper, given that I only use it to read/write DHCP leases and CAPsMAN ACLs.

Still, in the process of designing and implementing it, I've learnt a lot about many of the inner workings of .NET and
some common libraries. Specifically:
- I've discovered that Entity Framework's expression parser is an incredibly complex piece of tech. The same goes for
  any LINQ-based ORM.
- Writing a LINQ provider on its own isn't as complicated as I initially thought it'd be. The whole thing revolves
  around stacking expressions.
- Parsing LINQ expressions can be tricky, with how some things work under the hood. Also, the expression engine itself
  is incredibly flexible and complex.
- Source generation has come a long way since its initial introduction. Most notably, the IDE experience has gotten a
  massive upgrade, because it's no longer necessary to restart it and the compiler service every time you make a change
  to your analyzer or source generator.
- The source analysis and generation APIs themselves has evolved a lot. You can do a lot cooler stuff now, and it's more
  performant to boot.
- However, troubleshooting generated sources remains not fun.
- The gains are worth it though. You can offload a lot of operations that were normally done at runtime, such as
  reflection and inspection, to compile time, netting some performance gains.

So why go to all that trouble for something that does 2 things and is meant for internal use only? I don't know, but I
hope the knowledge gained is worth it.

### The backend
The backend is an ASP.NET Core app, which hosts a gRPC service. There's little more to it than that. There's also all
the standard stuff, such as authentication, database storage, and so on.

### The frontend
The frontend is an Angular Material SPA, which replaces the old project's static HTML backend with something more
responsive. I might eventually also implement some extra functionality into the entire system, but the important thing
is that core stuff is there.

## So why the overengineering?
I don't know. Personal projects for me are usually about creating something practical, or learning something. I
occasionally get to do both at the same time, which is how projects like this are born. And when I learn, I tend to
start with a small base, and keep on expanding upon it until I reach a point, where I am satisfied with what I learned.
This can, at times, lead to runaway complexity. This is in stark contrast to purely practical things, where the
objective is to make it minimal, make it work, and make it fulfill its purpose.

That said, there's always gonna be someone who wants to criticize something, and I bet they can find plenty in this
repository. If you have any problems with the project, feel free to express it, as long as it's constructive. You can
even write me an angry email, if you're so inclined, as long as it's constructive. You can even suggest changes, but I
ask that you explain them. This way, I can learn.

## "Your code sucks!"
So? Give me something constructive, or contribute an improvement. I can't exactly act on your feelings or laconic
statements. I'd like it to be a lesson. Maybe for you, certainly for me.

## Resources
Here's some cool and useful links I used when working on the project.

- [Mikrotik's website][1] - they make some cool hardware, and it's the reason this project exists in the first place
- [Mikrotik's API documentation][2] - this documents just the protocol, for functionality, it's the same as CLI
- [Angular gRPC framework][5] - I used that in this project
- [Not a rick roll][3]
- [Cat video][4]


[0]: https://gitlab.emzi0767.dev/Emzi0767/arctis-local-network-manager
[1]: https://mikrotik.com/
[2]: https://help.mikrotik.com/docs/spaces/ROS/pages/47579160/API
[3]: https://youtu.be/dQw4w9WgXcQ
[4]: https://youtu.be/h3XXlYbdzdU
[5]: https://github.com/smnbbrv/ngx-grpc
