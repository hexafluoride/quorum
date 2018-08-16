---
title: Quorum Specification Document Working Draft
---

Introduction
============

Quorum is feature-rich forum software. Quorum’s attractive features include a
flexible, lightweight, and modular architecture, granular access control,
support for multiple database backends and authentication protocols, and high
performance. Quorum uses various technologies to implement its functionality;
for an overview, please see [Technologies](#technologies).

Technologies
============

The main Quorum codebase is built on top of Nancy. Nancy is a web framework
written in C\# that prioritizes code clarity. Other software that is closely
involved with Quorum include Razor, PostgreSQL, and Bootstrap. Currently, only
PostgreSQL is supported as a database backend, however, since the database
implementation is quite modularized and abstract, any other backend may be
implemented with ease. Particularly, any database library that exposes an
ADO.NET interface is an easy target for porting.

Planned support includes compatibility with OpenID Connect as a user/session
provider.

Architecture
============

User authentication
-------------------

In this section, user authentication, identification, identity management and
session management are detailed.

### Login

The login operation consists of multiple steps working in tandem to identify and
authenticate a particular user. Here is what an example workflow may look like:

![Figure 1](figure-01.png)

### Registration

By default, Quorum requires a minimal amount of information to register and  
employs no vetting/spam mitigation. However, Quorum can be configured to deal 
with potential spammers and bots using a multitude of ways, including:

-   Requiring registrants to solve a CAPTCHA

-   Requiring email confirmation

-   Javascript Proof of Work

-   Requiring admins to confirm each registration before accounts are active

-   An invite system

### Anonymous users

Quorum can be configured to allow anonymous users to post. Internally, anonymous
 authors are tracked using their IP address for the purposes of spam prevention 
and moderation. Additionally, anonymous users are subjected to extra spam 
prevention measures, such as CAPTCHAs and stricter rate limits.

Forums and posting
------------------

This section goes into detail on the abstract workings of the forum.
A single installation of the Quorum software can be referred to as a “forum”,
which can be represented as a set of boards. In turn, boards can be represented
as an ordered sequence of boards and threads, typically arranged by last post
date, colloquially referred to as “bump order”.

### Threads

A thread is a sequence of posts. Threads are created and appended to boards by
posters. Each thread’s identifier is equal to the identifier of its opening post
(OP). In the context of a thread, the author of the opening post has special
privileges such as banning and unbanning certain users from posting in the
thread. A thread is generally expected to be centered around a certain topic,
and its representation reflects the social process of conversation, of which
linear progression is a natural model.

Posting in a thread normally "bumps" the thread to the front of the board. Users
 can opt out of this behavior, also referred to as "saging".

### Posts

Posts are the building block of discussion. Posts contain the following data:

-   Post content (in Markdown, BBcode, etc.)

-   Post title

-   Author

-   Creation date

-   Last edit date

Posts are represented by globally unique numeric identifiers that are consistent
across a single forum. The identifier is taken from a global counter that is
incremented by one after each successful post. This identifier can be used to
refer to posts, also called backlinking and quoting, from the originating thread
or from other threads, possibly from other boards as well. The global counter
cannot decrease, and post deletion leaves a gap in the sequence.

### Boards

Boards categorize and contain discussion around general topics. A forum usually
has multiple boards where users can engage in different subjects, possibly under
different rules and different posting mechanics. Boards serve to make
enforcement of these features and differences easier for the administrators.
Boards are hierarchically organized under board groups, and any given board is
either directly under a board group or under another board.

Each board can have its own access control structure that dictates whether a
board is visible to all users, whether posting and thread creation are allowed
to preapproved users or everyone, et cetera.

Boards are required to have a name and a description. They may have a shorthand
identifier (such as /r9k/) that can be used in URLs.

Boards can be configured to have limits on thread count, and post count per 
thread(in the form of restricting bumps after a certain number of posts).

### Board groups

A board group is a utility that is used to logically section boards apart. Each
board is either contained by another board or by a board group. Board groups are
either orphans(contained at the root of the virtual tree of boards and board
groups) or parented by another board group.

### Thread creation

Threads are created and appended to boards. The interface that allows for thread
creation is virtually identical to that of post creation, with the exception
that the newly created post is used as the opening post of the thread to be
created. Thread creation can be restricted to certain user roles on a per-board
basis.

### Post creation

Posts are appended to threads. The interface for post creation lets users use
different markup languages, such as Markdown or BBcode. Post creation can be
restricted using traditional access control paradigms, with the addition of
thread owners also being able to restrict certain users from posting in their
threads.

---

## Features specific to Wetfish Forums

This section contains previews of features that are mostly specific to Wetfish 
Forums, and are (to-be-)implemented using plugins.

Mockups will appear here.

### Coral

Coral is a cryptocurrency that is issued by the forums as part of a posting
reward scheme. Coral may be used to buy and sell customization items(see [Fish 
avatars](#Fish_avatars)), tip other users for their posts, or as a general-use 
currency. Coral is an asset hosted on the Stellar network.

### Fish avatars

One of the unique features part of the first iteration of the Wetfish Forums are
customizable fish avatars, featuring different items that can be added onto your
virtual avatar. The new Wetfish Forums backed by Quorum expands on this concept
by hosting data related to a user's avatar on the Stellar network,
introducing an additional degree of freedom and decentralization. As part of
this feature, users can equip, remove, and trade items on the decentralized
Stellar network itself. The specific implementation of this feature will use an
NFT framework for the Stellar network(currently unnamed and unreleased, however 
see [coral.topkek.party](http://coral.topkek.party/) for a WIP proof of concept)

### Cross-board backlinking
This feature is not specific to Wetfish Forums, as it will be implemented in
Quorum itself, and the idea is taken from various imageboards already in place.
Using the convenient property that posts can be globally identified and 
referenced, Quorum posts can reference other posts, regardless whether they're
in different threads or even different boards. The interface for this feature 
would:

-    Allow users to hover over backlinks to display a preview

-    Click on posts to expand them inline

-    View the list of posts that reference a specific post, divided by posts
     that are in the same thread, posts that are in the same board, and posts 
	 that are on different boards.

-    Automatically recognize and create backlinks using the syntax >>`post_id`