stonehenge
==========
An open source .NET Framework to use Web UI technologies for desktop applications.

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

Note: It's chatty - it's designed for local use.

Current project state: Experimental

![Stonehenge](http://ict-baden.de/images/stonehenge.png)

A great application has to be founded on solid pilars of frameworks.

Features
--------
* WPF like ViewModels
* No JavaScript coding necessary

Known Problems
--------------
* IE (all versions)

TODO
----
* localization

Planned Improvements
--------------------
* Replace long running polls by modern communication
	as soon as ServiceStack (and IE) supports it

