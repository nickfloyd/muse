## Continuous delivery is...
- the ability to put the release schedule in the hands of the business, not in the hands of development or operations; making sure that any build could potentially be released to users at the touch of a button using a fully automated process, quickly.

## A continuous delivery ecosystem...
relies on comprehensive automation of the build, test and deployment process
needs a high degree of collaboration between developers, testers, DBAs, DevOps, users and the business
consider code as “done” when it is working in production
does not use testing or deployment “phases” but rather releases software features as they become complete and not as packages
individual stories have had the entire test suite run against the build containing them
has tests at the unit, component (integration tests) and acceptance level
has stories that have been demonstrated to the customers / POs from a production-like environment
is a system where there are NO obstacles to deploying to production


## Continuous deployment (release) is...
 - the ability and behavior of shipping your code to users as often as possible, ensuring that you just don’t “think” you have a shippable build but that you have a shipped build.

A continuous deployment (release) ecosystem...
has developers with the ability to push to production
has an implemented “feature dark” framework where the business can turn on and off features at will
has quality gates that prevent most issues getting into production
builds mainline continuously and measures quality and records metrics
runs all tests all of the time

## Resources:

Timothy Fitz on Continuous Deployment
http://timothyfitz.wordpress.com/2009/02/08/continuous-deployment/

Jez Humble on Continuous Delivery vs. Continuous Deployment
http://continuousdelivery.com/2010/08/continuous-delivery-vs-continuous-deployment/

Continuous Delivery by Jez Humble and David Farley
http://www.amazon.com/gp/product/0321601912

Jez Humble on Organization of Continuous Delivery in an Organization
http://continuousdelivery.com/2011/12/organize-software-delivery-around-outcomes-not-roles/

