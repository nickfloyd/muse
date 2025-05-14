# Why is this important?

This API thing is nothing new. 

We can all agree that it’s more than just an image thing, it’s also **a competitive advantage** \- have a look at the brief competitive analysis below on what our competitors are currently offering for a glimpse into this.  Keep in mind though InVision is really the only design ecosystem (Adobe comes close just because of its sheer size) currently so even the API offerings detailed below wouldn’t be able to match an API surface like the one we are trying to conceptualize here.

APIs can **promote growth** in terms of revenue, growth in our user base, and growth in our software maturity. Public APIs, if done well, can drive adoption from companies just because the APIs are part of the offering \- even if the company never intends on using them. 

They can encourage the growth of the entire platform by utilizing a community of rich engineering knowledge and experience.  These individuals can extend our platform and do things that we either never thought to do or build useful tools that we’d never plan on building.

Additionally having a set of public APIs can **force great design practices and unified engineering**.  Building out a consistent API surface for consumers to use can be an accelerant to drive change in our internal APIs.

Public APIs can be one of our company’s greatest asset if done well.    
---

# What are some core design north stars?

* APIs **MUST** **BE** protocol-based / consistent   
* The API surface **MUST** **BE** highly discoverable  
* Multi tenants and cross-team federation **MUST** **BE** taken into account where authorization is concerned  
* APIs **SHOULD** **BE** easy to use and hard to break  
* The API surface **SHOULD BE** easy to build and maintain  
* Engineering efforts **SHOULD** grow with the organization and consumer models  
* The APIs **SHOULD** address real needs from a real consumer base  
* Examples and documentation **SHOULD BE** exceptional aspects of the API  
  * see OOPSLA ‘06  
  * see D.L. Parnas, Software Aging  
* We **SHOULD** align choices with realistic expectations \- “Aim to displease everyone equally”, we’re not going to make everyone happy.  
* We **SHOULD** expect mistakes and missteps but learn and recover  
* We **MUST** keep implementation details out of the surface of the API no leaks  
* The API surface **SHOULD BE** symmetrical  
* We MUST be mindful of data hierarchy \- subclass if it makes sense (watch out for the tendency to model API resources after the actual data model  
  * See: Liskov Substitution Principle  
* APIs **SHOULD** fail fast  
* A Public API does not \== just enabling integrations  
* Parameter order **SHOULD BE** consistent  
* APIs **SHOULD BE** opinionated

# What are some core design Philosophies?

* Individual APIs should do one thing well  
* Clear names and realms  
* Interfaces should communicate intent  
* When the design is not clear for a given need favor an opinionated approach that is consistent with the other aspects of the API.  
* When doubting the value or clarity of something \- simply leave it out until the value of it realizes itself. Don’t jam in APIs because they “could be used”.  
* Conceptual weight is more important than the bulk of the actual surface. How much cognitive capital is needed to conceptualize the API?  
* Minimize the accessibility of everything \- only exposed the schema-based model for instance  
* Consider performance always \- but don’t prematurely optimize   
* The APIs need to coexist with the InVison Platform \- InVision approaches and models and even software behaviors that appear in the model should be understood \- we shouldn’t all behaviors that our Platform does not allow \- but we should avoid transliteration for the sake of matching software surface.  
* Don’t make the consumers do anything that the API implementation could do \- this introduces the principle of API misuse and serendipitous consumer implementations that could lead to creating fragile consumer applications.  
* Avoid violation of the [Principle of least Astonishment](https://en.wikipedia.org/wiki/Principle_of_least_astonishment) \- consumers of the API should not be surprised by an implementation or API behavior

---

# Steps to begin designing what we need

Here it is \- time to strike and I am certain that there is a tendency that is boiling up in everyone to go to the Boss fight before we play the level. As with every huge initiative that has the potential to be boundless we have to start with simple, easy to composite parts that are driven by a rallying call pushed through the filter of our chosen philosophies and north stars (see above).  **Favor iteration over big plans. If something is stupid we stop doing it.  If something works well we enhance and improve that.** 

If our plan is to build a reality-based API surface then we have to be realistic in our approach.  We need to honor the effort that needs to go into this by avoiding statements like “we are building a public API surface” and favor saying things such as “We are looking at our needs internally and from customers to determine what APIs are needed to support sharing Freehand documents via slack.” **The idea here is that speaking in specifics will root us in a semantic mindset to build to a target and not to a shifting plan or emotion.**

* Determine stakeholders  
* Gather requirements with skepticism as a filter  
  * The goal is to find   
    * true needs  
    * reality-based cases  
    * The delta between what is stated as a need verse what is intended as a need  
* Determine data/business logic involved \- what models should we expose based on reality-based user stories  
* Intend on building out a short spec  
  * “agility trumps completeness”  
  * Helps avoid ego investment by overwork on something that is not useful  
  * Possibly on one page  
  * Pseudocode against the API that does not exist  
* Write API against the API implementations in a tight loop \- write early/often, prove against use case via unit tests, iterate (?)  
* Build a Ubiquitous Language for InVision Public APIs \- one consistent language that can easily be understood and consumed

# Use case / User Story Considerations

* Use cases / User stories should be reality-based from real sources (if possible)  
* We currently have some products where implementations have taken place \- Slate: Slack integration, Studio \> Freehand integration, Microsoft teams, so on  \- these might be good places to gather user information.  
* Consider SPIs (Service Provider Interfaces) \- can 3rd parties implement custom authentication as a sidecar to the API for instance  
* Consider the value of exposing log, metrics, telemetry data to consumers  
* Consider 1st, 2nd, 3rd Party Authentication Extensions (See OAuth Extension spec)

---

# Brief Competitive Analysis 

**Figma**: Uses a web-based approach (their tool is web-based) supporting OAuth and REST-like endpoints that return json. [Docs](https://www.figma.com/developers/docs?utm_source=figma_blog&utm_medium=link_in_post&utm_campaign=platform_launch) | [Platform API write up](https://blog.figma.com/introducing-figmas-platform-ee681bf861e7).  Simplified to support 3 core capabilities: read (only) files, read/write files, and render files. They can “get away” with a decoupled approach like this because their files and application are stored in and execute from the cloud. Consumers simply need to understand HTTP, related behaviors, and how to parse json. The API has two concepts in place: 1\. Document reading and 2\. Rendering the document.

//token

\[GET\] [https://api.figma.com/v1/files/:key](https://api.figma.com/v1/files/:key)

**Sketch**: Closely related to our execution model (installed software) and local files.  They provide a document-based API that is RPC-like. The APIs are terse and named accordingly (document.getLayerWithID, document.getSymbols(), etc…).  These APIs closely relate to the way our project-actions are \- workflow and functional-based rather than resource-based. The API comes bundled inside of Sketch so it can be referenced var sketch \= require('sketch/dom').  Consumers have to understand and interpret the object model. The API has two concepts in place: 1\. Document interactions and 2\. Tooling interactions. [Docs](https://developer.sketchapp.com/reference/api/)

var document \= require('sketch/dom').getSelectedDocument()

// or

var document \= Document.getSelectedDocument()

**Adobe photoshop**: Old [API](https://www.adobe.io/apis/creativecloud/photoshop.html) is SDK based, C++ modules. Creative Cloud apps can be extended using their Common Extensibility Platform([CEP](https://console.adobe.io/downloads/cep), [samples](https://github.com/Adobe-CEP)) with HTML, CSS, and JavaScript.  They use a manifest to aid in the loading of the plugin ([ex](https://github.com/Adobe-CEP/Getting-Started-guides/blob/master/Network%20requests%20and%20responses%20with%20Fetch/com.cep.weather-simple/CSXS/manifest.xml)), document-based API (app.documents\[0\].artLayers\[0\]), and instantiation of an interface to handle interactions (const csInterface \= new CSInterface();) .  There JavaScript-based API, [CSInterface.js](https://github.com/Adobe-CEP/CEP-Resources/blob/master/CEP_8.x/CSInterface.js) is downloaded and added to the extension.

//js 

const csInterface \= new CSInterface();

csInterface.evalScript(\`doThing("${dataset.xyx}", "${dataset.pdq}")\`);

//jsx

app.activeDocument.saveAs(File(sanitizedFilePath), pdfSaveOptions);

---

# Technology / Frameworks / Protocols / Specification

While this is not an important consideration at this point in the design and projection of what we intend on doing it’s worth calling out some notes on what is available and how it could fit our use cases.

### [REST](https://en.wikipedia.org/wiki/Representational_state_transfer) vs [GraphQL](https://graphql.github.io/graphql-spec/June2018/) vs [gRPC](https://grpc.io/)

Let me begin by stating that all of these approaches, frameworks, transports, runtimes (call them what you will) can all be complementary and can help us iterate toward an ideal API surface. Additionally, it is important to point out as far as web-based APIs go there are alternatives to the three listed above \- were going to hone in on these three because they have broad community, language, and tooling support.

This highlights a very important point:  Currently, the implementation detail of what “framework” to use is not a priority \- what is the priority is determining our use cases, architectural approaches, the fabric of our how we want to offer our data to consumers, what shape could that data be in, how we secure that data, how we filter it, and lastly how will consumers use the data (outside of concrete use cases \- bulk, web-based applications, mobile, extensions, augments to other applications so on. 

The three architectural styles are defined as follows (this is not a comprehensive comparison but rather a set of talking points to help with familiarity) :

#### **REST**

Representational state transfer is a style of constraints and protocols that can be used for creating web-based services / APIs.  It was introduced by Roy Fielding in 2000 \- chapter 5 of his PhD dissertation on network-based software architecture. It was developed with and makes heavy use of RFC 2616 (the HTTP 1.1 specification).

**Characteristics of REST**:

* REST tends to be chatty \- where multiple calls are needed to fulfill data concerns of a given application  
* No type system (not counting HTTP type augmentation)  
* REST uses a resource-driven discovery system where discovery can be made only through an understanding of the resources. Resources can implement [HATEOAS](https://hub.packtpub.com/developing-rest-based-web-service/) (Hypertext as the Engine of Application State) to give requests proper discoverability attributes.  
* REST is thin client / fat server  
* Requests return the entire resource  
* It harnesses HTTP 1.1 specification which has stood the test of time  
* Heavily protocol-based and if implemented properly can provide a well-known surface for consumers.  
* There are many libraries that support REST

One of the challenging aspects of REST is that it is commonly used for implementations that it wasn’t really built for.  Fielding himself [comments](https://roy.gbiv.com/untangled/2008/rest-apis-must-be-hypertext-driven) that “REST is intended for long-lived network-based applications that span multiple organizations.” This can also be one of the strong points of REST.

For more on real-world design and architecture have a look at one of the [talks](https://speakerdeck.com/player/4fcb99a4c1de41002200f751) I gave on REST. Also here’s a [white paper on HATOAS](https://hub.packtpub.com/developing-rest-based-web-service/) that I wrote a while back.

#### **GraphQL**

GraphQL is simple a query language for your data graph and execution engine originally created at Facebook in 2012 for describing the capabilities and requirements of data models for client‐server applications. The development of this open standard started in 2015\.

**Characteristics of GraphQL:**

* Few round trips for data  
* Has a sophisticated type system  
* Has native discoverability  
* GraphQL can be represented as fat client / fat server or thin client thin server) \- can be ideal for all types of device and client interactions  
* Can aggregate many resources into one single request  
* Requests can be made for only what is needed 

The superpower of GraphQL is that it provides a highly discoverable API that allows consumers to request and get exactly what they want without placing a burden on the server to create many resources and inputs as filters \- It provides a great degree of flexibility.

#### **gRPC**

gRPC is often used to allow consumer applications the ability to directly call APIs on various as if they were methods from objects local to the client application. It relies heavily on remoting and runs an interface implemented on the server to route requests to the appropriate instance.  

Why do I call this out as a framework that might be helpful to accomplishing a public API goal?  Consider our monolith of microservices \- we have many disparate services (BFFs and the like) calling into many different Domain Services that end up providing data to our end-users \- a potential iteration or approach to help unify these disparate systems might be to look at something like gRPC \+ protocol buffers to help us bridge gaps across our services.   
---

# Definitions

##### **Symmetrical APIs** 

This is typically a concept found in REST-based API surfaces and even RPC based ones. I was originally introduced to a concrete definition of this principle in a book called "Practical API Design: Confessions of a Java Architect".

This principle is rooted in pragmatism and predictability. It states that: APIs should be designed in such a way that they provide closed loops on resources and operations such that the consumption of an API can be predictable and assumption-based.

An example of this would be something like: If you have an API that allows you to Create a resource a consumer should be able to reason that there will be a Destroy or Delete a resource. If there is an API that allows the consumer to be able to add things to a collection then it would reason (from a consumer standpoint) that there would be a corresponding API that would allow the removal of items from that collection.

Symmetry, in this case, describes an API surface's tendency to be diametric.

##### **Principle of least astonishment**

The principle means that a component of a system should behave in a way that most users will expect it to behave; the behavior should not astonish or surprise users. ([see ref](https://en.wikipedia.org/wiki/Principle_of_least_astonishment))

##### **Liskov Substitution Principle**

Liskov's notion of a behavioral subtype defines a notion of substitutability for objects; that is, if S is a subtype of T, then objects of type T in a program may be replaced with objects of type S without altering any of the desirable properties of that program (e.g. correctness). ([see ref](https://en.wikipedia.org/wiki/Liskov_substitution_principle))

---

# References

Book: GraphQL or Bust 

Book: The Rule of Threes: Confessions of a Used Program Salesman \- details aspects of software reuse

Book: GraphQL API Design, Matthias Biehl

Book: Code Complete, Steve McConnell

Book: Working Effectively with Legacy Code, Michael Feathers

Book: Clean Code, Robert C. Martin

Book: What Every Web Developer Should Know About HTTP, K. Scott Allen

Book: [Domain Driven Design](https://www.amazon.com/gp/product/0321125215?ie=UTF8&tag=martinfowlerc-20&linkCode=as2&camp=1789&creative=9325&creativeASIN=0321125215), Eric Evans

Book: Practical API Design: Confessions of a Java Framework Architect, Jaroslav Tulach

Article: Embracing the Differences: Inside the Netflix API Redesign

[https://medium.com/netflix-techblog/embracing-the-differences-inside-the-netflix-api-redesign-15fd8b3dc49d](https://medium.com/netflix-techblog/embracing-the-differences-inside-the-netflix-api-redesign-15fd8b3dc49d)

Article: How Facebook organizes their GraphQL code

[https://blog.apollographql.com/graphql-at-facebook-by-dan-schafer-38d65ef075af](https://blog.apollographql.com/graphql-at-facebook-by-dan-schafer-38d65ef075af) 

Article: The Significance of GraphQL

[https://blog.usejournal.com/the-significance-of-graphql-part-1-graphql-vs-rest-5f25c9f34b1e](https://blog.usejournal.com/the-significance-of-graphql-part-1-graphql-vs-rest-5f25c9f34b1e)

[https://blog.usejournal.com/the-significance-of-graphql-part-2-how-facebook-coursera-and-artsy-use-graphql-86abe9ab9cb2](https://blog.usejournal.com/the-significance-of-graphql-part-2-how-facebook-coursera-and-artsy-use-graphql-86abe9ab9cb2)

[https://blog.usejournal.com/the-significance-of-graphql-part-3-how-shopify-teitterand-the-new-york-times-use-graphql-dd99e1fab979](https://blog.usejournal.com/the-significance-of-graphql-part-3-how-shopify-teitterand-the-new-york-times-use-graphql-dd99e1fab979)

Article: GraphQL Case studies

[https://www.graphql.com/case-studies/](https://www.graphql.com/case-studies/)

GraphQL Community resources: [https://graphql.org/community/](https://graphql.org/community/)

Nick Floyd Ed./Author, OAuth 1.0 / 1.0a Extension spec: [https://developer.fellowshipone.com/docs/v1/Util/AuthDocs.help](https://developer.fellowshipone.com/docs/v1/Util/AuthDocs.help)

[https://spec.fellowshiponeapi.com/v1/Content/F1OAuthExtensionv1.html](https://spec.fellowshiponeapi.com/v1/Content/F1OAuthExtensionv1.html)

Darion Welch, Best Practices for using GraphQL at InVision

[https://invisionapp.atlassian.net/wiki/spaces/EDOC/pages/891784646/Best+Practices+for+using+GraphQL+at+InVision](https://invisionapp.atlassian.net/wiki/spaces/EDOC/pages/891784646/Best+Practices+for+using+GraphQL+at+InVision)

OOPSLA (Object-Oriented Programming, Systems, Languages & Applications) ‘06 P. 75 (Proof of seeding through example \[getting them wrong\]. Engineers will emulate documented/examples and patterns good or bad): [http://acme.able.cs.cmu.edu/pubs/uploads/pdf/OOPSLA-06.pdf](http://acme.able.cs.cmu.edu/pubs/uploads/pdf/OOPSLA-06.pdf)

D.L. Parnas, Software Aging [https://www.cs.drexel.edu/\~yfcai/CS451/RequiredReadings/SoftwareAging.pdf](https://www.cs.drexel.edu/~yfcai/CS451/RequiredReadings/SoftwareAging.pdf)

Joshua Bloch: Bumper-Sticker API Design: [https://www.infoq.com/articles/API-Design-Joshua-Bloch/](https://www.infoq.com/articles/API-Design-Joshua-Bloch/)

Nick Floyd, Developing a REST-based Web Service

[https://hub.packtpub.com/developing-rest-based-web-service/](https://hub.packtpub.com/developing-rest-based-web-service/)

Nick Floyd, REST API: Design and Architecture for the REST of US

[https://speakerdeck.com/nickfloyd/for-the-rest-of-us](https://speakerdeck.com/nickfloyd/for-the-rest-of-us)

Nick Floyd, Services: Rules and Guidelines

[https://invisionapp.atlassian.net/wiki/spaces/ARCH/pages/724928020/Services+Rules+and+Guidelines](https://invisionapp.atlassian.net/wiki/spaces/ARCH/pages/724928020/Services+Rules+and+Guidelines)   
