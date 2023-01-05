# Templar
Templar is an Azure Functions library that uses templating to render static web pages and serves them to your users.

## How to use
Clone this repo and add it to your Azure Functions solution.

Add templar to your Startup.cs
```csharp
builder.Services.AddTemplar(o =>
{
    /*Add components and middleware here*/
});
```
Create your `index.html` and put it in a `wwwroot` folder
```html
<html>
<body>
    <div>
        {Component:AppBody}
    </div>
</body>
</html>
```
Create `IndexPage.cs`
```csharp
internal class IndexPage : TemplarComponent
{
    public override string TemplatePath => "templates/index-body.html";
}
```
And its relevent document `templates/index-body.html`
```html
<p>Here is the body of your index page.</p>
```
Components are defined like `{Component:Name}` where Name is the class name of the component with `Component` trimmed off the end of the name. 
**The component `AppBody` in the example above is a special single use entry point required for building your page.**

Create your functions that point to your pages:
```csharp
public class Function1
{
    private readonly ITemplarService _templarService;
    public Function1(ITemplarService templarService)
    {
        _templarService = templarService;
    }

    [FunctionName("StaticRoot")]
    public async Task<IActionResult> StaticRoot(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{file?}")]
        HttpRequest req)
    {
        return await _templarService.Invoke(req, new IndexPage());
    }
}
```

You can add the following to your `host.json` configuration to disable the `/api` route prefix on your functions:
```json
{
  "extensions": {
    "http": {
      "routePrefix": ""
    }
  }
}
```

Here is how your solution directory should look with the above examples
```
Solution
    Components
        -IndexPage.cs
    -Function1.cs
    wwwroot
        templates
            -index-body.html
        -index.html
```

All the items in your `wwwroot` folder need to be set to `Content` and `Copy if Newer` or you can change your `.csproj` to do that 
automatically:
```xml
<ItemGroup>
    <None Remove="wwwroot\**" />
</ItemGroup>
<ItemGroup>
    <Content Include="wwwroot\**">
        <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </Content>
</ItemGroup>
```

Examine the `Templar.Example` project to see how this is implemented fully.

## Components
Components inherit `TemplarComponent` and have a matching document to render which you link with the abstract property `TemplatePath`. 
Pages are also components, but are called directly from your api and are injected into `{Component:AppBody}`. 
You can implement a component by adding `{Component:ComponentName}`. Components must be added to the DI chain with the 
startup extension:
```csharp
builder.Services.AddTemplar(o => 
{
    o.AddComponent<ComponentName>();
});
```
Components are not activated as part of the DI container so they must have an empty constructor. To see how to inject services 
into your components, see the *Service Injection* section below.

You can skip linking a document to render(ideal for small components) by overriding the method `GetFile(IStaticSiteService)` and 
returning the html of the component like so:
```csharp
protected override string GetFile(IStaticSiteService siteService)
{
    return @"
        <span class=""hint"">@Date</span>
        <div class=""sub-title-bar"">
            <span class=""hint"">@LinkedCategories</span>
            <span class=""hint right"">@Author</span>
        </div>";
}
```

Components don't have to be html documents, for instance you could make a css component to inline minified css to your html. 

## Property Binding
Property binding is automatic. All properties in your component class are matched to their equivalent in the document prefixed with a `@` 
So the property `public string Data { get; set; }` would be matched in a document to `@Data`. Any property(public or non-public) can 
be bound, but not fields or methods.

## Parameters
Parameters can be defined with the `[Parameter]` attribute. Parameters on the `{Component:AppBody}`(The current Page component) 
will capture any `GET` query parameters from the incoming url. 
Parameters are consumed as they're matched, unless you use the attribute `[CascadingParameter]` to which will 
continue to pass the parameter until it is consumed by a `[Parameter]` attribute.

To pass parameters to components, structure your component with the parameters inside the brackets like so:
```
{Component:Test p1="Hello" p2=", " p3="World"}
```
Parameters are separated by spaces, parameter names must be lowercase and they must have their values surrounded by quotes, 
even if you're using a binding.

## Service Injection
You can inject services into your components with the `[Inject]` tag.

## Custom Middleware
You can define custom middleware to customize the render chain. To create custom middleware implement `ITemplarMiddleware` and 
do your logic. Once your logic is complete, **You must call** `await next.Next(req, page)` to continue down the chain. 
Your middleware needs to be defined in the DI chain:
```csharp
builder.Services.AddTemplar(o => 
{
    o.AddMiddleware<CustomMiddleware>();
});
```
Custom middleware has a Transient scope.

## Component Lifecycle
Each individual component follows the following lifecycle
* Resolve dependencies with the `[Inject]` attribute
* `SetParameters(IParameters)` which sets the properties with the `[Parameter]` attributes, followed by the `[CascadingParameter]` attribute
* `OnInitializedAsync()` which serves only as an initialization event for user code
* `GetFile(ISiteService)` which retrieves the document defined at `TemplatePath`
* Maps any bindings in the document
* Recursively builds each subsequent component