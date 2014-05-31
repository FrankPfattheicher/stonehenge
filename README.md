stonehenge
==========
An open source .NET Framework to use Web UI technologies for desktop and/or web applications.

New in version 0.9: Session ID stored in URL parameter 'stonehenge_id' 
                    Support for Midori browser - now runs on Raspberry Pi
                    KnockoutJS 3.1.0

Motivation
----------
Due to Microsofts unclear future of WPF and the lack of WPF support
with Mono on other platforms there was the idea to use HTML5/CSS for
use with desktop applications.

Compile one EXE - copy and run on multiple platforms: Windows, Ubuntu and even Raspberry Pi

There are always other products featuring this
* [awesomium](http://awesomium.com/)

But this is not free...

How It Works
------------
If your .NET WPF Application ist already using the MVVM pattern
just throw away the XAML views and replace them by HTML views.

The .NET framework part of doing XAML data binding is
replaced by a proxy and stub transferring data and knockout is used for client site binding.

Note: It's chatty - it's primary designed for local/intranet use.
Sample running on [http://stonehengecs.de/](http://stonehengecs.de/)

Current project state: Working (Basic)

![image](Stonehenge.png)

A great application has to be founded on solid pilars of frameworks.

Features
--------
* WPF like ViewModels
* No JavaScript coding necessary
* All controls usable using knockout bindings
* CDN support for js and css files
* Development environments: VS2010, VS2013, MonoDevelop, XamarinStudio
* VisualStudio template

Known bugs
----------
* none

TODO
----
* Documentation
* include toastr
* More controls
* Widgets
* localization support
* NUGET package

Planned Improvements
--------------------
* single responsible refactoring to isolate ServiceStack
* Replace ServiceStack due to V4.0 is no more open source
* Microsoft Azure hosted version
* Replace long running polls by modern communication as soon as commonly supported
#### Future Plans
* Use self hostet webkit

