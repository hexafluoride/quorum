---
title: Quorum Specification Document Working Draft
---

Introduction
============

Quorum is high performance forum software. Quorum’s features include
a lightweight modular architecture, granular access control, and
support for multiple database backends / authentication protocols. 
Quorum uses various technologies to implement its functionality;
for an overview, please see [Technologies](#technologies).

Technologies
============

The core Quorum codebase is built on top of Nancy. Nancy is a web framework
written in C\# that prioritizes code clarity. Other software that is closely
involved with Quorum include Razor, PostgreSQL, and Bootstrap. Currently only
PostgreSQL is supported as a database backend, however in the future any other 
backend may be implemented with ease. Particularly any database library that
exposes an ADO.NET interface is an easy target for porting.

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

### User profiles

Each user has a profile that consists of information related to the user, such 
as the user's full name, avatar, bio, contact information, etc. Most of these 
fields are optional.

Additionally, Quorum plugins can store user-specific data in the user's profile 
for the purposes of letting Quorum handle user data and optionally displaying 
the data in a meaningful manner across the forum interface. One example would 
be the Fish avatars plugin that supersedes the forum avatar feature.

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
 
#### Thread creation

Threads are created and appended to boards. The interface that allows for thread
creation is virtually identical to that of post creation, with the exception
that the newly created post is used as the opening post of the thread to be
created. Thread creation can be restricted to certain user roles on a per-board
basis.

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

#### Post content

Posts contain rich text with support for formatting text and embedding media. 
Embedded media is handled by media support plugins that convert an embedded 
media identifier(such as a URL to a YouTube video) to an inline display. 
Furthermore, Quorum can also be configured to host media files for users.

#### Post creation

Posts are appended to threads. The interface for post creation lets users use
different markup languages, such as Markdown or BBcode. Post creation can be
restricted using traditional access control paradigms, with the addition of
thread owners also being able to restrict certain users from posting in their
threads.

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


Permissions
-----------

_This is the vision I have for a flexible permissioning system. This might get 
replaced by a more traditional approach, based on **your** feedback!_

## Permissioning

Quorum permissions are built upon the key concepts of _actions_, _scopes_ and 
_transactions_.

### Actions

An _action_ is basically what the name says, it represents a function that can 
bring a change to the state of Quorum. Examples of actions include creating a 
post, banning a user or changing an avatar.

### Scopes

Scopes are the subjects of actions. Examples of scopes corresponding to the 
action examples include the thread that is being posted to, the user being 
banned and the user getting its avatar changed.

### Transactions

Transactions are specific instances of actions. They can be thought of as 
function calls in that they represent a particular combination of an action, 
a caller, one or many scopes and a set of arguments.

A transaction is either granted or denied, and this decision is made according
 to permission rules.

### Permission rules

Permission rules are 4-tuples of a source, an action, a scope and the given 
permission for the source-action-scope combination(grant/deny). When a 
transaction is being considered for application, Quorum walks through all of the
 permission rules that match the source, action, and all of the scopes under one 
scope type in the transaction. These rules are then ranked by _specificity_, and
 the most specific rule determines whether the transaction is applied or not.

Example: the user "rachel" creating a new post in the thread 30 would be 
represented by the following transaction:

    "transaction": {
        "source": { "uid": 2 },
        "scopes": [
            { "type": "board_group", "id": 10 },
            { "type": "board", "id": 20 },
            { "type": "thread", "id": 30 },
        ],
        "action": "ACTION_POST"
    }

__Note: this structure is solely for the purpose of visualizing the permission 
process.__

With a permission table that looks like this(simplified for example):

| Source | Action      | Scope                               | Permission |
|--------|-------------|-------------------------------------|------------|
| rachel | ACTION_POST | *                                   | GRANT      |
| rachel | ACTION_POST | { "type": "board_group", "id": 10 } | GRANT      |
| rachel | ACTION_POST | { "type": "board", "id": 20 }       | DENY       |
| rachel | ACTION_POST | { "type": "thread", "id": 30 }      | GRANT      |
| Emcy   | ACTION_BAN  | *                                   | DENY       |
| Emcy   | ACTION_BAN  | { "type": "role", "id": 1 }         | GRANT      |

The most specific permission entry for the action ACTION_POST is in the 4th row,
 which lets rachel post in the thread 30, even though it appears that rachel has
 been forbidden from posting on the board 20. This makes sense because posting 
in any other thread in the board 20 is still forbidden, as the most specific 
rule in that case is the one in the line 2, which denies her from posting in 
that board. The special scope "*" is used to denote a default permission level 
that works across all scopes, of course as long as there are no other permission 
 entries that concern that scope.

The user "Emcy" banning the user "h"(uid 4, roles include role 1 and 2) would be 
 represented as:

    "transaction": {
        "source": { "uid": 3 },
        "scopes": [
            { "type": "role", "id": 1 },
            { "type": "role", "id": 2 },
            { "type": "user", "uid": 4 }
        ],
        "action": "ACTION_BAN"
    }

In this example, we can see that Emcy is allowed to ban anyone with the role of 
1, however, there are multiple scopes that have the type of "role". In a 
scenario like this, all scopes under a single type must be granted in order for 
the transaction to be granted. This can be visualized like:

    "scopes": [
        { "type": "role", "id": [1, 2] },
        { "type": "user", "uid": 4}
    ]
    
Since are no entries that let Emcy ban someone with a role of 2, this 
transaction would be denied. Scopes that have the same type are treated like a 
single scope that fails if any one of the sub-scopes fail.

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

### Banning users from own threads

Thread authors can ban users from threads they create. This is implemented via 
a plugin that lets thread authors add a permission that looks like: 

| Source | Action      | Scope                               | Permission |
|--------|-------------|-------------------------------------|------------|
| target | ACTION_POST | { "type": "thread", "id": 3 }       | DENY       |

Where `target` is the user to ban.
