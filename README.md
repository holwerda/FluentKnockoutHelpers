#Fluent Knockout Helpers Overview
##with Survey App Demo SPA using Durandal.js at _/demo/SurveyApp.sln_

Knockout.js allows for a very powerful MVVM style binding that makes it very easy to create rich web applications very quickly and with little code. One downside coming from the world of .NET and strong typing is that Knockout bindings use lots of "magic strings". This is where FluentKnockoutHelpers comes in as an attempt to bring all the goodness of ASP.NET MVC-like property expressions to Knockout with a fluent twist.

Instead of resorting to magic strings, no compile-time support and little intellisense when working with Knockout you can use FluentKnockoutHelpers to create property binding expressions just like how MVC does it for all the .NET defined types you are already using on the server to interact with your Knockout web client.

Durandal.js is a new framework that uses Knockout, jQuery, Require and Sammy that makes it very easy to create single page applications. It 'just works' with Knockout and feels very clean so I have created the Survey App using it. Also take note other than for the start page and WebAPI **MVC isn't used at all for views**. This seems pretty natural so far but time will tell as it is built out.

Take a look at Survey App demo to see a full demo using Durandal.js

#Quick Example
Here is a quick example of what FluentKnockoutHelpers allows for using a classic Knocokout hello world example and an API endpoint to serve up the data (use of WebAPI isn't by any means required)

##Person class defined in C# code
```c#
public class Person
{
  public string FirstName { get; set; }
  public string LastName { get; set; }
}
```

##API controller serving up a Person
```c#
public class PersonController : ApiController
{
  // GET api/person/5
  public Person Get(int id)
  {
    return _repo.People.First(x => x.PersonId == id)
  }
}
```

##Knockout JavaScript ViewModel
```javascript
var PersonViewModel = function(personId) {
	var self = this;

	self.person = ko.observable();

	//ctor
	$.getJSON('person/' + personId)
		.then(function(person) {
		
			//the ko.mapping plugin below converts the entire JSON
			//object graph into observables
			self.person(ko.mapping.fromJS(person));
		});
};

ko.applyBindings(new PersonViewModel(1));
```

##CSHTML Helper Creation
###Create a Strongly-Typed Helper for an Endpoint (Option A)
Here we create a Knockout helper bound the the WebAPI endpoint that we know the model is coming from. (Don't worry MVC controllers work fine too.) The expression isn't actually executed it just allows us to having strong typing. We must inform the FluentKnockoutHelpers what JavaScript ViewModel field our _person_ is stored in, 'true' indicates it is observable. The 'this' below is an extension method off of WebPageBase meaning it works with MVC Razor views and "normal" Razor views alike.
```html
<!-- The top of our CSHTML file -->
@{
  var person = this.KnockoutHelperForApi<PersonController>().Endpoint(api => api.Get(default(int)), "person", true);
}
```

###Create a Strongly-Typed Helper for a Type (Option B)
Well maybe that felt a little weird or you aren't using WebAPI or MVC. No problemo, here is another approach that yields the same result. We still must inform the FluentKnockoutHelpers what JavaScript ViewModel field our _person_ is stored in, 'true' indicates it is observable. The 'this' below is an extension method off of WebPageBase meaning it works with MVC Razor views and "normal" Razor views alike.
```html
<!-- The top of our CSHTML file -->
@{
  var person = this.KnockoutHelperForType<Person>("person", true);
}
```

##Putting it all together
Here is the bread and butter of FluentKnockoutHelpers. Since we have created a strongly-typed helper we now can use it to create strongly-typed expressions that at run time yield plain old Knockout code sans the magic strings. Since everyone likes to do their HTML in many different ways FluentKnockoutHelpers is flexible and extensible to accomodate many different ways of accomplishing the same markup result.
###The markup we want to generate
```html
<p>
	First name: 
	<input type="text" class="fancy" data-bind="value: person().FirstName" />
</p>
<p>
	Last name:
	<input type="text" class="fancy" data-bind="value: person().LastName" />
</p>
<h2>
	Hello,
	<!-- ko text: person().FirstName --><!-- /ko -->
	<!-- ko text: person().LastName --><!-- /ko -->
</h2>
```

###Minimalistic markup generation via FluentKnockoutHelpers (Option A)
Minimilastic markup for those that like almost vanilla HTML and/or lots of typing
```html
<!-- Creation of the strongly-typed helper -->
@{
  var person = this.KnockoutHelperForApi<PersonController>().Endpoint(api => api.Get(default(int)), "person", true);
}

<p>
	First name:
	<input type="text" class="fancy" @person.DataBind(db => db.Value(p => p.FirstName)) />
</p>
<p>
	Last name:
	<input type="text" class="fancy" @person.DataBind(db => db.Value(p => p.LastName))  /></p>
<h2>
	Hello,
	<!-- ko text: @person.EvalObservableFor(x => x.FirstName) --><!-- /ko -->
	<!-- ko text: @person.EvalObservableFor(x => x.LastName) --><!-- /ko -->
</h2>
```

###All out C# via FluentKnockoutHelpers (Option B)
We all love MVC (right?) lets pay it some hommage with a fluent twist! Note the DisplayNameFor(..) pulls from the [Display(..)] attribute. Don't worry there is a LabelFor(...) too..
```html
<!-- Creation of the strongly-typed helper -->
@{
  var person = this.KnockoutHelperForApi<PersonController>().Endpoint(api => api.Get(default(int)), "person", true);
}

<p>
	@person.DisplayNameFor(x => x.FirstName)
	@person.BoundTextBoxFor(x => x.FirstName).Class("fancy")
</p>
<p>
	@person.DisplayNameFor(x => x.LastName)
	@person.BoundTextBoxFor(x => x.LastName).Class("fancy")
</p>
<h2>
	Hello,
	@person.BoundTextFor(x => x.FirstName)
	@person.BoundTextFor(x => x.LastName)
</h2>
```

####Take a look at the demo app at _/demo/SurveyApp.sln_ for an entire non-trival demo application that will cover almost all available Knockout bindings
