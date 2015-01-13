# KY.Controls
Web controls for easy forms development with validation support.
## Features
- Creates nested form fields with required, regex and compare validators
- Two-way object binding, for populating form values and updating source object on submit.
- Generates xml representation of form data

### Version
1.0.3

### Examples
```aspnet
<ky:FieldSet ID="fs1" runat="server" Title="My Form">
    <ky:FieldSetRow Label="Name" CssClass="row">
        <ky:FieldSetItem Name="FirstName" Type="TextBox" Required="true" />
        <ky:FieldSetItem Name="LastName" Type="TextBox" Required="true" />
    </ky:FieldSetRow>
    <ky:FieldSetRow Label="Email" CssClass="row">
        <ky:FieldSetItem Name="Email" Type="TextBox" RegexName="Email" Required="true" />
    </ky:FieldSetRow>
    <ky:FieldSetRow CssClass="row footer">
        <ky:FieldSetItem Type="Button" Text="Send" />
    </ky:FieldSetRow>
</ky:FieldSet>
```
translates to this:
```html
<div id="fs1" class="fieldset">
    <div class="title">
        <span>My Form</span>
    </div>
    <div class="body">
        <div class="row">
            <label for="fs1_FirstName">Name</label>
            <span class="validators"><span id="fs1_rfvFirstName" title="This field is required." style="display:none;">*</span>
            <span id="fs1_rfvLastName" title="This field is required." style="display:none;">*</span>
            </span>
            <span class="controls">
                <input name="fs1$FirstName" type="text" id="fs1_FirstName" />
                <input name="fs1$LastName" type="text" id="fs1_LastName" />
            </span>
        </div>
        <div class="row">
            <label for="fs1_Email">Email</label>
            <span class="validators">
                <span id="fs1_rfvEmail" title="This field is required." style="display:none;">*</span>
                <span id="fs1_revEmail" title="Invalid entry." style="display:none;">!</span>
            </span>
            <span class="controls">
                <input name="fs1$Email" type="text" id="fs1_Email" />
            </span>
        </div>
        <div class="row footer">
            <span class="controls">
                <input type="submit" name="fs1$ctl13" value="Send" onclick="javascript:WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(&quot;fs1$ctl13&quot;, &quot;&quot;, true, &quot;&quot;, &quot;&quot;, false, false))" />
            </span>
        </div>
    </div>
</div>
```
## Accessing fields
```c#
var formValue = fs1["Name"].Value;
```
## Populating form values based on a bound object
```c#
fs1.Bind(myObject);
```
## Updating an object
```c#
fs1.UpdateObject(myObject);
```
## Example
```c#
public partial class Default : System.Web.UI.Page
{
    MyClass myObj;

    protected void Page_Init(object sender, EventArgs e)
    {
        myObj = new MyClass()
        {
            FirstName = "Foo",
            LastName = "Bar",
            Email = "foo@localhost"
        };
        
        // populate initial field values
        fs1.Bind(myObj);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Page.IsPostBack)
        {
            // update myObj based on posted values
            fs1.UpdateObject(myObj);
        }
    }
}
```

## Installation
To install KY.Controls, run the following command in the Package Manager Console
```
PM> Install-Package KY.Controls
```
