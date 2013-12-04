


<!DOCTYPE html>
<html>
  <head prefix="og: http://ogp.me/ns# fb: http://ogp.me/ns/fb# githubog: http://ogp.me/ns/fb/githubog#">
    <meta charset='utf-8'>
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
        <title>jquery-throttle-debounce/jquery.ba-throttle-debounce.js at master · cowboy/jquery-throttle-debounce</title>
    <link rel="search" type="application/opensearchdescription+xml" href="/opensearch.xml" title="GitHub" />
    <link rel="fluid-icon" href="https://github.com/fluidicon.png" title="GitHub" />
    <link rel="apple-touch-icon" sizes="57x57" href="/apple-touch-icon-114.png" />
    <link rel="apple-touch-icon" sizes="114x114" href="/apple-touch-icon-114.png" />
    <link rel="apple-touch-icon" sizes="72x72" href="/apple-touch-icon-144.png" />
    <link rel="apple-touch-icon" sizes="144x144" href="/apple-touch-icon-144.png" />
    <link rel="logo" type="image/svg" href="https://github-media-downloads.s3.amazonaws.com/github-logo.svg" />
    <meta property="og:image" content="https://github.global.ssl.fastly.net/images/modules/logos_page/Octocat.png">
    <meta name="hostname" content="github-fe133-cp1-prd.iad.github.net">
    <meta name="ruby" content="ruby 1.9.3p194-tcs-github-tcmalloc (e1c0c3f392) [x86_64-linux]">
    <link rel="assets" href="https://github.global.ssl.fastly.net/">
    <link rel="conduit-xhr" href="https://ghconduit.com:25035/">
    <link rel="xhr-socket" href="/_sockets" />
    


    <meta name="msapplication-TileImage" content="/windows-tile.png" />
    <meta name="msapplication-TileColor" content="#ffffff" />
    <meta name="selected-link" value="repo_source" data-pjax-transient />
    <meta content="collector.githubapp.com" name="octolytics-host" /><meta content="collector-cdn.github.com" name="octolytics-script-host" /><meta content="github" name="octolytics-app-id" /><meta content="4FDCC5F1:3B1D:2328B6A:529FA9A3" name="octolytics-dimension-request_id" /><meta content="921532" name="octolytics-actor-id" /><meta content="FrankPfattheicher" name="octolytics-actor-login" /><meta content="4cf461729c1987bea963310b81adcbb56641119bfc607633f3b0e6c9ac9e0618" name="octolytics-actor-hash" />
    

    
    
    <link rel="icon" type="image/x-icon" href="/favicon.ico" />

    <meta content="authenticity_token" name="csrf-param" />
<meta content="BP+kLRZ4//+owuErUghjDkiK5oI9ySs4KaF+yFKE8pY=" name="csrf-token" />

    <link href="https://github.global.ssl.fastly.net/assets/github-2463c5a914067d4dc7f89efd96e25208e500da6f.css" media="all" rel="stylesheet" type="text/css" />
    <link href="https://github.global.ssl.fastly.net/assets/github2-c99fa84f579e23161cdbae521efd658f27200d73.css" media="all" rel="stylesheet" type="text/css" />
    

    

      <script src="https://github.global.ssl.fastly.net/assets/frameworks-5970f5a0a3dcc441d5f7ff74326ffd59bbe9388e.js" type="text/javascript"></script>
      <script src="https://github.global.ssl.fastly.net/assets/github-09126674d274b008770e46db73d473d5a3d80cb9.js" type="text/javascript"></script>
      
      <meta http-equiv="x-pjax-version" content="13af1f12f85acd999b16800009131da6">

        <link data-pjax-transient rel='permalink' href='/cowboy/jquery-throttle-debounce/blob/bb5bd71e405cd65ec8b750e17cf99dcdcf5cc5c9/jquery.ba-throttle-debounce.js'>
  <meta property="og:title" content="jquery-throttle-debounce"/>
  <meta property="og:type" content="githubog:gitrepository"/>
  <meta property="og:url" content="https://github.com/cowboy/jquery-throttle-debounce"/>
  <meta property="og:image" content="https://github.global.ssl.fastly.net/images/gravatars/gravatar-user-420.png"/>
  <meta property="og:site_name" content="GitHub"/>
  <meta property="og:description" content="jquery-throttle-debounce - jQuery throttle / debounce: Sometimes, less is more!"/>

  <meta name="description" content="jquery-throttle-debounce - jQuery throttle / debounce: Sometimes, less is more!" />

  <meta content="54051" name="octolytics-dimension-user_id" /><meta content="cowboy" name="octolytics-dimension-user_login" /><meta content="549996" name="octolytics-dimension-repository_id" /><meta content="cowboy/jquery-throttle-debounce" name="octolytics-dimension-repository_nwo" /><meta content="true" name="octolytics-dimension-repository_public" /><meta content="false" name="octolytics-dimension-repository_is_fork" /><meta content="549996" name="octolytics-dimension-repository_network_root_id" /><meta content="cowboy/jquery-throttle-debounce" name="octolytics-dimension-repository_network_root_nwo" />
  <link href="https://github.com/cowboy/jquery-throttle-debounce/commits/master.atom" rel="alternate" title="Recent Commits to jquery-throttle-debounce:master" type="application/atom+xml" />

  </head>


  <body class="logged_in  env-production windows vis-public page-blob">
    <div class="wrapper">
      
      
      
      


      <div class="header header-logged-in true">
  <div class="container clearfix">

    <a class="header-logo-invertocat" href="https://github.com/">
  <span class="mega-octicon octicon-mark-github"></span>
</a>

    
    <a href="/notifications" class="notification-indicator tooltipped downwards" data-gotokey="n" title="You have no unread notifications">
        <span class="mail-status all-read"></span>
</a>

      <div class="command-bar js-command-bar  in-repository">
          <form accept-charset="UTF-8" action="/search" class="command-bar-form" id="top_search_form" method="get">

<input type="text" data-hotkey="/ s" name="q" id="js-command-bar-field" placeholder="Search or type a command" tabindex="1" autocapitalize="off"
    
    data-username="FrankPfattheicher"
      data-repo="cowboy/jquery-throttle-debounce"
      data-branch="master"
      data-sha="790480364261048387fc3e80f67ecdcbaf174c23"
  >

    <input type="hidden" name="nwo" value="cowboy/jquery-throttle-debounce" />

    <div class="select-menu js-menu-container js-select-menu search-context-select-menu">
      <span class="minibutton select-menu-button js-menu-target">
        <span class="js-select-button">This repository</span>
      </span>

      <div class="select-menu-modal-holder js-menu-content js-navigation-container">
        <div class="select-menu-modal">

          <div class="select-menu-item js-navigation-item js-this-repository-navigation-item selected">
            <span class="select-menu-item-icon octicon octicon-check"></span>
            <input type="radio" class="js-search-this-repository" name="search_target" value="repository" checked="checked" />
            <div class="select-menu-item-text js-select-button-text">This repository</div>
          </div> <!-- /.select-menu-item -->

          <div class="select-menu-item js-navigation-item js-all-repositories-navigation-item">
            <span class="select-menu-item-icon octicon octicon-check"></span>
            <input type="radio" name="search_target" value="global" />
            <div class="select-menu-item-text js-select-button-text">All repositories</div>
          </div> <!-- /.select-menu-item -->

        </div>
      </div>
    </div>

  <span class="octicon help tooltipped downwards" title="Show command bar help">
    <span class="octicon octicon-question"></span>
  </span>


  <input type="hidden" name="ref" value="cmdform">

</form>
        <ul class="top-nav">
          <li class="explore"><a href="/explore">Explore</a></li>
            <li><a href="https://gist.github.com">Gist</a></li>
            <li><a href="/blog">Blog</a></li>
          <li><a href="https://help.github.com">Help</a></li>
        </ul>
      </div>

    


  <ul id="user-links">
    <li>
      <a href="/FrankPfattheicher" class="name">
        <img height="20" src="https://0.gravatar.com/avatar/a1108f4167032292e23b2b92bc58bcd3?d=https%3A%2F%2Fidenticons.github.com%2Fe23c14e33d931c87e25c50af4a34d5a1.png&amp;r=x&amp;s=140" width="20" /> FrankPfattheicher
      </a>
    </li>

      <li>
        <a href="/new" id="new_repo" class="tooltipped downwards" title="Create a new repo" aria-label="Create a new repo">
          <span class="octicon octicon-repo-create"></span>
        </a>
      </li>

      <li>
        <a href="/settings/profile" id="account_settings"
          class="tooltipped downwards"
          aria-label="Account settings "
          title="Account settings ">
          <span class="octicon octicon-tools"></span>
        </a>
      </li>
      <li>
        <a class="tooltipped downwards" href="/logout" data-method="post" id="logout" title="Sign out" aria-label="Sign out">
          <span class="octicon octicon-log-out"></span>
        </a>
      </li>

  </ul>

<div class="js-new-dropdown-contents hidden">
  

<ul class="dropdown-menu">
  <li>
    <a href="/new"><span class="octicon octicon-repo-create"></span> New repository</a>
  </li>
  <li>
    <a href="/organizations/new"><span class="octicon octicon-organization"></span> New organization</a>
  </li>



    <li class="section-title">
      <span title="cowboy/jquery-throttle-debounce">This repository</span>
    </li>
      <li>
        <a href="/cowboy/jquery-throttle-debounce/issues/new"><span class="octicon octicon-issue-opened"></span> New issue</a>
      </li>
</ul>

</div>


    
  </div>
</div>

      

      




          <div class="site" itemscope itemtype="http://schema.org/WebPage">
    
    <div class="pagehead repohead instapaper_ignore readability-menu">
      <div class="container">
        

<ul class="pagehead-actions">

    <li class="subscription">
      <form accept-charset="UTF-8" action="/notifications/subscribe" class="js-social-container" data-autosubmit="true" data-remote="true" method="post"><div style="margin:0;padding:0;display:inline"><input name="authenticity_token" type="hidden" value="BP+kLRZ4//+owuErUghjDkiK5oI9ySs4KaF+yFKE8pY=" /></div>  <input id="repository_id" name="repository_id" type="hidden" value="549996" />

    <div class="select-menu js-menu-container js-select-menu">
      <a class="social-count js-social-count" href="/cowboy/jquery-throttle-debounce/watchers">
        24
      </a>
      <span class="minibutton select-menu-button with-count js-menu-target" role="button" tabindex="0">
        <span class="js-select-button">
          <span class="octicon octicon-eye-watch"></span>
          Watch
        </span>
      </span>

      <div class="select-menu-modal-holder">
        <div class="select-menu-modal subscription-menu-modal js-menu-content">
          <div class="select-menu-header">
            <span class="select-menu-title">Notification status</span>
            <span class="octicon octicon-remove-close js-menu-close"></span>
          </div> <!-- /.select-menu-header -->

          <div class="select-menu-list js-navigation-container" role="menu">

            <div class="select-menu-item js-navigation-item selected" role="menuitem" tabindex="0">
              <span class="select-menu-item-icon octicon octicon-check"></span>
              <div class="select-menu-item-text">
                <input checked="checked" id="do_included" name="do" type="radio" value="included" />
                <h4>Not watching</h4>
                <span class="description">You only receive notifications for conversations in which you participate or are @mentioned.</span>
                <span class="js-select-button-text hidden-select-button-text">
                  <span class="octicon octicon-eye-watch"></span>
                  Watch
                </span>
              </div>
            </div> <!-- /.select-menu-item -->

            <div class="select-menu-item js-navigation-item " role="menuitem" tabindex="0">
              <span class="select-menu-item-icon octicon octicon octicon-check"></span>
              <div class="select-menu-item-text">
                <input id="do_subscribed" name="do" type="radio" value="subscribed" />
                <h4>Watching</h4>
                <span class="description">You receive notifications for all conversations in this repository.</span>
                <span class="js-select-button-text hidden-select-button-text">
                  <span class="octicon octicon-eye-unwatch"></span>
                  Unwatch
                </span>
              </div>
            </div> <!-- /.select-menu-item -->

            <div class="select-menu-item js-navigation-item " role="menuitem" tabindex="0">
              <span class="select-menu-item-icon octicon octicon-check"></span>
              <div class="select-menu-item-text">
                <input id="do_ignore" name="do" type="radio" value="ignore" />
                <h4>Ignoring</h4>
                <span class="description">You do not receive any notifications for conversations in this repository.</span>
                <span class="js-select-button-text hidden-select-button-text">
                  <span class="octicon octicon-mute"></span>
                  Stop ignoring
                </span>
              </div>
            </div> <!-- /.select-menu-item -->

          </div> <!-- /.select-menu-list -->

        </div> <!-- /.select-menu-modal -->
      </div> <!-- /.select-menu-modal-holder -->
    </div> <!-- /.select-menu -->

</form>
    </li>

  <li>
  

  <div class="js-toggler-container js-social-container starring-container ">
    <a href="/cowboy/jquery-throttle-debounce/unstar"
      class="minibutton with-count js-toggler-target star-button starred upwards"
      title="Unstar this repository" data-remote="true" data-method="post" rel="nofollow">
      <span class="octicon octicon-star-delete"></span><span class="text">Unstar</span>
    </a>

    <a href="/cowboy/jquery-throttle-debounce/star"
      class="minibutton with-count js-toggler-target star-button unstarred upwards"
      title="Star this repository" data-remote="true" data-method="post" rel="nofollow">
      <span class="octicon octicon-star"></span><span class="text">Star</span>
    </a>

      <a class="social-count js-social-count" href="/cowboy/jquery-throttle-debounce/stargazers">
        430
      </a>
  </div>

  </li>


        <li>
          <a href="/cowboy/jquery-throttle-debounce/fork" class="minibutton with-count js-toggler-target fork-button lighter upwards" title="Fork this repo" rel="nofollow" data-method="post">
            <span class="octicon octicon-git-branch-create"></span><span class="text">Fork</span>
          </a>
          <a href="/cowboy/jquery-throttle-debounce/network" class="social-count">76</a>
        </li>


</ul>

        <h1 itemscope itemtype="http://data-vocabulary.org/Breadcrumb" class="entry-title public">
          <span class="repo-label"><span>public</span></span>
          <span class="mega-octicon octicon-repo"></span>
          <span class="author">
            <a href="/cowboy" class="url fn" itemprop="url" rel="author"><span itemprop="title">cowboy</span></a>
          </span>
          <span class="repohead-name-divider">/</span>
          <strong><a href="/cowboy/jquery-throttle-debounce" class="js-current-repository js-repo-home-link">jquery-throttle-debounce</a></strong>

          <span class="page-context-loader">
            <img alt="Octocat-spinner-32" height="16" src="https://github.global.ssl.fastly.net/images/spinners/octocat-spinner-32.gif" width="16" />
          </span>

        </h1>
      </div><!-- /.container -->
    </div><!-- /.repohead -->

    <div class="container">

      <div class="repository-with-sidebar repo-container  ">

        <div class="repository-sidebar">
            

<div class="sunken-menu vertical-right repo-nav js-repo-nav js-repository-container-pjax js-octicon-loaders">
  <div class="sunken-menu-contents">
    <ul class="sunken-menu-group">
      <li class="tooltipped leftwards" title="Code">
        <a href="/cowboy/jquery-throttle-debounce" aria-label="Code" class="selected js-selected-navigation-item sunken-menu-item" data-gotokey="c" data-pjax="true" data-selected-links="repo_source repo_downloads repo_commits repo_tags repo_branches /cowboy/jquery-throttle-debounce">
          <span class="octicon octicon-code"></span> <span class="full-word">Code</span>
          <img alt="Octocat-spinner-32" class="mini-loader" height="16" src="https://github.global.ssl.fastly.net/images/spinners/octocat-spinner-32.gif" width="16" />
</a>      </li>

        <li class="tooltipped leftwards" title="Issues">
          <a href="/cowboy/jquery-throttle-debounce/issues" aria-label="Issues" class="js-selected-navigation-item sunken-menu-item js-disable-pjax" data-gotokey="i" data-selected-links="repo_issues /cowboy/jquery-throttle-debounce/issues">
            <span class="octicon octicon-issue-opened"></span> <span class="full-word">Issues</span>
            <span class='counter'>7</span>
            <img alt="Octocat-spinner-32" class="mini-loader" height="16" src="https://github.global.ssl.fastly.net/images/spinners/octocat-spinner-32.gif" width="16" />
</a>        </li>

      <li class="tooltipped leftwards" title="Pull Requests"><a href="/cowboy/jquery-throttle-debounce/pulls" aria-label="Pull Requests" class="js-selected-navigation-item sunken-menu-item js-disable-pjax" data-gotokey="p" data-selected-links="repo_pulls /cowboy/jquery-throttle-debounce/pulls">
            <span class="octicon octicon-git-pull-request"></span> <span class="full-word">Pull Requests</span>
            <span class='counter'>2</span>
            <img alt="Octocat-spinner-32" class="mini-loader" height="16" src="https://github.global.ssl.fastly.net/images/spinners/octocat-spinner-32.gif" width="16" />
</a>      </li>


        <li class="tooltipped leftwards" title="Wiki">
          <a href="/cowboy/jquery-throttle-debounce/wiki" aria-label="Wiki" class="js-selected-navigation-item sunken-menu-item" data-pjax="true" data-selected-links="repo_wiki /cowboy/jquery-throttle-debounce/wiki">
            <span class="octicon octicon-book"></span> <span class="full-word">Wiki</span>
            <img alt="Octocat-spinner-32" class="mini-loader" height="16" src="https://github.global.ssl.fastly.net/images/spinners/octocat-spinner-32.gif" width="16" />
</a>        </li>
    </ul>
    <div class="sunken-menu-separator"></div>
    <ul class="sunken-menu-group">

      <li class="tooltipped leftwards" title="Pulse">
        <a href="/cowboy/jquery-throttle-debounce/pulse" aria-label="Pulse" class="js-selected-navigation-item sunken-menu-item" data-pjax="true" data-selected-links="pulse /cowboy/jquery-throttle-debounce/pulse">
          <span class="octicon octicon-pulse"></span> <span class="full-word">Pulse</span>
          <img alt="Octocat-spinner-32" class="mini-loader" height="16" src="https://github.global.ssl.fastly.net/images/spinners/octocat-spinner-32.gif" width="16" />
</a>      </li>

      <li class="tooltipped leftwards" title="Graphs">
        <a href="/cowboy/jquery-throttle-debounce/graphs" aria-label="Graphs" class="js-selected-navigation-item sunken-menu-item" data-pjax="true" data-selected-links="repo_graphs repo_contributors /cowboy/jquery-throttle-debounce/graphs">
          <span class="octicon octicon-graph"></span> <span class="full-word">Graphs</span>
          <img alt="Octocat-spinner-32" class="mini-loader" height="16" src="https://github.global.ssl.fastly.net/images/spinners/octocat-spinner-32.gif" width="16" />
</a>      </li>

      <li class="tooltipped leftwards" title="Network">
        <a href="/cowboy/jquery-throttle-debounce/network" aria-label="Network" class="js-selected-navigation-item sunken-menu-item js-disable-pjax" data-selected-links="repo_network /cowboy/jquery-throttle-debounce/network">
          <span class="octicon octicon-git-branch"></span> <span class="full-word">Network</span>
          <img alt="Octocat-spinner-32" class="mini-loader" height="16" src="https://github.global.ssl.fastly.net/images/spinners/octocat-spinner-32.gif" width="16" />
</a>      </li>
    </ul>


  </div>
</div>

            <div class="only-with-full-nav">
              

  

<div class="clone-url open"
  data-protocol-type="http"
  data-url="/users/set_protocol?protocol_selector=http&amp;protocol_type=clone">
  <h3><strong>HTTPS</strong> clone URL</h3>
  <div class="clone-url-box">
    <input type="text" class="clone js-url-field"
           value="https://github.com/cowboy/jquery-throttle-debounce.git" readonly="readonly">

    <span class="js-zeroclipboard url-box-clippy minibutton zeroclipboard-button" data-clipboard-text="https://github.com/cowboy/jquery-throttle-debounce.git" data-copied-hint="copied!" title="copy to clipboard"><span class="octicon octicon-clippy"></span></span>
  </div>
</div>

  

<div class="clone-url "
  data-protocol-type="ssh"
  data-url="/users/set_protocol?protocol_selector=ssh&amp;protocol_type=clone">
  <h3><strong>SSH</strong> clone URL</h3>
  <div class="clone-url-box">
    <input type="text" class="clone js-url-field"
           value="git@github.com:cowboy/jquery-throttle-debounce.git" readonly="readonly">

    <span class="js-zeroclipboard url-box-clippy minibutton zeroclipboard-button" data-clipboard-text="git@github.com:cowboy/jquery-throttle-debounce.git" data-copied-hint="copied!" title="copy to clipboard"><span class="octicon octicon-clippy"></span></span>
  </div>
</div>

  

<div class="clone-url "
  data-protocol-type="subversion"
  data-url="/users/set_protocol?protocol_selector=subversion&amp;protocol_type=clone">
  <h3><strong>Subversion</strong> checkout URL</h3>
  <div class="clone-url-box">
    <input type="text" class="clone js-url-field"
           value="https://github.com/cowboy/jquery-throttle-debounce" readonly="readonly">

    <span class="js-zeroclipboard url-box-clippy minibutton zeroclipboard-button" data-clipboard-text="https://github.com/cowboy/jquery-throttle-debounce" data-copied-hint="copied!" title="copy to clipboard"><span class="octicon octicon-clippy"></span></span>
  </div>
</div>


<p class="clone-options">You can clone with
      <a href="#" class="js-clone-selector" data-protocol="http">HTTPS</a>,
      <a href="#" class="js-clone-selector" data-protocol="ssh">SSH</a>,
      or <a href="#" class="js-clone-selector" data-protocol="subversion">Subversion</a>.
  <span class="octicon help tooltipped upwards" title="Get help on which URL is right for you.">
    <a href="https://help.github.com/articles/which-remote-url-should-i-use">
    <span class="octicon octicon-question"></span>
    </a>
  </span>
</p>


  <a href="http://windows.github.com" class="minibutton sidebar-button">
    <span class="octicon octicon-device-desktop"></span>
    Clone in Desktop
  </a>

              <a href="/cowboy/jquery-throttle-debounce/archive/master.zip"
                 class="minibutton sidebar-button"
                 title="Download this repository as a zip file"
                 rel="nofollow">
                <span class="octicon octicon-cloud-download"></span>
                Download ZIP
              </a>
            </div>
        </div><!-- /.repository-sidebar -->

        <div id="js-repo-pjax-container" class="repository-content context-loader-container" data-pjax-container>
          


<!-- blob contrib key: blob_contributors:v21:44c958abc39e9fe49a3d32349c5cf665 -->

<p title="This is a placeholder element" class="js-history-link-replace hidden"></p>

<a href="/cowboy/jquery-throttle-debounce/find/master" data-pjax data-hotkey="t" class="js-show-file-finder" style="display:none">Show File Finder</a>

<div class="file-navigation">
  

<div class="select-menu js-menu-container js-select-menu" >
  <span class="minibutton select-menu-button js-menu-target" data-hotkey="w"
    data-master-branch="master"
    data-ref="master"
    role="button" aria-label="Switch branches or tags" tabindex="0">
    <span class="octicon octicon-git-branch"></span>
    <i>branch:</i>
    <span class="js-select-button">master</span>
  </span>

  <div class="select-menu-modal-holder js-menu-content js-navigation-container" data-pjax>

    <div class="select-menu-modal">
      <div class="select-menu-header">
        <span class="select-menu-title">Switch branches/tags</span>
        <span class="octicon octicon-remove-close js-menu-close"></span>
      </div> <!-- /.select-menu-header -->

      <div class="select-menu-filters">
        <div class="select-menu-text-filter">
          <input type="text" aria-label="Filter branches/tags" id="context-commitish-filter-field" class="js-filterable-field js-navigation-enable" placeholder="Filter branches/tags">
        </div>
        <div class="select-menu-tabs">
          <ul>
            <li class="select-menu-tab">
              <a href="#" data-tab-filter="branches" class="js-select-menu-tab">Branches</a>
            </li>
            <li class="select-menu-tab">
              <a href="#" data-tab-filter="tags" class="js-select-menu-tab">Tags</a>
            </li>
          </ul>
        </div><!-- /.select-menu-tabs -->
      </div><!-- /.select-menu-filters -->

      <div class="select-menu-list select-menu-tab-bucket js-select-menu-tab-bucket" data-tab-filter="branches">

        <div data-filterable-for="context-commitish-filter-field" data-filterable-type="substring">


            <div class="select-menu-item js-navigation-item selected">
              <span class="select-menu-item-icon octicon octicon-check"></span>
              <a href="/cowboy/jquery-throttle-debounce/blob/master/jquery.ba-throttle-debounce.js"
                 data-name="master"
                 data-skip-pjax="true"
                 rel="nofollow"
                 class="js-navigation-open select-menu-item-text js-select-button-text css-truncate-target"
                 title="master">master</a>
            </div> <!-- /.select-menu-item -->
        </div>

          <div class="select-menu-no-results">Nothing to show</div>
      </div> <!-- /.select-menu-list -->

      <div class="select-menu-list select-menu-tab-bucket js-select-menu-tab-bucket" data-tab-filter="tags">
        <div data-filterable-for="context-commitish-filter-field" data-filterable-type="substring">


            <div class="select-menu-item js-navigation-item ">
              <span class="select-menu-item-icon octicon octicon-check"></span>
              <a href="/cowboy/jquery-throttle-debounce/tree/v1.1/jquery.ba-throttle-debounce.js"
                 data-name="v1.1"
                 data-skip-pjax="true"
                 rel="nofollow"
                 class="js-navigation-open select-menu-item-text js-select-button-text css-truncate-target"
                 title="v1.1">v1.1</a>
            </div> <!-- /.select-menu-item -->
            <div class="select-menu-item js-navigation-item ">
              <span class="select-menu-item-icon octicon octicon-check"></span>
              <a href="/cowboy/jquery-throttle-debounce/tree/v1.0/jquery.ba-throttle-debounce.js"
                 data-name="v1.0"
                 data-skip-pjax="true"
                 rel="nofollow"
                 class="js-navigation-open select-menu-item-text js-select-button-text css-truncate-target"
                 title="v1.0">v1.0</a>
            </div> <!-- /.select-menu-item -->
        </div>

        <div class="select-menu-no-results">Nothing to show</div>
      </div> <!-- /.select-menu-list -->

    </div> <!-- /.select-menu-modal -->
  </div> <!-- /.select-menu-modal-holder -->
</div> <!-- /.select-menu -->

  <div class="breadcrumb">
    <span class='repo-root js-repo-root'><span itemscope="" itemtype="http://data-vocabulary.org/Breadcrumb"><a href="/cowboy/jquery-throttle-debounce" data-branch="master" data-direction="back" data-pjax="true" itemscope="url"><span itemprop="title">jquery-throttle-debounce</span></a></span></span><span class="separator"> / </span><strong class="final-path">jquery.ba-throttle-debounce.js</strong> <span class="js-zeroclipboard minibutton zeroclipboard-button" data-clipboard-text="jquery.ba-throttle-debounce.js" data-copied-hint="copied!" title="copy to clipboard"><span class="octicon octicon-clippy"></span></span>
  </div>
</div>



  <div class="commit file-history-tease">
    <img class="main-avatar" height="24" src="https://1.gravatar.com/avatar/978a1a2a80394217a0e39c84f07a7c16?d=https%3A%2F%2Fidenticons.github.com%2F93851216f80848cd6feece07c3398617.png&amp;r=x&amp;s=140" width="24" />
    <span class="author"><a href="/cowboy" rel="author">cowboy</a></span>
    <time class="js-relative-date" datetime="2010-03-07T11:14:51-08:00" title="2010-03-07 11:14:51">March 07, 2010</time>
    <div class="commit-title">
        <a href="/cowboy/jquery-throttle-debounce/commit/550ea6c4c5cc636e5a839b5543a2ad0f3ebffb7f" class="message" data-pjax="true" title="Slight comment tweak">Slight comment tweak</a>
    </div>

    <div class="participation">
      <p class="quickstat"><a href="#blob_contributors_box" rel="facebox"><strong>1</strong> contributor</a></p>
      
    </div>
    <div id="blob_contributors_box" style="display:none">
      <h2 class="facebox-header">Users who have contributed to this file</h2>
      <ul class="facebox-user-list">
          <li class="facebox-user-list-item">
            <img height="24" src="https://1.gravatar.com/avatar/978a1a2a80394217a0e39c84f07a7c16?d=https%3A%2F%2Fidenticons.github.com%2F93851216f80848cd6feece07c3398617.png&amp;r=x&amp;s=140" width="24" />
            <a href="/cowboy">cowboy</a>
          </li>
      </ul>
    </div>
  </div>

<div id="files" class="bubble">
  <div class="file">
    <div class="meta">
      <div class="info">
        <span class="icon"><b class="octicon octicon-file-text"></b></span>
        <span class="mode" title="File Mode">file</span>
          <span>253 lines (232 sloc)</span>
        <span>10.254 kb</span>
      </div>
      <div class="actions">
        <div class="button-group">
            <a class="minibutton tooltipped leftwards"
               href="http://windows.github.com" title="Open this file in GitHub for Windows">
                <span class="octicon octicon-device-desktop"></span> Open
            </a>
                <a class="minibutton tooltipped upwards"
                   title="Clicking this button will automatically fork this project so you can edit the file"
                   href="/cowboy/jquery-throttle-debounce/edit/master/jquery.ba-throttle-debounce.js"
                   data-method="post" rel="nofollow">Edit</a>
          <a href="/cowboy/jquery-throttle-debounce/raw/master/jquery.ba-throttle-debounce.js" class="button minibutton " id="raw-url">Raw</a>
            <a href="/cowboy/jquery-throttle-debounce/blame/master/jquery.ba-throttle-debounce.js" class="button minibutton ">Blame</a>
          <a href="/cowboy/jquery-throttle-debounce/commits/master/jquery.ba-throttle-debounce.js" class="button minibutton " rel="nofollow">History</a>
        </div><!-- /.button-group -->
          <a class="minibutton danger empty-icon tooltipped downwards"
             href="/cowboy/jquery-throttle-debounce/delete/master/jquery.ba-throttle-debounce.js"
             title="Fork this project and delete file"
             data-method="post" data-test-id="delete-blob-file" rel="nofollow">
          Delete
        </a>
      </div><!-- /.actions -->

    </div>
        <div class="blob-wrapper data type-javascript js-blob-data">
        <table class="file-code file-diff">
          <tr class="file-code-line">
            <td class="blob-line-nums">
              <span id="L1" rel="#L1">1</span>
<span id="L2" rel="#L2">2</span>
<span id="L3" rel="#L3">3</span>
<span id="L4" rel="#L4">4</span>
<span id="L5" rel="#L5">5</span>
<span id="L6" rel="#L6">6</span>
<span id="L7" rel="#L7">7</span>
<span id="L8" rel="#L8">8</span>
<span id="L9" rel="#L9">9</span>
<span id="L10" rel="#L10">10</span>
<span id="L11" rel="#L11">11</span>
<span id="L12" rel="#L12">12</span>
<span id="L13" rel="#L13">13</span>
<span id="L14" rel="#L14">14</span>
<span id="L15" rel="#L15">15</span>
<span id="L16" rel="#L16">16</span>
<span id="L17" rel="#L17">17</span>
<span id="L18" rel="#L18">18</span>
<span id="L19" rel="#L19">19</span>
<span id="L20" rel="#L20">20</span>
<span id="L21" rel="#L21">21</span>
<span id="L22" rel="#L22">22</span>
<span id="L23" rel="#L23">23</span>
<span id="L24" rel="#L24">24</span>
<span id="L25" rel="#L25">25</span>
<span id="L26" rel="#L26">26</span>
<span id="L27" rel="#L27">27</span>
<span id="L28" rel="#L28">28</span>
<span id="L29" rel="#L29">29</span>
<span id="L30" rel="#L30">30</span>
<span id="L31" rel="#L31">31</span>
<span id="L32" rel="#L32">32</span>
<span id="L33" rel="#L33">33</span>
<span id="L34" rel="#L34">34</span>
<span id="L35" rel="#L35">35</span>
<span id="L36" rel="#L36">36</span>
<span id="L37" rel="#L37">37</span>
<span id="L38" rel="#L38">38</span>
<span id="L39" rel="#L39">39</span>
<span id="L40" rel="#L40">40</span>
<span id="L41" rel="#L41">41</span>
<span id="L42" rel="#L42">42</span>
<span id="L43" rel="#L43">43</span>
<span id="L44" rel="#L44">44</span>
<span id="L45" rel="#L45">45</span>
<span id="L46" rel="#L46">46</span>
<span id="L47" rel="#L47">47</span>
<span id="L48" rel="#L48">48</span>
<span id="L49" rel="#L49">49</span>
<span id="L50" rel="#L50">50</span>
<span id="L51" rel="#L51">51</span>
<span id="L52" rel="#L52">52</span>
<span id="L53" rel="#L53">53</span>
<span id="L54" rel="#L54">54</span>
<span id="L55" rel="#L55">55</span>
<span id="L56" rel="#L56">56</span>
<span id="L57" rel="#L57">57</span>
<span id="L58" rel="#L58">58</span>
<span id="L59" rel="#L59">59</span>
<span id="L60" rel="#L60">60</span>
<span id="L61" rel="#L61">61</span>
<span id="L62" rel="#L62">62</span>
<span id="L63" rel="#L63">63</span>
<span id="L64" rel="#L64">64</span>
<span id="L65" rel="#L65">65</span>
<span id="L66" rel="#L66">66</span>
<span id="L67" rel="#L67">67</span>
<span id="L68" rel="#L68">68</span>
<span id="L69" rel="#L69">69</span>
<span id="L70" rel="#L70">70</span>
<span id="L71" rel="#L71">71</span>
<span id="L72" rel="#L72">72</span>
<span id="L73" rel="#L73">73</span>
<span id="L74" rel="#L74">74</span>
<span id="L75" rel="#L75">75</span>
<span id="L76" rel="#L76">76</span>
<span id="L77" rel="#L77">77</span>
<span id="L78" rel="#L78">78</span>
<span id="L79" rel="#L79">79</span>
<span id="L80" rel="#L80">80</span>
<span id="L81" rel="#L81">81</span>
<span id="L82" rel="#L82">82</span>
<span id="L83" rel="#L83">83</span>
<span id="L84" rel="#L84">84</span>
<span id="L85" rel="#L85">85</span>
<span id="L86" rel="#L86">86</span>
<span id="L87" rel="#L87">87</span>
<span id="L88" rel="#L88">88</span>
<span id="L89" rel="#L89">89</span>
<span id="L90" rel="#L90">90</span>
<span id="L91" rel="#L91">91</span>
<span id="L92" rel="#L92">92</span>
<span id="L93" rel="#L93">93</span>
<span id="L94" rel="#L94">94</span>
<span id="L95" rel="#L95">95</span>
<span id="L96" rel="#L96">96</span>
<span id="L97" rel="#L97">97</span>
<span id="L98" rel="#L98">98</span>
<span id="L99" rel="#L99">99</span>
<span id="L100" rel="#L100">100</span>
<span id="L101" rel="#L101">101</span>
<span id="L102" rel="#L102">102</span>
<span id="L103" rel="#L103">103</span>
<span id="L104" rel="#L104">104</span>
<span id="L105" rel="#L105">105</span>
<span id="L106" rel="#L106">106</span>
<span id="L107" rel="#L107">107</span>
<span id="L108" rel="#L108">108</span>
<span id="L109" rel="#L109">109</span>
<span id="L110" rel="#L110">110</span>
<span id="L111" rel="#L111">111</span>
<span id="L112" rel="#L112">112</span>
<span id="L113" rel="#L113">113</span>
<span id="L114" rel="#L114">114</span>
<span id="L115" rel="#L115">115</span>
<span id="L116" rel="#L116">116</span>
<span id="L117" rel="#L117">117</span>
<span id="L118" rel="#L118">118</span>
<span id="L119" rel="#L119">119</span>
<span id="L120" rel="#L120">120</span>
<span id="L121" rel="#L121">121</span>
<span id="L122" rel="#L122">122</span>
<span id="L123" rel="#L123">123</span>
<span id="L124" rel="#L124">124</span>
<span id="L125" rel="#L125">125</span>
<span id="L126" rel="#L126">126</span>
<span id="L127" rel="#L127">127</span>
<span id="L128" rel="#L128">128</span>
<span id="L129" rel="#L129">129</span>
<span id="L130" rel="#L130">130</span>
<span id="L131" rel="#L131">131</span>
<span id="L132" rel="#L132">132</span>
<span id="L133" rel="#L133">133</span>
<span id="L134" rel="#L134">134</span>
<span id="L135" rel="#L135">135</span>
<span id="L136" rel="#L136">136</span>
<span id="L137" rel="#L137">137</span>
<span id="L138" rel="#L138">138</span>
<span id="L139" rel="#L139">139</span>
<span id="L140" rel="#L140">140</span>
<span id="L141" rel="#L141">141</span>
<span id="L142" rel="#L142">142</span>
<span id="L143" rel="#L143">143</span>
<span id="L144" rel="#L144">144</span>
<span id="L145" rel="#L145">145</span>
<span id="L146" rel="#L146">146</span>
<span id="L147" rel="#L147">147</span>
<span id="L148" rel="#L148">148</span>
<span id="L149" rel="#L149">149</span>
<span id="L150" rel="#L150">150</span>
<span id="L151" rel="#L151">151</span>
<span id="L152" rel="#L152">152</span>
<span id="L153" rel="#L153">153</span>
<span id="L154" rel="#L154">154</span>
<span id="L155" rel="#L155">155</span>
<span id="L156" rel="#L156">156</span>
<span id="L157" rel="#L157">157</span>
<span id="L158" rel="#L158">158</span>
<span id="L159" rel="#L159">159</span>
<span id="L160" rel="#L160">160</span>
<span id="L161" rel="#L161">161</span>
<span id="L162" rel="#L162">162</span>
<span id="L163" rel="#L163">163</span>
<span id="L164" rel="#L164">164</span>
<span id="L165" rel="#L165">165</span>
<span id="L166" rel="#L166">166</span>
<span id="L167" rel="#L167">167</span>
<span id="L168" rel="#L168">168</span>
<span id="L169" rel="#L169">169</span>
<span id="L170" rel="#L170">170</span>
<span id="L171" rel="#L171">171</span>
<span id="L172" rel="#L172">172</span>
<span id="L173" rel="#L173">173</span>
<span id="L174" rel="#L174">174</span>
<span id="L175" rel="#L175">175</span>
<span id="L176" rel="#L176">176</span>
<span id="L177" rel="#L177">177</span>
<span id="L178" rel="#L178">178</span>
<span id="L179" rel="#L179">179</span>
<span id="L180" rel="#L180">180</span>
<span id="L181" rel="#L181">181</span>
<span id="L182" rel="#L182">182</span>
<span id="L183" rel="#L183">183</span>
<span id="L184" rel="#L184">184</span>
<span id="L185" rel="#L185">185</span>
<span id="L186" rel="#L186">186</span>
<span id="L187" rel="#L187">187</span>
<span id="L188" rel="#L188">188</span>
<span id="L189" rel="#L189">189</span>
<span id="L190" rel="#L190">190</span>
<span id="L191" rel="#L191">191</span>
<span id="L192" rel="#L192">192</span>
<span id="L193" rel="#L193">193</span>
<span id="L194" rel="#L194">194</span>
<span id="L195" rel="#L195">195</span>
<span id="L196" rel="#L196">196</span>
<span id="L197" rel="#L197">197</span>
<span id="L198" rel="#L198">198</span>
<span id="L199" rel="#L199">199</span>
<span id="L200" rel="#L200">200</span>
<span id="L201" rel="#L201">201</span>
<span id="L202" rel="#L202">202</span>
<span id="L203" rel="#L203">203</span>
<span id="L204" rel="#L204">204</span>
<span id="L205" rel="#L205">205</span>
<span id="L206" rel="#L206">206</span>
<span id="L207" rel="#L207">207</span>
<span id="L208" rel="#L208">208</span>
<span id="L209" rel="#L209">209</span>
<span id="L210" rel="#L210">210</span>
<span id="L211" rel="#L211">211</span>
<span id="L212" rel="#L212">212</span>
<span id="L213" rel="#L213">213</span>
<span id="L214" rel="#L214">214</span>
<span id="L215" rel="#L215">215</span>
<span id="L216" rel="#L216">216</span>
<span id="L217" rel="#L217">217</span>
<span id="L218" rel="#L218">218</span>
<span id="L219" rel="#L219">219</span>
<span id="L220" rel="#L220">220</span>
<span id="L221" rel="#L221">221</span>
<span id="L222" rel="#L222">222</span>
<span id="L223" rel="#L223">223</span>
<span id="L224" rel="#L224">224</span>
<span id="L225" rel="#L225">225</span>
<span id="L226" rel="#L226">226</span>
<span id="L227" rel="#L227">227</span>
<span id="L228" rel="#L228">228</span>
<span id="L229" rel="#L229">229</span>
<span id="L230" rel="#L230">230</span>
<span id="L231" rel="#L231">231</span>
<span id="L232" rel="#L232">232</span>
<span id="L233" rel="#L233">233</span>
<span id="L234" rel="#L234">234</span>
<span id="L235" rel="#L235">235</span>
<span id="L236" rel="#L236">236</span>
<span id="L237" rel="#L237">237</span>
<span id="L238" rel="#L238">238</span>
<span id="L239" rel="#L239">239</span>
<span id="L240" rel="#L240">240</span>
<span id="L241" rel="#L241">241</span>
<span id="L242" rel="#L242">242</span>
<span id="L243" rel="#L243">243</span>
<span id="L244" rel="#L244">244</span>
<span id="L245" rel="#L245">245</span>
<span id="L246" rel="#L246">246</span>
<span id="L247" rel="#L247">247</span>
<span id="L248" rel="#L248">248</span>
<span id="L249" rel="#L249">249</span>
<span id="L250" rel="#L250">250</span>
<span id="L251" rel="#L251">251</span>
<span id="L252" rel="#L252">252</span>

            </td>
            <td class="blob-line-code">
                    <div class="highlight"><pre><div class='line' id='LC1'><span class="cm">/*!</span></div><div class='line' id='LC2'><span class="cm"> * jQuery throttle / debounce - v1.1 - 3/7/2010</span></div><div class='line' id='LC3'><span class="cm"> * http://benalman.com/projects/jquery-throttle-debounce-plugin/</span></div><div class='line' id='LC4'><span class="cm"> * </span></div><div class='line' id='LC5'><span class="cm"> * Copyright (c) 2010 &quot;Cowboy&quot; Ben Alman</span></div><div class='line' id='LC6'><span class="cm"> * Dual licensed under the MIT and GPL licenses.</span></div><div class='line' id='LC7'><span class="cm"> * http://benalman.com/about/license/</span></div><div class='line' id='LC8'><span class="cm"> */</span></div><div class='line' id='LC9'><br/></div><div class='line' id='LC10'><span class="c1">// Script: jQuery throttle / debounce: Sometimes, less is more!</span></div><div class='line' id='LC11'><span class="c1">//</span></div><div class='line' id='LC12'><span class="c1">// *Version: 1.1, Last updated: 3/7/2010*</span></div><div class='line' id='LC13'><span class="c1">// </span></div><div class='line' id='LC14'><span class="c1">// Project Home - http://benalman.com/projects/jquery-throttle-debounce-plugin/</span></div><div class='line' id='LC15'><span class="c1">// GitHub       - http://github.com/cowboy/jquery-throttle-debounce/</span></div><div class='line' id='LC16'><span class="c1">// Source       - http://github.com/cowboy/jquery-throttle-debounce/raw/master/jquery.ba-throttle-debounce.js</span></div><div class='line' id='LC17'><span class="c1">// (Minified)   - http://github.com/cowboy/jquery-throttle-debounce/raw/master/jquery.ba-throttle-debounce.min.js (0.7kb)</span></div><div class='line' id='LC18'><span class="c1">// </span></div><div class='line' id='LC19'><span class="c1">// About: License</span></div><div class='line' id='LC20'><span class="c1">// </span></div><div class='line' id='LC21'><span class="c1">// Copyright (c) 2010 &quot;Cowboy&quot; Ben Alman,</span></div><div class='line' id='LC22'><span class="c1">// Dual licensed under the MIT and GPL licenses.</span></div><div class='line' id='LC23'><span class="c1">// http://benalman.com/about/license/</span></div><div class='line' id='LC24'><span class="c1">// </span></div><div class='line' id='LC25'><span class="c1">// About: Examples</span></div><div class='line' id='LC26'><span class="c1">// </span></div><div class='line' id='LC27'><span class="c1">// These working examples, complete with fully commented code, illustrate a few</span></div><div class='line' id='LC28'><span class="c1">// ways in which this plugin can be used.</span></div><div class='line' id='LC29'><span class="c1">// </span></div><div class='line' id='LC30'><span class="c1">// Throttle - http://benalman.com/code/projects/jquery-throttle-debounce/examples/throttle/</span></div><div class='line' id='LC31'><span class="c1">// Debounce - http://benalman.com/code/projects/jquery-throttle-debounce/examples/debounce/</span></div><div class='line' id='LC32'><span class="c1">// </span></div><div class='line' id='LC33'><span class="c1">// About: Support and Testing</span></div><div class='line' id='LC34'><span class="c1">// </span></div><div class='line' id='LC35'><span class="c1">// Information about what version or versions of jQuery this plugin has been</span></div><div class='line' id='LC36'><span class="c1">// tested with, what browsers it has been tested in, and where the unit tests</span></div><div class='line' id='LC37'><span class="c1">// reside (so you can test it yourself).</span></div><div class='line' id='LC38'><span class="c1">// </span></div><div class='line' id='LC39'><span class="c1">// jQuery Versions - none, 1.3.2, 1.4.2</span></div><div class='line' id='LC40'><span class="c1">// Browsers Tested - Internet Explorer 6-8, Firefox 2-3.6, Safari 3-4, Chrome 4-5, Opera 9.6-10.1.</span></div><div class='line' id='LC41'><span class="c1">// Unit Tests      - http://benalman.com/code/projects/jquery-throttle-debounce/unit/</span></div><div class='line' id='LC42'><span class="c1">// </span></div><div class='line' id='LC43'><span class="c1">// About: Release History</span></div><div class='line' id='LC44'><span class="c1">// </span></div><div class='line' id='LC45'><span class="c1">// 1.1 - (3/7/2010) Fixed a bug in &lt;jQuery.throttle&gt; where trailing callbacks</span></div><div class='line' id='LC46'><span class="c1">//       executed later than they should. Reworked a fair amount of internal</span></div><div class='line' id='LC47'><span class="c1">//       logic as well.</span></div><div class='line' id='LC48'><span class="c1">// 1.0 - (3/6/2010) Initial release as a stand-alone project. Migrated over</span></div><div class='line' id='LC49'><span class="c1">//       from jquery-misc repo v0.4 to jquery-throttle repo v1.0, added the</span></div><div class='line' id='LC50'><span class="c1">//       no_trailing throttle parameter and debounce functionality.</span></div><div class='line' id='LC51'><span class="c1">// </span></div><div class='line' id='LC52'><span class="c1">// Topic: Note for non-jQuery users</span></div><div class='line' id='LC53'><span class="c1">// </span></div><div class='line' id='LC54'><span class="c1">// jQuery isn&#39;t actually required for this plugin, because nothing internal</span></div><div class='line' id='LC55'><span class="c1">// uses any jQuery methods or properties. jQuery is just used as a namespace</span></div><div class='line' id='LC56'><span class="c1">// under which these methods can exist.</span></div><div class='line' id='LC57'><span class="c1">// </span></div><div class='line' id='LC58'><span class="c1">// Since jQuery isn&#39;t actually required for this plugin, if jQuery doesn&#39;t exist</span></div><div class='line' id='LC59'><span class="c1">// when this plugin is loaded, the method described below will be created in</span></div><div class='line' id='LC60'><span class="c1">// the `Cowboy` namespace. Usage will be exactly the same, but instead of</span></div><div class='line' id='LC61'><span class="c1">// $.method() or jQuery.method(), you&#39;ll need to use Cowboy.method().</span></div><div class='line' id='LC62'><br/></div><div class='line' id='LC63'><span class="p">(</span><span class="kd">function</span><span class="p">(</span><span class="nb">window</span><span class="p">,</span><span class="kc">undefined</span><span class="p">){</span></div><div class='line' id='LC64'>&nbsp;&nbsp;<span class="s1">&#39;$:nomunge&#39;</span><span class="p">;</span> <span class="c1">// Used by YUI compressor.</span></div><div class='line' id='LC65'>&nbsp;&nbsp;</div><div class='line' id='LC66'>&nbsp;&nbsp;<span class="c1">// Since jQuery really isn&#39;t required for this plugin, use `jQuery` as the</span></div><div class='line' id='LC67'>&nbsp;&nbsp;<span class="c1">// namespace only if it already exists, otherwise use the `Cowboy` namespace,</span></div><div class='line' id='LC68'>&nbsp;&nbsp;<span class="c1">// creating it if necessary.</span></div><div class='line' id='LC69'>&nbsp;&nbsp;<span class="kd">var</span> <span class="nx">$</span> <span class="o">=</span> <span class="nb">window</span><span class="p">.</span><span class="nx">jQuery</span> <span class="o">||</span> <span class="nb">window</span><span class="p">.</span><span class="nx">Cowboy</span> <span class="o">||</span> <span class="p">(</span> <span class="nb">window</span><span class="p">.</span><span class="nx">Cowboy</span> <span class="o">=</span> <span class="p">{}</span> <span class="p">),</span></div><div class='line' id='LC70'>&nbsp;&nbsp;&nbsp;&nbsp;</div><div class='line' id='LC71'>&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// Internal method reference.</span></div><div class='line' id='LC72'>&nbsp;&nbsp;&nbsp;&nbsp;<span class="nx">jq_throttle</span><span class="p">;</span></div><div class='line' id='LC73'>&nbsp;&nbsp;</div><div class='line' id='LC74'>&nbsp;&nbsp;<span class="c1">// Method: jQuery.throttle</span></div><div class='line' id='LC75'>&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC76'>&nbsp;&nbsp;<span class="c1">// Throttle execution of a function. Especially useful for rate limiting</span></div><div class='line' id='LC77'>&nbsp;&nbsp;<span class="c1">// execution of handlers on events like resize and scroll. If you want to</span></div><div class='line' id='LC78'>&nbsp;&nbsp;<span class="c1">// rate-limit execution of a function to a single time, see the</span></div><div class='line' id='LC79'>&nbsp;&nbsp;<span class="c1">// &lt;jQuery.debounce&gt; method.</span></div><div class='line' id='LC80'>&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC81'>&nbsp;&nbsp;<span class="c1">// In this visualization, | is a throttled-function call and X is the actual</span></div><div class='line' id='LC82'>&nbsp;&nbsp;<span class="c1">// callback execution:</span></div><div class='line' id='LC83'>&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC84'>&nbsp;&nbsp;<span class="c1">// &gt; Throttled with `no_trailing` specified as false or unspecified:</span></div><div class='line' id='LC85'>&nbsp;&nbsp;<span class="c1">// &gt; ||||||||||||||||||||||||| (pause) |||||||||||||||||||||||||</span></div><div class='line' id='LC86'>&nbsp;&nbsp;<span class="c1">// &gt; X    X    X    X    X    X        X    X    X    X    X    X</span></div><div class='line' id='LC87'>&nbsp;&nbsp;<span class="c1">// &gt; </span></div><div class='line' id='LC88'>&nbsp;&nbsp;<span class="c1">// &gt; Throttled with `no_trailing` specified as true:</span></div><div class='line' id='LC89'>&nbsp;&nbsp;<span class="c1">// &gt; ||||||||||||||||||||||||| (pause) |||||||||||||||||||||||||</span></div><div class='line' id='LC90'>&nbsp;&nbsp;<span class="c1">// &gt; X    X    X    X    X             X    X    X    X    X</span></div><div class='line' id='LC91'>&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC92'>&nbsp;&nbsp;<span class="c1">// Usage:</span></div><div class='line' id='LC93'>&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC94'>&nbsp;&nbsp;<span class="c1">// &gt; var throttled = jQuery.throttle( delay, [ no_trailing, ] callback );</span></div><div class='line' id='LC95'>&nbsp;&nbsp;<span class="c1">// &gt; </span></div><div class='line' id='LC96'>&nbsp;&nbsp;<span class="c1">// &gt; jQuery(&#39;selector&#39;).bind( &#39;someevent&#39;, throttled );</span></div><div class='line' id='LC97'>&nbsp;&nbsp;<span class="c1">// &gt; jQuery(&#39;selector&#39;).unbind( &#39;someevent&#39;, throttled );</span></div><div class='line' id='LC98'>&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC99'>&nbsp;&nbsp;<span class="c1">// This also works in jQuery 1.4+:</span></div><div class='line' id='LC100'>&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC101'>&nbsp;&nbsp;<span class="c1">// &gt; jQuery(&#39;selector&#39;).bind( &#39;someevent&#39;, jQuery.throttle( delay, [ no_trailing, ] callback ) );</span></div><div class='line' id='LC102'>&nbsp;&nbsp;<span class="c1">// &gt; jQuery(&#39;selector&#39;).unbind( &#39;someevent&#39;, callback );</span></div><div class='line' id='LC103'>&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC104'>&nbsp;&nbsp;<span class="c1">// Arguments:</span></div><div class='line' id='LC105'>&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC106'>&nbsp;&nbsp;<span class="c1">//  delay - (Number) A zero-or-greater delay in milliseconds. For event</span></div><div class='line' id='LC107'>&nbsp;&nbsp;<span class="c1">//    callbacks, values around 100 or 250 (or even higher) are most useful.</span></div><div class='line' id='LC108'>&nbsp;&nbsp;<span class="c1">//  no_trailing - (Boolean) Optional, defaults to false. If no_trailing is</span></div><div class='line' id='LC109'>&nbsp;&nbsp;<span class="c1">//    true, callback will only execute every `delay` milliseconds while the</span></div><div class='line' id='LC110'>&nbsp;&nbsp;<span class="c1">//    throttled-function is being called. If no_trailing is false or</span></div><div class='line' id='LC111'>&nbsp;&nbsp;<span class="c1">//    unspecified, callback will be executed one final time after the last</span></div><div class='line' id='LC112'>&nbsp;&nbsp;<span class="c1">//    throttled-function call. (After the throttled-function has not been</span></div><div class='line' id='LC113'>&nbsp;&nbsp;<span class="c1">//    called for `delay` milliseconds, the internal counter is reset)</span></div><div class='line' id='LC114'>&nbsp;&nbsp;<span class="c1">//  callback - (Function) A function to be executed after delay milliseconds.</span></div><div class='line' id='LC115'>&nbsp;&nbsp;<span class="c1">//    The `this` context and all arguments are passed through, as-is, to</span></div><div class='line' id='LC116'>&nbsp;&nbsp;<span class="c1">//    `callback` when the throttled-function is executed.</span></div><div class='line' id='LC117'>&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC118'>&nbsp;&nbsp;<span class="c1">// Returns:</span></div><div class='line' id='LC119'>&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC120'>&nbsp;&nbsp;<span class="c1">//  (Function) A new, throttled, function.</span></div><div class='line' id='LC121'>&nbsp;&nbsp;</div><div class='line' id='LC122'>&nbsp;&nbsp;<span class="nx">$</span><span class="p">.</span><span class="nx">throttle</span> <span class="o">=</span> <span class="nx">jq_throttle</span> <span class="o">=</span> <span class="kd">function</span><span class="p">(</span> <span class="nx">delay</span><span class="p">,</span> <span class="nx">no_trailing</span><span class="p">,</span> <span class="nx">callback</span><span class="p">,</span> <span class="nx">debounce_mode</span> <span class="p">)</span> <span class="p">{</span></div><div class='line' id='LC123'>&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// After wrapper has stopped being called, this timeout ensures that</span></div><div class='line' id='LC124'>&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// `callback` is executed at the proper times in `throttle` and `end`</span></div><div class='line' id='LC125'>&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// debounce modes.</span></div><div class='line' id='LC126'>&nbsp;&nbsp;&nbsp;&nbsp;<span class="kd">var</span> <span class="nx">timeout_id</span><span class="p">,</span></div><div class='line' id='LC127'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div><div class='line' id='LC128'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// Keep track of the last time `callback` was executed.</span></div><div class='line' id='LC129'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="nx">last_exec</span> <span class="o">=</span> <span class="mi">0</span><span class="p">;</span></div><div class='line' id='LC130'>&nbsp;&nbsp;&nbsp;&nbsp;</div><div class='line' id='LC131'>&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// `no_trailing` defaults to falsy.</span></div><div class='line' id='LC132'>&nbsp;&nbsp;&nbsp;&nbsp;<span class="k">if</span> <span class="p">(</span> <span class="k">typeof</span> <span class="nx">no_trailing</span> <span class="o">!==</span> <span class="s1">&#39;boolean&#39;</span> <span class="p">)</span> <span class="p">{</span></div><div class='line' id='LC133'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="nx">debounce_mode</span> <span class="o">=</span> <span class="nx">callback</span><span class="p">;</span></div><div class='line' id='LC134'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="nx">callback</span> <span class="o">=</span> <span class="nx">no_trailing</span><span class="p">;</span></div><div class='line' id='LC135'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="nx">no_trailing</span> <span class="o">=</span> <span class="kc">undefined</span><span class="p">;</span></div><div class='line' id='LC136'>&nbsp;&nbsp;&nbsp;&nbsp;<span class="p">}</span></div><div class='line' id='LC137'>&nbsp;&nbsp;&nbsp;&nbsp;</div><div class='line' id='LC138'>&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// The `wrapper` function encapsulates all of the throttling / debouncing</span></div><div class='line' id='LC139'>&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// functionality and when executed will limit the rate at which `callback`</span></div><div class='line' id='LC140'>&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// is executed.</span></div><div class='line' id='LC141'>&nbsp;&nbsp;&nbsp;&nbsp;<span class="kd">function</span> <span class="nx">wrapper</span><span class="p">()</span> <span class="p">{</span></div><div class='line' id='LC142'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="kd">var</span> <span class="nx">that</span> <span class="o">=</span> <span class="k">this</span><span class="p">,</span></div><div class='line' id='LC143'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="nx">elapsed</span> <span class="o">=</span> <span class="o">+</span><span class="k">new</span> <span class="nb">Date</span><span class="p">()</span> <span class="o">-</span> <span class="nx">last_exec</span><span class="p">,</span></div><div class='line' id='LC144'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="nx">args</span> <span class="o">=</span> <span class="nx">arguments</span><span class="p">;</span></div><div class='line' id='LC145'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div><div class='line' id='LC146'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// Execute `callback` and update the `last_exec` timestamp.</span></div><div class='line' id='LC147'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="kd">function</span> <span class="nx">exec</span><span class="p">()</span> <span class="p">{</span></div><div class='line' id='LC148'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="nx">last_exec</span> <span class="o">=</span> <span class="o">+</span><span class="k">new</span> <span class="nb">Date</span><span class="p">();</span></div><div class='line' id='LC149'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="nx">callback</span><span class="p">.</span><span class="nx">apply</span><span class="p">(</span> <span class="nx">that</span><span class="p">,</span> <span class="nx">args</span> <span class="p">);</span></div><div class='line' id='LC150'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="p">};</span></div><div class='line' id='LC151'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div><div class='line' id='LC152'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// If `debounce_mode` is true (at_begin) this is used to clear the flag</span></div><div class='line' id='LC153'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// to allow future `callback` executions.</span></div><div class='line' id='LC154'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="kd">function</span> <span class="nx">clear</span><span class="p">()</span> <span class="p">{</span></div><div class='line' id='LC155'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="nx">timeout_id</span> <span class="o">=</span> <span class="kc">undefined</span><span class="p">;</span></div><div class='line' id='LC156'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="p">};</span></div><div class='line' id='LC157'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div><div class='line' id='LC158'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="k">if</span> <span class="p">(</span> <span class="nx">debounce_mode</span> <span class="o">&amp;&amp;</span> <span class="o">!</span><span class="nx">timeout_id</span> <span class="p">)</span> <span class="p">{</span></div><div class='line' id='LC159'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// Since `wrapper` is being called for the first time and</span></div><div class='line' id='LC160'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// `debounce_mode` is true (at_begin), execute `callback`.</span></div><div class='line' id='LC161'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="nx">exec</span><span class="p">();</span></div><div class='line' id='LC162'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="p">}</span></div><div class='line' id='LC163'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div><div class='line' id='LC164'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// Clear any existing timeout.</span></div><div class='line' id='LC165'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="nx">timeout_id</span> <span class="o">&amp;&amp;</span> <span class="nx">clearTimeout</span><span class="p">(</span> <span class="nx">timeout_id</span> <span class="p">);</span></div><div class='line' id='LC166'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div><div class='line' id='LC167'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="k">if</span> <span class="p">(</span> <span class="nx">debounce_mode</span> <span class="o">===</span> <span class="kc">undefined</span> <span class="o">&amp;&amp;</span> <span class="nx">elapsed</span> <span class="o">&gt;</span> <span class="nx">delay</span> <span class="p">)</span> <span class="p">{</span></div><div class='line' id='LC168'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// In throttle mode, if `delay` time has been exceeded, execute</span></div><div class='line' id='LC169'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// `callback`.</span></div><div class='line' id='LC170'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="nx">exec</span><span class="p">();</span></div><div class='line' id='LC171'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</div><div class='line' id='LC172'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="p">}</span> <span class="k">else</span> <span class="k">if</span> <span class="p">(</span> <span class="nx">no_trailing</span> <span class="o">!==</span> <span class="kc">true</span> <span class="p">)</span> <span class="p">{</span></div><div class='line' id='LC173'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// In trailing throttle mode, since `delay` time has not been</span></div><div class='line' id='LC174'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// exceeded, schedule `callback` to execute `delay` ms after most</span></div><div class='line' id='LC175'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// recent execution.</span></div><div class='line' id='LC176'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC177'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// If `debounce_mode` is true (at_begin), schedule `clear` to execute</span></div><div class='line' id='LC178'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// after `delay` ms.</span></div><div class='line' id='LC179'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC180'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// If `debounce_mode` is false (at end), schedule `callback` to</span></div><div class='line' id='LC181'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// execute after `delay` ms.</span></div><div class='line' id='LC182'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="nx">timeout_id</span> <span class="o">=</span> <span class="nx">setTimeout</span><span class="p">(</span> <span class="nx">debounce_mode</span> <span class="o">?</span> <span class="nx">clear</span> <span class="o">:</span> <span class="nx">exec</span><span class="p">,</span> <span class="nx">debounce_mode</span> <span class="o">===</span> <span class="kc">undefined</span> <span class="o">?</span> <span class="nx">delay</span> <span class="o">-</span> <span class="nx">elapsed</span> <span class="o">:</span> <span class="nx">delay</span> <span class="p">);</span></div><div class='line' id='LC183'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="p">}</span></div><div class='line' id='LC184'>&nbsp;&nbsp;&nbsp;&nbsp;<span class="p">};</span></div><div class='line' id='LC185'>&nbsp;&nbsp;&nbsp;&nbsp;</div><div class='line' id='LC186'>&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// Set the guid of `wrapper` function to the same of original callback, so</span></div><div class='line' id='LC187'>&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// it can be removed in jQuery 1.4+ .unbind or .die by using the original</span></div><div class='line' id='LC188'>&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// callback as a reference.</span></div><div class='line' id='LC189'>&nbsp;&nbsp;&nbsp;&nbsp;<span class="k">if</span> <span class="p">(</span> <span class="nx">$</span><span class="p">.</span><span class="nx">guid</span> <span class="p">)</span> <span class="p">{</span></div><div class='line' id='LC190'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="nx">wrapper</span><span class="p">.</span><span class="nx">guid</span> <span class="o">=</span> <span class="nx">callback</span><span class="p">.</span><span class="nx">guid</span> <span class="o">=</span> <span class="nx">callback</span><span class="p">.</span><span class="nx">guid</span> <span class="o">||</span> <span class="nx">$</span><span class="p">.</span><span class="nx">guid</span><span class="o">++</span><span class="p">;</span></div><div class='line' id='LC191'>&nbsp;&nbsp;&nbsp;&nbsp;<span class="p">}</span></div><div class='line' id='LC192'>&nbsp;&nbsp;&nbsp;&nbsp;</div><div class='line' id='LC193'>&nbsp;&nbsp;&nbsp;&nbsp;<span class="c1">// Return the wrapper function.</span></div><div class='line' id='LC194'>&nbsp;&nbsp;&nbsp;&nbsp;<span class="k">return</span> <span class="nx">wrapper</span><span class="p">;</span></div><div class='line' id='LC195'>&nbsp;&nbsp;<span class="p">};</span></div><div class='line' id='LC196'>&nbsp;&nbsp;</div><div class='line' id='LC197'>&nbsp;&nbsp;<span class="c1">// Method: jQuery.debounce</span></div><div class='line' id='LC198'>&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC199'>&nbsp;&nbsp;<span class="c1">// Debounce execution of a function. Debouncing, unlike throttling,</span></div><div class='line' id='LC200'>&nbsp;&nbsp;<span class="c1">// guarantees that a function is only executed a single time, either at the</span></div><div class='line' id='LC201'>&nbsp;&nbsp;<span class="c1">// very beginning of a series of calls, or at the very end. If you want to</span></div><div class='line' id='LC202'>&nbsp;&nbsp;<span class="c1">// simply rate-limit execution of a function, see the &lt;jQuery.throttle&gt;</span></div><div class='line' id='LC203'>&nbsp;&nbsp;<span class="c1">// method.</span></div><div class='line' id='LC204'>&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC205'>&nbsp;&nbsp;<span class="c1">// In this visualization, | is a debounced-function call and X is the actual</span></div><div class='line' id='LC206'>&nbsp;&nbsp;<span class="c1">// callback execution:</span></div><div class='line' id='LC207'>&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC208'>&nbsp;&nbsp;<span class="c1">// &gt; Debounced with `at_begin` specified as false or unspecified:</span></div><div class='line' id='LC209'>&nbsp;&nbsp;<span class="c1">// &gt; ||||||||||||||||||||||||| (pause) |||||||||||||||||||||||||</span></div><div class='line' id='LC210'>&nbsp;&nbsp;<span class="c1">// &gt;                          X                                 X</span></div><div class='line' id='LC211'>&nbsp;&nbsp;<span class="c1">// &gt; </span></div><div class='line' id='LC212'>&nbsp;&nbsp;<span class="c1">// &gt; Debounced with `at_begin` specified as true:</span></div><div class='line' id='LC213'>&nbsp;&nbsp;<span class="c1">// &gt; ||||||||||||||||||||||||| (pause) |||||||||||||||||||||||||</span></div><div class='line' id='LC214'>&nbsp;&nbsp;<span class="c1">// &gt; X                                 X</span></div><div class='line' id='LC215'>&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC216'>&nbsp;&nbsp;<span class="c1">// Usage:</span></div><div class='line' id='LC217'>&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC218'>&nbsp;&nbsp;<span class="c1">// &gt; var debounced = jQuery.debounce( delay, [ at_begin, ] callback );</span></div><div class='line' id='LC219'>&nbsp;&nbsp;<span class="c1">// &gt; </span></div><div class='line' id='LC220'>&nbsp;&nbsp;<span class="c1">// &gt; jQuery(&#39;selector&#39;).bind( &#39;someevent&#39;, debounced );</span></div><div class='line' id='LC221'>&nbsp;&nbsp;<span class="c1">// &gt; jQuery(&#39;selector&#39;).unbind( &#39;someevent&#39;, debounced );</span></div><div class='line' id='LC222'>&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC223'>&nbsp;&nbsp;<span class="c1">// This also works in jQuery 1.4+:</span></div><div class='line' id='LC224'>&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC225'>&nbsp;&nbsp;<span class="c1">// &gt; jQuery(&#39;selector&#39;).bind( &#39;someevent&#39;, jQuery.debounce( delay, [ at_begin, ] callback ) );</span></div><div class='line' id='LC226'>&nbsp;&nbsp;<span class="c1">// &gt; jQuery(&#39;selector&#39;).unbind( &#39;someevent&#39;, callback );</span></div><div class='line' id='LC227'>&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC228'>&nbsp;&nbsp;<span class="c1">// Arguments:</span></div><div class='line' id='LC229'>&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC230'>&nbsp;&nbsp;<span class="c1">//  delay - (Number) A zero-or-greater delay in milliseconds. For event</span></div><div class='line' id='LC231'>&nbsp;&nbsp;<span class="c1">//    callbacks, values around 100 or 250 (or even higher) are most useful.</span></div><div class='line' id='LC232'>&nbsp;&nbsp;<span class="c1">//  at_begin - (Boolean) Optional, defaults to false. If at_begin is false or</span></div><div class='line' id='LC233'>&nbsp;&nbsp;<span class="c1">//    unspecified, callback will only be executed `delay` milliseconds after</span></div><div class='line' id='LC234'>&nbsp;&nbsp;<span class="c1">//    the last debounced-function call. If at_begin is true, callback will be</span></div><div class='line' id='LC235'>&nbsp;&nbsp;<span class="c1">//    executed only at the first debounced-function call. (After the</span></div><div class='line' id='LC236'>&nbsp;&nbsp;<span class="c1">//    throttled-function has not been called for `delay` milliseconds, the</span></div><div class='line' id='LC237'>&nbsp;&nbsp;<span class="c1">//    internal counter is reset)</span></div><div class='line' id='LC238'>&nbsp;&nbsp;<span class="c1">//  callback - (Function) A function to be executed after delay milliseconds.</span></div><div class='line' id='LC239'>&nbsp;&nbsp;<span class="c1">//    The `this` context and all arguments are passed through, as-is, to</span></div><div class='line' id='LC240'>&nbsp;&nbsp;<span class="c1">//    `callback` when the debounced-function is executed.</span></div><div class='line' id='LC241'>&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC242'>&nbsp;&nbsp;<span class="c1">// Returns:</span></div><div class='line' id='LC243'>&nbsp;&nbsp;<span class="c1">// </span></div><div class='line' id='LC244'>&nbsp;&nbsp;<span class="c1">//  (Function) A new, debounced, function.</span></div><div class='line' id='LC245'>&nbsp;&nbsp;</div><div class='line' id='LC246'>&nbsp;&nbsp;<span class="nx">$</span><span class="p">.</span><span class="nx">debounce</span> <span class="o">=</span> <span class="kd">function</span><span class="p">(</span> <span class="nx">delay</span><span class="p">,</span> <span class="nx">at_begin</span><span class="p">,</span> <span class="nx">callback</span> <span class="p">)</span> <span class="p">{</span></div><div class='line' id='LC247'>&nbsp;&nbsp;&nbsp;&nbsp;<span class="k">return</span> <span class="nx">callback</span> <span class="o">===</span> <span class="kc">undefined</span></div><div class='line' id='LC248'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="o">?</span> <span class="nx">jq_throttle</span><span class="p">(</span> <span class="nx">delay</span><span class="p">,</span> <span class="nx">at_begin</span><span class="p">,</span> <span class="kc">false</span> <span class="p">)</span></div><div class='line' id='LC249'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="o">:</span> <span class="nx">jq_throttle</span><span class="p">(</span> <span class="nx">delay</span><span class="p">,</span> <span class="nx">callback</span><span class="p">,</span> <span class="nx">at_begin</span> <span class="o">!==</span> <span class="kc">false</span> <span class="p">);</span></div><div class='line' id='LC250'>&nbsp;&nbsp;<span class="p">};</span></div><div class='line' id='LC251'>&nbsp;&nbsp;</div><div class='line' id='LC252'><span class="p">})(</span><span class="k">this</span><span class="p">);</span></div></pre></div>
            </td>
          </tr>
        </table>
  </div>

  </div>
</div>

<a href="#jump-to-line" rel="facebox[.linejump]" data-hotkey="l" class="js-jump-to-line" style="display:none">Jump to Line</a>
<div id="jump-to-line" style="display:none">
  <form accept-charset="UTF-8" class="js-jump-to-line-form">
    <input class="linejump-input js-jump-to-line-field" type="text" placeholder="Jump to line&hellip;" autofocus>
    <button type="submit" class="button">Go</button>
  </form>
</div>

        </div>

      </div><!-- /.repo-container -->
      <div class="modal-backdrop"></div>
    </div><!-- /.container -->
  </div><!-- /.site -->


    </div><!-- /.wrapper -->

      <div class="container">
  <div class="site-footer">
    <ul class="site-footer-links right">
      <li><a href="https://status.github.com/">Status</a></li>
      <li><a href="http://developer.github.com">API</a></li>
      <li><a href="http://training.github.com">Training</a></li>
      <li><a href="http://shop.github.com">Shop</a></li>
      <li><a href="/blog">Blog</a></li>
      <li><a href="/about">About</a></li>

    </ul>

    <a href="/">
      <span class="mega-octicon octicon-mark-github"></span>
    </a>

    <ul class="site-footer-links">
      <li>&copy; 2013 <span title="0.05745s from github-fe133-cp1-prd.iad.github.net">GitHub</span>, Inc.</li>
        <li><a href="/site/terms">Terms</a></li>
        <li><a href="/site/privacy">Privacy</a></li>
        <li><a href="/security">Security</a></li>
        <li><a href="/contact">Contact</a></li>
    </ul>
  </div><!-- /.site-footer -->
</div><!-- /.container -->


    <div class="fullscreen-overlay js-fullscreen-overlay" id="fullscreen_overlay">
  <div class="fullscreen-container js-fullscreen-container">
    <div class="textarea-wrap">
      <textarea name="fullscreen-contents" id="fullscreen-contents" class="js-fullscreen-contents" placeholder="" data-suggester="fullscreen_suggester"></textarea>
          <div class="suggester-container">
              <div class="suggester fullscreen-suggester js-navigation-container" id="fullscreen_suggester"
                 data-url="/cowboy/jquery-throttle-debounce/suggestions/commit">
              </div>
          </div>
    </div>
  </div>
  <div class="fullscreen-sidebar">
    <a href="#" class="exit-fullscreen js-exit-fullscreen tooltipped leftwards" title="Exit Zen Mode">
      <span class="mega-octicon octicon-screen-normal"></span>
    </a>
    <a href="#" class="theme-switcher js-theme-switcher tooltipped leftwards"
      title="Switch themes">
      <span class="octicon octicon-color-mode"></span>
    </a>
  </div>
</div>



    <div id="ajax-error-message" class="flash flash-error">
      <span class="octicon octicon-alert"></span>
      <a href="#" class="octicon octicon-remove-close close ajax-error-dismiss"></a>
      Something went wrong with that request. Please try again.
    </div>

  </body>
</html>

