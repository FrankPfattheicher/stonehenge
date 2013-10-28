stonehenge
==========
An open source .NET Framework to use Web UI technologies for desktop and/or web applications.

New in version 0.6: CDN support to improve loading speed.

Motivation
----------
Due to Microsofts unclear future of WPF and the lack of WPF support
with Mono on other platforms there was the idea to use HTML5/CSS for
use with desktop applications.

There are always other products featuring this
* [awesomium](http://awesomium.com/)

But this products are not free...

How It Works
------------
If your .NET WPF Application ist already using the MVVM pattern
just throw away the XAML views and replace them by HTML views.

The .NET framework part parsing XAML, do data binding is
replaced by a proxy and stub transferring data and knockout for binding.

Note: It's chatty - it's designed for local use.

Current project state: Working (Basic)

![Stonehenge](http://ict-baden.de/images/stonehenge.png)

A great application has to be founded on solid pilars of frameworks.

Features
--------
* WPF like ViewModels
* No JavaScript coding necessary
* All controls usable using knockout bindings

Known bugs
----------
* none

TODO
----
* Widgets
* More controls
* Kiosk runner for more browsers
* localization support
* NUGET package
* VisualStudio templates
* Documentation

Planned Improvements
--------------------
* Replace long running polls by modern communication
	as soon as ServiceStack (and IE) supports it

