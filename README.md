# Reanimator

[![semantic-release](https://img.shields.io/badge/%20%20%F0%9F%93%A6%F0%9F%9A%80-semantic--release-e10079.svg)](https://github.com/semantic-release/semantic-release)

Reanimator is a custom animator for Unity created 
[to simplify the development of Astortion](https://youtu.be/nA2IChvy_QU).

It's tailored specifically for traditional animations - animations represented by a series of 
cels displayed one after the other.
<br>
It's not modeled using a **finite state machine** but rather a **tree-like graph** - mitigating 
the "transition hell" problem while still providing all the basic functionalities you would expect 
an animation system to provide:

- animation switching
- transitional/one-off animations
- percentage-driven animations
- animation overrides
- events

### Installation
Reanimator can be [installed as a unity package](https://docs.unity3d.com/Manual/upm-ui-giturl.html) 
using this url:
```
https://github.com/aarthificial/reanimation.git
```
I'm still not sure if and when it will land on the Asset Store.

### Release video:
[![Saying goodbye to Animation State Machines](https://img.youtube.com/vi/5aHhmRiVpZI/maxresdefault.jpg)](https://youtu.be/5aHhmRiVpZI)

Please note, however, that I'm not claiming that this solution is better than the built-in animator. 
<br>
*It's an interesting alternative, that's it.*
<br>
I'm releasing it due to a lot of requests I got under my devlog, 
not because I'm trying to popularize it or something.
