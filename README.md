<h1>ASP.NET Core Image Serving Middleware 1.0.0</h1>

<p>
  Hello Everyone, in this reposirotry you are looking at a asp.net middleware thats used for manipulating images on the fly like imageflow. you just have to
  download this from here and build for your self or you can just download this to your project using nuget (chamod.ImageServer).
</p>

<h2>Installation</h2>

<ol>
  <li>Install From Nuget or Download and build.</li>
  <li>Reference the dll if you build from source.</li>
  <li>After Static File Middleware Add line =><b>app.UseImageServerMiddleware(Directory.GetCurrentDirectory(),30);</b></li>
  <li>Change MaxCacheTime by changing 30 to whatever you want.</li>
</ol>

<h2>Usage</h2>
<ul>
  <li>
    You can change image size by passing "h" & "w" by as query parameters. eg: (/images/pic1.jpg?w=100)
  </li>
  <br>
  <li>
    you can set quality while operations by passing "q" as query parameters (0-100). eg: (/images/pic1.jpg?w=100&q=90)
  </li>
  <br>
  <li>
    you can set rotation degrees (clockwise) by providing "r" as query parameters (0-360). eg: (/images/pic1.jpg?w=100&q=90&r=60)
  </li>
  <br>
</ul>
<h3>Notes</h3>
<p>Only Following Image Types are Supported as inputs for now.</p>
<ul>
  <li>png</li>
  <li>jpg</li>
  <li>jpeg</li>
  <li>gif</li>
  <li>tiff</li>
  <li>ico</li>
  <li>bmp</li>
</ul>
<p>It will be always <b>WebP</b> when output for now.</p>

<b>Thankyou Happy Coding.!</b>






