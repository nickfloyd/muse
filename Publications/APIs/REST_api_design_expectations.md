## Core API design expectations

* APIs **MUST** **BE** protocol-based / consistent.   
* The API surface **MUST** **BE** highly discoverable.  
* Multi-tenants and cross-team federation **MUST** **BE** taken into account where authorization is concerned.  
* APIs **SHOULD** **BE** easy to use and hard to break.  
* The API surface **SHOULD BE** easy to build and maintain  
* Engineering efforts **SHOULD** grow with the organization and consumer models  
* The APIs **SHOULD** address real needs from a real consumer base  
* Examples and documentation **SHOULD BE** exceptional aspects of the API  
  * see OOPSLA ‘06  
  * see D.L. Parnas, Software Aging  
* I **SHOULD** align choices with realistic expectations \- “Aim to displease everyone equally”, I’m not going to make everyone happy.  
* I **SHOULD** expect mistakes and missteps but learn and recover  
* I **MUST** keep implementation details out of the surface of the API no leaks  
* The API surface **SHOULD BE** symmetrical  
* I MUST be mindful of data hierarchy \- subclass if it makes sense (watch out for the tendency to model API resources after the actual data model.  
  * See: Liskov Substitution Principle  
* APIs **SHOULD** fail fast  
* A Public API does \!= just enable integrations  
* Parameter order **SHOULD BE** consistent  
* APIs **SHOULD BE** opinionated

## Core design Philosophies

* Individual APIs should do one thing well  
* Clear names and realms  
* Interfaces should communicate intent  
* When the design is unclear for a given need, favor an opinionated approach that is consistent with the other aspects of the API.  
* When doubting the value or clarity of something \- simply leave it out until the value of it realizes itself. Don’t jam in APIs because they “could be used.”  
* Conceptual weight is more important than the bulk of the actual surface. How much cognitive capital is needed to conceptualize the API?  
* Minimize the accessibility of everything \- only expose the schema-based model, for instance  
* Consider performance always \- but don’t prematurely optimize   
* The APIs need to coexist with the Platform \- approaches and models and even software behaviors that appear in the model should be understood \- avoid transliteration for the sake of matching software surface.  
* Don’t make the consumers do anything that the API implementation could do \- this introduces the principle of API misuse and serendipitous consumer implementations that could lead to creating fragile consumer applications.  
* Avoid violation of the [Principle of least Astonishment](https://en.wikipedia.org/wiki/Principle_of_least_astonishment) \- consumers of the API should not be surprised by an implementation or API behavior  
* Open API definitions should be comprehensive and valid  
* Open API definitions should be continually linted