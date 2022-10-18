VAR relicsDiscussed = false

-> Main

=== Main ===
Rock Boy! Thank the crystals you survived the cave in! #speaker:Granite Elder #layout:right #portrait:None
Elder, I have questions... what happened? Where is this? What are these things I keep fi- #speaker:Rock Boy #layout:left #portrait:None
Hold on there my boy, you're getting way ahead of yourself! Please, calm down, and ask me one thing at a time. #speaker:Granite Elder #layout:right #portrait:None
->Questions

=== Questions ===
What did you want to ask first? #speaker:Granite Elder #layout:right #portrait:None
+ [What Happened]
    What happened? Everything was so quiet, I was off doing some digging and all of a sudden I was falling through the depths and into the caves below! #speaker:Rock Boy #layout:left #portrait:None
    Hmm... that I'm not sure on. In all my years in these tunnels, I'd never witnessed anything as bad as this. Though my predecessor did foretell of something like this happening. #speaker:Granite Elder #layout:right #portrait:None
    It is said that when the caverns crumble, a great disaster will befall the sub-surface world, wiping out all life. Unless many relics, imbued with power, are gathered.
    Only then will they bring stability to our world in the deep.
    ~relicsDiscussed = true
+ [Where is this?]
    Where is this place? What is it? It seems to connect to the caves, not to mention- #speaker:Rock Boy #layout:left #portrait:None
    I wish I could tell you, however not even I know of this place. It certainly is an enigma. #speaker:Granite Elder #layout:right #portrait:None
    But, judging by how aged and withered the stone is, I'd guess it's been here for countless generations. It's worth investigating that's for certain
+ [Relics]
    {
        -relicsDiscussed: ->TheRelicsProphecy 
        - else: ->WhatIsThisRelic
    }
- Do you have anything else you'd wish to ask? I am willing to listen.
+ [Yes]
    -> Questions
+ [No]
    -> Plan

=== TheRelicsProphecy ===
Wait, relics? Do you consider this a relic? #speaker:Rock Boy #layout:left #portrait:None
Hmm... this does fit the description of the prophecy. Relics imbued with power... there's some kind of otherworldly energy emanating from this slab here.. #speaker:Granite Elder #layout:right #portrait:None
It might be worth searching for more of these, they could be our hope for the sub-surface world!
- Do you have anything else you'd wish to ask? I am willing to listen.
+ [Yes]
    -> Questions
+ [No]
    -> Plan
=== WhatIsThisRelic ===
Granite Elder, I found this weird relic while I was escaping the caves. Do you have any idea what this could be? #speaker:Rock Boy #layout:left #portrait:None
How odd.. and you say this was just sitting there? Perhaps there's more to this... I'll look into this. #speaker:Granite Elder #layout:right #portrait:None
- Do you have anything else you'd wish to ask? I am willing to listen.
+ [Yes]
    -> Questions
+ [No]
    -> Plan

=== Plan ===
Now that we have a better grasp of the situation... Rock Boy! #speaker:Granite Elder #layout:right #portrait:None
Yes Granite Elder! #speaker:Rock Boy #layout:left #portrait:None
I felt some strange energy from somewhere above us, you should take a look, see if there's any connection between it and these relics of ours. Come back to me when you find something! #speaker:Granite Elder #layout:right #portrait:None
You've got it! #speaker:Rock Boy #layout:left #portrait:None
->END

=== DemoEnd ===
Rock Boy! Thank the crystals you survived the cave in! But it's not time to invesigate yet! #speaker:Granite Elder #layout:right #portrait:None
You will have plenty of time to find out what's going on in the full game! But for now, just rest.
-> END