# Overview

Given that the line between web and client-based applications have been blurring over the past few years implementations and patterns like REST seem to fit less and less in the new world interconnected applications \- the "Server" is no longer the center of the web-based world (see: headless architectures, ephemeral jobs such as AWS Lambda and Azure Functions, and client-side caching models with asynchronous interoperations). 

The fact is that there is no one size fits all architecture or patterned approach which is why we have to drive our efforts from context (what do we have and how does it work today) and vision (how do we see things working in the future to support our business then and the environments where we serve our customers).

There are tons of side steps that we can take, ask what "if" until another API framework emerges, or even poke holes in the never-ending collection of scenarios that cause us to question every move we make \- we're not going to do that.  This write-up will seek to propose approaches to help us answer how can we technically achieve a Public API surface given what we know, what our vision is, and what [product communicates](https://invisionapp.atlassian.net/wiki/spaces/PPT/pages/897582480) as the important needs of our users.

# Considerations

Initially, we will need to discuss these things in generalities and narrow our focus as we get closer to our product vision.  This means when we discuss things like GraphQL we are talking in terms of a standard implementation \- it's an approach for querying data across disparate services, or when we discuss REST we are talking about it as stated in Chapter 5 of Roy Fielding's dissertation and how that applies to RFC 2616 \- we need to keep our perspectives simple; otherwise we'll get caught in the tight loop of nuance.

We should consider the following when discussing the "right fit" for an API approach (in no particular order):

* The current company platform and technology stack (Microservices, Kubernetes, Node, GO)  
* The current company software and patterns (BFF, Domain Services, Web-based applications, current integrations, so on)  
* The current company data store (both transactional and replicated \- data warehouse), the mechanisms of storage, and the shape of the data models   
* The current company authentication and authorization scheme  
* How does each approach handle request efficiency  
* How does each approach handle discovery (Documentation, schema discovery, so on)  
* How does each approach handle reading and writing data \- both should be equally simplistic as the other  
* How does each approach handle real-world Operational context (how easy is it to get up and running, how easy is it to use, simple configuration, is it secure, does it handle error scenarios well) 

# Approaches

Note, just because these approaches are listed as separate prospects they are not mutually exclusive and could even be used in tandem to solve some of our contextual challenges.

#### **Legend (Values translated from rankings)**

| \+1 | \+0 | \-1 |
| :---- | :---- | :---- |

|  | JSON:api | GraphQL | REST | gRPC w/proto3 |
| :---- | :---- | :---- | :---- | :---- |
| The current company platform and technology stack (Microservices, Kubernetes, Node, GO) | Simple, non-issue \- serialization libraries, various [implementations](https://jsonapi.org/implementations/), as well as infrastructure resources exist both inside and outside of company to accomplish the construction of an API that adheres to JSON:API standards and patterns | Incredibly simple, non-issue \- serialization libraries, various [implementations](https://graphql.org/code/), as well as infrastructure resources, exist both inside and outside of company to accomplish the construction of an API that adheres to GraphQL standards and patterns | Currently implemented.  Though various implementations are in prod.  We would benefit from following the tenets outlined in [Services: Rules and Guidelines](https://invisionapp.atlassian.net/wiki/spaces/ARCH/pages/724928020/Services%3A+Rules+and+Guidelines) | Services, not Objects, Messages not References \- Promote the microservices design philosophy of coarse-grained message exchange between systems while avoiding the pitfalls of distributed objects and the fallacies of ignoring the network. |
| The current company software and patterns (BFF, Domain Services, Web-based applications, current integrations, so on) | As with data store considerations, JSON:API would provide a sufficient, schema and validation considerations will make this more of a primary challenge and we unwrap the models that we want to make public. | As with data store considerations, GraphQL would provide an excellent overlay for our current implementation set given API and data source stitching is incredibly simple with GraphQL and was one of its core design principles.  | Current; various implementations are in prod.  We would benefit from following the tenets outlined in [Services: Rules and Guidelines](https://invisionapp.atlassian.net/wiki/spaces/ARCH/pages/724928020/Services%3A+Rules+and+Guidelines) | gRPC can be carried over HTTP2 framing as well as other transport options.  It also supports server to server communication. |
| The current company data store (both transactional and replicated \- data warehouse), the mechanisms of storage, and the shape of the data models  | Sufficient, schema and validation considerations will make this more of a primary challenge and we unwrap the models that we want to make public. | API and data source stitching is incredibly simple with GraphQL and was one of its core design principles.  | With our expansive data model and the fact that public based APIs are less esoteric and more generalized (unless we go with a heavy emphasis on Realms) \- a REST-only implementation will be restrictive. | We would have to reformat our data representation to fit the proto buf messages. |
| The current company authentication and authorization scheme | Simple, OAuth and token-based approaches exist (such as JWT as it is currently implemented for our BFFs) | Simple, OAuth and token-based approaches exist (such as JWT as it is currently implemented for our BFFs).  Libraries such as graphql-yoga also implement middlewares that make this even more simplistic  | Simple, OAuth and token-based approaches exist (such as JWT as it is currently implemented for our BFFs) | Simple, Supported auth mechanisms are SSL/TLS and Token-based auth (OAuth2)  |
| How does each approach handle **request efficiency** | Excellent; a single request is usually sufficient for most needs. Responses can be tailored to return only what is required. | Excellent; a single request is usually sufficient for most needs. Responses only include exactly what was requested. | Poor; multiple requests are needed to satisfy common needs. Responses are bloated. | Requests are made to a gRPC server that exposes specific methods from the server \- these must be composed individually but can perform incredibly well due to protocol buffers as a serialized data structure |
| **Request Efficiency** \- Multiple data objects in a single response | Yes | Yes | Usually; but every implementation is different (for Drupal: custom "REST Export" view or custom REST plugin needed). | Acceptable. It can be formatted to combine many worker services to return requests to individual clients.  |
| **Request Efficiency** \- Embed related data (e.g. the author of each comment) | Yes | Yes | No | Yes \- specific to the implementation and data structure serialized. |
| **Request Efficiency** \- Only needed fields of a data object | Yes; servers may choose sensible defaults, developers must be diligent to prevent over-fetching. | Yes; strict, but eliminates over-fetching, at the extreme, it can lead to poor caching. | No | No \- fixed data structures |
| How does each approach handle discovery (**Documentation**, schema discovery, so on) | Acceptable; generic schema only; links and error messages are self-documenting. | Excellent; precise schema; excellent tooling for exploration and documentation. | Poor; no schema, not explorable. | Acceptable.  Using a gRPC plugin like protoc client and server code can be generated |
| **Documentation** \- Auto-generated documentation | It depends; if using the OpenAPI standard. | Yes; various tools available. | It depends; if using the OpenAPI standard. | Acceptable, but obscure. The concept of metadata is opaque to gRPC itself \- it lets the client provide information associated with the call to the server and vice versa. Access to metadata is language-dependent. |
| **Documentation** \- Interactivity | Acceptable; observing available fields and links in its responses enable exploration of the API. | Excellent; autocomplete feature, instant results or compilation errors, complete and contextual documentation. | Poor; navigable links rarely available. Especially without HATEAOS | Poor; navigation must be discovered through client and server implementations |
| **Documentation** \- Validatable and programmable schema. | Depends; the JSON:API specification defines a generic schema, but a reliable field-level schema is not yet available. | Yes; a complete and reliable schema is provided (with very few exceptions). | It depends; if using the OpenAPI standard. | Tight coupling between server and client |
| How does each approach handle reading and writing data \- both should be equally simplistic as the other | Excellent; Writes are handled is clearly defined by the spec, one write per request, but multiple writes are being added to the specification. JSON:API prescribes a complete solution for handling writes. Bulk operations are coming soon. | Poor; Writes are left to each implementation and there are competing best practices, it's possible to execute multiple writes in a single request. GraphQL supports bulk/batch operations, but writes can be tricky to design and implement. There are competing conventions. | Acceptable; HTTP semantics give some guidance but how specifics left to each implementation, one write per request. Every implementation is different. No bulk support. | Poor; Writes are left up to the implementation |
| How does each approach handle real-world **Operational Context** (how easy is it to get up and running, how easy is it to use, simple configuration, is it secure, does it handle error scenarios well)  | Excellent; works out of the box with CDNs and reverse proxies, no client-side libraries needed, but many are available and useful. | Poor; extra infrastructure is often necessary client-side libraries are a practical necessity, specific patterns required to benefit from CDNs and browser caches. | Acceptable; works out of the box with CDNs and reverse proxies; few to no client-side libraries required. | Poor; client must have complete knowledge of the server's implementation \- this is one of the strengths of gRPC in a closed system where both server and consumer are controlled by the same group |
| **Operational Context** \- Scalability: additional infrastructure requirements | Excellent; same as a regular website (Varnish, CDN, etc). | Usually poor; only the simplest queries can use GET requests; to reap the full benefit of GraphQL, servers need their own tooling. | Excellent; same as a regular website (Varnish, CDN, etc). | The stack should be available on every popular development platform and easy for someone to build for their platform of choice. It should be viable on CPU & memory limited devices. |
| **Operational Context** \- Tooling ecosystem | Excellent; lots of developer tools available; tools don't need to be implementation-specific. | Excellent; lots of developer tools available; tools don't need to be implementation-specific. | Acceptable; lots of developer tools available, but for the best experience they need to be customized for the implementation. | Acceptable; many tools, language implementations exist \- see [this](https://github.com/grpc-ecosystem/awesome-grpc) curated list |
| **Operational Context** \- Typical points of failure | Fewer; server, client. | Many; server, client, client-side caching, client and build tooling. | Fewer; server, client. | Many; General errors, network failures, and protocol errors exist and are implemented using error codes custom to gRPC |
| **Operational Context** \- Versioning | Versioning is handled much like REST and has lead to a [lengthy amount of debate](https://github.com/json-api/json-api/issues/406). Header versioning, URI versioning using simver as a base structure for the version is typically applied. NOTE: JSON:API has no facilities to keep track of object modifications; this is at a lower-level than the interchange format. | Query defined representation and internal patterns native to GraphQL provide deprecation schemes \- note this is a specification and not an architectural style like REST.  [Lee Byron describes GraphQL as a "version free" implementation](https://engineering.fb.com/core-data/graphql-a-data-query-language/). | Versioning can be handled via URI, HATOAS (via vendor (vnd) headers \- possibly a bit more challenging to cache this way)  \- this is typically handled as a development discipline and becomes problematic as the Public API surface grows | Recommended \- Semantic versioning \- backward and forward compatibility is resolved through the implementation |
| **Developer Experience:** Familiarity, Ease of Use, Accessibility | Acceptable; It does have a simple transactional style which provides a great DX, however the way we iterate on unwrapping our data models for public use to fit this could potentially incur ongoing user frustration upfront as we spend time making sure the majority of use cases are covered. Also, this style will likely produce bloated responses that will add an extra load on the developer for filtering the data down to exactly what they're asking for. | Good; Compared to the long-term popularity of API styles like REST, Graph APIs are newer to the spectrum of developers we serve. However, we're seeing strong adoption, and rapidly growing familiarity and community support. This indicates that it's likely just a matter of time for GraphQL to become as familiar to the development community as other alternatives. GraphQL provides the most delightful DX out of all our options, and is an easily accessible style for both backend and frontend developer personas to use and reason about. | Acceptable; There's a cognitive barrier to entry when first trying to understand the nuances of any specific REST API which requires the developer to spend time reading documentation, rather than being enabled to implicitly reason about what they might be able to do by just be being familiar with the API style and type of data they're looking for. | Poor; gRPC is the least familiar paradigm/style across the types of developers we serve, and will require significant developer education to get them started. |
| **Developer Experience:** Ease of maintaining great documentation | Excellent; Will be simple enough to leverage general documentation tooling from the JS/Web ecosystem that will allow us to develop and maintain sustainable/always-up-to-date documentation. | Excellent; Has a robust and growing ecosystem of fantastic documentation generation tooling that will make it easy for us to develop and maintain sustainable/always-up-to-date docs. | Acceptable; Will likely require non-trivial development time to set up sustainable/always-up-to-date documentation in a way that addresses the nuances of our specific implementation. | Poor; Sparse ecosystem tooling & support for documentation generation. Will require significant development time to scaffold sustainable/always-up-to-date documentation that helps developers get familiar with our style, and addresses the nuances/eccentricities of our API. |
| **Developer Experience:** Community Support | Acceptable; Has a steady but fairly small community in comparison to our other options. | Excellent; There are 185k public repos using GraphQL on Github at present, and lots of interest in its [reference implementation repo](https://github.com/graphql/graphql-js). GraphQL is also governed by a centralized community-driven [foundation](https://foundation.graphql.org/) which operates under the Linux Foundation. This is a huge community win for us and our users. It will significantly increase GraphQL's growth and sustainability, scaling community ownership & relevance over time – ensuring that it doesn't cater to the interests of any particular company and become less useful for us over time. In fact, with this option we could increase our public developer presence/relevance in open source by participating in the development of GraphQL and bringing our use cases to the table. | Good; REST has been the predominant API style for a long time. Every implementation is different, and there's no centralized developer community – however, it is [the most popular API style right now](https://speakerdeck.com/zdne/what-api-your-guide-to-api-styles?slide=9) and there are plenty of tools and implementation examples available for us to draw from. | Acceptable; There's significant interest in grpc, and some supportive tooling available – however, it's not seeing nearly as fast of an adoption rate as our other options (eg. REST, GraphQL) and may have the potential to always remain the more niche tool among them. |
| **Developer Experience:** company data parity (does the API style logically align with the types of data our products will expose in a way that makes sense to our developers?) | Good; Fits the current transactional style of our internal APIs, however responses will likely be bloated and require the developer to spend more time filtering the data down to get only what they want. | Excellent; Developers will be able to articulate exactly what data they want from a given product and think about it in a way that's more object oriented than service driven, making it easy to reason about exactly what they want, and when. Overlaying our current APIs with GraphQL would make interacting with our data store delightful, and abstract away bloat. | Good; Would fit our current model, but produce bloated responses requiring developers to spend time filtering, etc. | Poor; Requires us to reformat our data representation (for proto buf), which will not only be expensive for us – but could introduce artifacts that make the API behave in ways the end user doesn't expect. |
| TOTALS | 11 | 9 | \-2 | \-2 |

# NOTES

#### **Webhooks**

* Should be self-explanatory  
  * Contain basic data aspects: timestamp,  
    expiration, id, type, body if dealing  
    with a resource  
* Should be consistent: always provide  
  a consistent structure  
* Consumers should be allowed to define  
  more than one URL  
* Should provide consumers with both API and UI based subscription services  
* Should have an additional verification token so that consumers can verify the payloads \- much like file signing  
* Should consider implementing a default  
  expiration

#### **JSON:API**

* Should be consistent with the specification  
  [jsonapi.org](http://jsonapi.org/)  
* Should play to the core strengths of its protocol resource flexibility, known data format, compound resources, caching, paging

#### **GraphQL**

* This aspect of the API surface is optional  and will be used when complex queries and mutations are necessary

# Definitions

[REST](https://en.wikipedia.org/wiki/Representational_state_transfer) : Representational state transfer (REST) is a software architectural style that defines a set of constraints to be used for creating Web services.

[GraphQL](https://graphql.github.io/graphql-spec/June2018/) : a query language and execution engine originally created at Facebook in 2012 for describing the capabilities and requirements of data models for client‐server applications.

[gRPC](https://grpc.io/) : a modern open-source, high-performance RPC framework that can run in any environment. It can efficiently connect services in and across data centers with pluggable support for load balancing, tracing, health checking and authentication. It is also applicable in the last mile of distributed computing to connect devices, mobile applications, and browsers to backend services.

[JSON:API](https://jsonapi.org/) : A specification for building APIs in JSON

# References 

[RFC 2616](https://tools.ietf.org/html/rfc2616)

[Roy Fielding's dissertation](https://www.ics.uci.edu/~fielding/pubs/dissertation/fielding_dissertation.pdf)

[Drupal Core write up on choosing a service implementation](https://dri.es/headless-cms-rest-vs-jsonapi-vs-graphql#operational-efficiency)

[JSON:API Implementations](https://jsonapi.org/implementations/)

[GraphQL implementations](https://graphql.org/code/)

[Authentication and Authorization Basics with GraphQL and REST](https://www.prisma.io/tutorials/graphql-rest-authentication-authorization-basics-ct20)

[Services: Rules and Guidelines](https://invisionapp.atlassian.net/wiki/spaces/ARCH/pages/724928020/Services%3A+Rules+and+Guidelines)

[Your API versioning is wrong, which is why I decided to do it 3 different wrong ways](https://www.troyhunt.com/your-api-versioning-is-wrong-which-is/)

[Guidance On Versioning Your API From Google](https://apievangelist.com/2017/03/09/guidance-on-versioning-your-api-from-google/)

[Backward and Forward Compatibility, Protobuf Versioning, Serialization](https://www.beautifulcode.co/blog/88-backward-and-forward-compatibility-protobuf-versioning-serialization)  
